
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RambutanSmartGarden.Api.Common.Exceptions;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Authentication;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers và OpenAPI.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Global Exception Handling.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Entity Framework Core và SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Chưa cấu hình ConnectionStrings:DefaultConnection.");

    options.UseSqlServer(connectionString);
});

// Đăng ký Password Hasher.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

// Đăng ký dịch vụ bootstrap System Admin.
builder.Services.AddScoped<AdminBootstrapper>();

// Đọc cấu hình JWT.
var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "Chưa cấu hình JWT.");

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
    string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
    throw new InvalidOperationException(
        "Jwt:Issuer và Jwt:Audience không được để trống.");
}

if (jwtOptions.AccessTokenMinutes <= 0 ||
    jwtOptions.AccessTokenMinutes > 60)
{
    throw new InvalidOperationException(
        "Jwt:AccessTokenMinutes phải từ 1 đến 60.");
}

byte[] signingKeyBytes;

try
{
    signingKeyBytes = Convert.FromBase64String(
        jwtOptions.SigningKey);
}
catch (FormatException exception)
{
    throw new InvalidOperationException(
        "Jwt:SigningKey phải là chuỗi Base64 hợp lệ.",
        exception);
}

if (signingKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:SigningKey phải có ít nhất 32 byte.");
}

builder.Services.AddSingleton(jwtOptions);

// JWT Bearer Authentication.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,

                ValidateLifetime = true,
                RequireExpirationTime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(signingKeyBytes),

                RequireSignedTokens = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                NameClaimType = "sub",
                RoleClaimType = "role"
            };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Chế độ khởi tạo System Admin bằng Terminal.
if (args.Contains(
        "--bootstrap-admin",
        StringComparer.OrdinalIgnoreCase))
{
    if (!app.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Bootstrap Admin chỉ được phép trong Development.");
    }

    await using var scope = app.Services.CreateAsyncScope();

    var bootstrapper = scope.ServiceProvider
        .GetRequiredService<AdminBootstrapper>();

    var succeeded = await bootstrapper.RunAsync();

    Environment.ExitCode = succeeded ? 0 : 1;

    // Không khởi động Web Server khi chạy bootstrap.
    return;
}

// Global Exception Handling.
app.UseExceptionHandler();

// Swagger chỉ bật trong Development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
