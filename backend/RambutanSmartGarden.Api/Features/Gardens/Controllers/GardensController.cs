
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens")]
public sealed class GardensController : ControllerBase
{
    private readonly IGardenService _gardenService;

    public GardensController(IGardenService gardenService)
    {
        _gardenService = gardenService;
    }

    // GET /api/gardens
    [HttpGet]
    public async Task<ActionResult<List<GardenListItemResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var currentUserId))
        {
            return Unauthorized();
        }

        var gardens = await _gardenService.GetAccessibleGardensAsync(
            currentUserId,
            cancellationToken);

        if (gardens is null)
        {
            return Unauthorized();
        }

        return Ok(gardens);
    }

    // GET /api/gardens/{gardenId}
    [HttpGet("{gardenId:guid}")]
    public async Task<ActionResult<GardenDetailResponse>> GetById(
        [FromRoute] Guid gardenId,
        CancellationToken cancellationToken)
    {
        var result = await _gardenService.GetByIdAsync(
            User,
            gardenId,
            cancellationToken);

        if (result.Status != GardenOperationStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn"
            });
        }

        return Ok(result.Garden);
    }

    // POST /api/gardens
    [HttpPost]
    [Authorize(Policy = "SystemAdmin")]
    public async Task<ActionResult<GardenResponse>> Create(
        [FromBody] CreateGardenRequest request,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var creatorUserId))
        {
            return Unauthorized();
        }

        var result = await _gardenService.CreateAsync(
            request,
            creatorUserId,
            cancellationToken);

        if (result.Status == CreateGardenStatus.InvalidRequest)
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Thông tin vườn không hợp lệ"
            });
        }

        return StatusCode(201, result.Garden);
    }

    // PUT /api/gardens/{gardenId}
    [HttpPut("{gardenId:guid}")]
    public async Task<ActionResult<GardenDetailResponse>> Update(
        [FromRoute] Guid gardenId,
        [FromBody] UpdateGardenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _gardenService.UpdateAsync(
            User,
            gardenId,
            request,
            cancellationToken);

        return result.Status switch
        {
            GardenOperationStatus.Succeeded =>
                Ok(result.Garden),

            GardenOperationStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin cập nhật không hợp lệ"
                }),

            GardenOperationStatus.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu vườn đã thay đổi",
                    Detail =
                        "Vui lòng tải lại thông tin vườn trước khi cập nhật."
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn"
            })
        };
    }
}
    