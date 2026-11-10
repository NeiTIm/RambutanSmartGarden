
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RambutanSmartGarden.Api.Common.Exceptions;
using RambutanSmartGarden.Api.Features.Auth.Repositories;
using RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Auth.Services;
using RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Features.Users.Repositories;
using RambutanSmartGarden.Api.Features.Users.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Services;
using RambutanSmartGarden.Api.Features.Users.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authentication;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers và OpenAPI.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Global Exception Handling.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// EF Core và SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Chưa cấu hình ConnectionStrings:DefaultConnection.");

    options.UseSqlServer(connectionString);
});

// Password Hashing.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

// Bootstrap System Admin.
builder.Services.AddScoped<AdminBootstrapper>();

// Repositories.
builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();

builder.Services.AddScoped<
    IAuthRepository,
    AuthRepository>();
builder.Services.AddScoped<
    IPasswordRepository,
    PasswordRepository>();
// Services.
builder.Services.AddScoped<
    IAdminUserService,
    AdminUserService>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    IPasswordService,
    PasswordService>();
// JWT Configuration.
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

builder.Services.AddSingleton<
    IJwtTokenService,
    JwtTokenService>();

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

        // Kiểm tra phiên trong SQL Server sau khi JWT hợp lệ.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var subject = context.Principal?
                    .FindFirst("sub")?.Value;

                var session = context.Principal?
                    .FindFirst("sid")?.Value;

                if (!Guid.TryParse(subject, out var userId) ||
                    !Guid.TryParse(session, out var sessionId))
                {
                    context.Fail(
                        "Token không chứa thông tin phiên hợp lệ.");
                    return;
                }

                var dbContext = context.HttpContext
                    .RequestServices
                    .GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;

                var isActiveSession = await dbContext.UserSessions
                    .AsNoTracking()
                    .AnyAsync(
                        s =>
                            s.Id == sessionId &&
                            s.UserId == userId &&
                            s.RevokedAt == null &&
                            s.ExpiresAt > now &&
                            dbContext.Users.Any(u =>
                                u.Id == userId &&
                                u.IsActive),
                        context.HttpContext.RequestAborted);

                if (!isActiveSession)
                {
                    context.Fail(
                        "Phiên đăng nhập đã hết hạn hoặc bị thu hồi.");
                }
            }
        };
    });

// Authorization.
builder.Services.AddScoped<
    IAuthorizationHandler,
    SystemAdminAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SystemAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.AddRequirements(
            new SystemAdminRequirement());
    });
});

var app = builder.Build();

// Khởi tạo System Admin bằng Terminal.
if (args.Contains(
        "--bootstrap-admin",
        StringComparer.OrdinalIgnoreCase))
{
    if (!app.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Bootstrap Admin chỉ được phép trong Development.");
    }

    await using var scope =
        app.Services.CreateAsyncScope();

    var bootstrapper = scope.ServiceProvider
        .GetRequiredService<AdminBootstrapper>();

    var succeeded = await bootstrapper.RunAsync();

    Environment.ExitCode = succeeded ? 0 : 1;

    return;
}

// Global Exception Handling.
app.UseExceptionHandler();

// Swagger chỉ dành cho Development.
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
