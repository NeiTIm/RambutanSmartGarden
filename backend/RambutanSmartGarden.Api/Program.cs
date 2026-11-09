
var builder = WebApplication.CreateBuilder(args);

// Đăng ký các dịch vụ của ứng dụng.
builder.Services.AddControllers();

// Đăng ký dịch vụ tạo tài liệu OpenAPI.
builder.Services.AddOpenApi();

var app = builder.Build();

// Chỉ bật OpenAPI và Swagger UI trong môi trường Development.
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

// Ánh xạ các API Controller.
app.MapControllers();

app.Run();
