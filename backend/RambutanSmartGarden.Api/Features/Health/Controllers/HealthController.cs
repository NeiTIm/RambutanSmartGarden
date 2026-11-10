
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Common.Results;
using RambutanSmartGarden.Api.Features.Health.DTOs;

namespace RambutanSmartGarden.Api.Features.Health.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Application = "RambutanSmartGarden.Api"
        });
    }

    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        var response = new ApiResponse<object>(
            new
            {
                Application = "RambutanSmartGarden.Api",
                Version = "1.0.0"
            },
            "Lấy thông tin ứng dụng thành công."
        );

        return Ok(response);
    }

    [HttpPost("validation-demo")]
    public IActionResult ValidateDemo(
        [FromBody] ValidationDemoRequest request)
    {
        return Ok(new ApiResponse<object>(
            new
            {
                request.TreeCode,
                request.TreeName,
                request.Age
            },
            "Dữ liệu đầu vào hợp lệ."
        ));
    }
}
