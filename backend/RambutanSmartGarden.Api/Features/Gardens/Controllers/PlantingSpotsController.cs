
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/planting-spots")]
public sealed class PlantingSpotsController : ControllerBase
{
    private readonly IPlantingSpotService _service;

    public PlantingSpotsController(IPlantingSpotService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlantingSpotResponse>>> GetAll(
        [FromRoute] Guid gardenId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(
            User,
            gardenId,
            cancellationToken);

        if (result.Status != PlantingSpotStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn hoặc không có quyền truy cập"
            });
        }

        return Ok(result.Spots);
    }

    [HttpGet("{spotId:guid}")]
    public async Task<ActionResult<PlantingSpotResponse>> GetById(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid spotId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            User,
            gardenId,
            spotId,
            cancellationToken);

        if (result.Status != PlantingSpotStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vị trí trồng"
            });
        }

        return Ok(result.Spot);
    }

    [HttpPost]
    public async Task<ActionResult<PlantingSpotResponse>> Create(
        [FromRoute] Guid gardenId,
        [FromBody] CreatePlantingSpotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            User,
            gardenId,
            request,
            cancellationToken);

        return result.Status switch
        {
            PlantingSpotStatus.Succeeded =>
                CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        gardenId,
                        spotId = result.Spot!.Id
                    },
                    result.Spot),

            PlantingSpotStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin vị trí trồng không hợp lệ"
                }),

            PlantingSpotStatus.InvalidGardenArea =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Khu vực không thuộc vườn này"
                }),

            PlantingSpotStatus.DuplicateCode =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Mã vị trí đã tồn tại trong vườn"
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn hoặc không có quyền"
            })
        };
    }

    [HttpPut("{spotId:guid}")]
    public async Task<ActionResult<PlantingSpotResponse>> Update(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid spotId,
        [FromBody] UpdatePlantingSpotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            User,
            gardenId,
            spotId,
            request,
            cancellationToken);

        return result.Status switch
        {
            PlantingSpotStatus.Succeeded =>
                Ok(result.Spot),

            PlantingSpotStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin cập nhật không hợp lệ"
                }),

            PlantingSpotStatus.InvalidGardenArea =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Khu vực không thuộc vườn này"
                }),

            PlantingSpotStatus.DuplicateCode =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Mã vị trí đã tồn tại trong vườn"
                }),

            PlantingSpotStatus.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Thông tin vị trí đã thay đổi",
                    Detail = "Vui lòng tải lại dữ liệu trước khi cập nhật."
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vị trí hoặc không có quyền"
            })
        };
    }
}
