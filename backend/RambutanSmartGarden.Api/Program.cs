
using RambutanSmartGarden.Api.Common.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controller.
builder.Services.AddControllers();

// Đăng ký OpenAPI.
builder.Services.AddOpenApi();

// Đăng ký Global Exception Handler.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Đăng ký ProblemDetails.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Middleware xử lý exception.
// Đặt sớm để bắt lỗi từ các middleware phía sau.
app.UseExceptionHandler();

// Chỉ bật OpenAPI và Swagger UI trong Development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

// Middleware xử lý HTTP request.
app.UseHttpsRedirection();

app.UseAuthorization();

// Ánh xạ Controller.
app.MapControllers();

app.Run();
