
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Gardens.Areas.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Areas.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/areas")]
public sealed class GardenAreasController : ControllerBase
{
    private readonly IGardenAreaService _areaService;

    public GardenAreasController(IGardenAreaService areaService)
    {
        _areaService = areaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GardenAreaResponse>>> GetAll(
        [FromRoute] Guid gardenId,
        CancellationToken cancellationToken)
    {
        var result = await _areaService.GetAllAsync(
            User,
            gardenId,
            cancellationToken);

        if (result.Status != GardenAreaStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn hoặc không có quyền truy cập"
            });
        }

        return Ok(result.Areas);
    }

    [HttpGet("{areaId:guid}")]
    public async Task<ActionResult<GardenAreaResponse>> GetById(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid areaId,
        CancellationToken cancellationToken)
    {
        var result = await _areaService.GetByIdAsync(
            User,
            gardenId,
            areaId,
            cancellationToken);

        if (result.Status != GardenAreaStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy khu vực"
            });
        }

        return Ok(result.Area);
    }

    [HttpPost]
    public async Task<ActionResult<GardenAreaResponse>> Create(
        [FromRoute] Guid gardenId,
        [FromBody] CreateGardenAreaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _areaService.CreateAsync(
            User,
            gardenId,
            request,
            cancellationToken);

        return result.Status switch
        {
            GardenAreaStatus.Succeeded =>
                CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        gardenId,
                        areaId = result.Area!.Id
                    },
                    result.Area),

            GardenAreaStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin khu vực không hợp lệ"
                }),

            GardenAreaStatus.DuplicateName =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Tên khu vực đã tồn tại trong vườn"
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn hoặc không có quyền"
            })
        };
    }

    [HttpPut("{areaId:guid}")]
    public async Task<ActionResult<GardenAreaResponse>> Update(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid areaId,
        [FromBody] UpdateGardenAreaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _areaService.UpdateAsync(
            User,
            gardenId,
            areaId,
            request,
            cancellationToken);

        return result.Status switch
        {
            GardenAreaStatus.Succeeded =>
                Ok(result.Area),

            GardenAreaStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin cập nhật không hợp lệ"
                }),

            GardenAreaStatus.DuplicateName =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Tên khu vực đã tồn tại trong vườn"
                }),

            GardenAreaStatus.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu khu vực đã được thay đổi",
                    Detail = "Vui lòng tải lại khu vực trước khi cập nhật."
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy khu vực hoặc không có quyền"
            })
        };
    }
}
