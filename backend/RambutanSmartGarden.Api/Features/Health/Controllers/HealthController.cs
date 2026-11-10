
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Common.Results;

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
    var result = new ApiResponse<object>(
        new
        {
            Application = "RambutanSmartGarden.Api",
            Version = "1.0.0"
        },
        "Lấy thông tin ứng dụng thành công."
    );

    return Ok(result);
}

}
