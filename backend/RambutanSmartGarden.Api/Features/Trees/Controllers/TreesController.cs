
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Trees.DTOs;
using RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Trees.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/trees")]
public sealed class TreesController : ControllerBase
{
    private readonly ITreeService _treeService;

    public TreesController(ITreeService treeService)
    {
        _treeService = treeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TreeResponse>>> GetAll(
        [FromRoute] Guid gardenId,
        CancellationToken cancellationToken)
    {
        var result = await _treeService.GetAllAsync(
            User,
            gardenId,
            cancellationToken);

        if (result.Status != TreeOperationStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn hoặc không có quyền truy cập"
            });
        }

        return Ok(result.Trees);
    }

    [HttpGet("{treeId:guid}")]
    public async Task<ActionResult<TreeResponse>> GetById(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid treeId,
        CancellationToken cancellationToken)
    {
        var result = await _treeService.GetByIdAsync(
            User,
            gardenId,
            treeId,
            cancellationToken);

        if (result.Status != TreeOperationStatus.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy cây"
            });
        }

        return Ok(result.Tree);
    }

    [HttpPost]
    public async Task<ActionResult<TreeResponse>> Create(
        [FromRoute] Guid gardenId,
        [FromBody] CreateTreeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _treeService.CreateAsync(
            User,
            gardenId,
            request,
            cancellationToken);

        return result.Status switch
        {
            TreeOperationStatus.Succeeded =>
                CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        gardenId,
                        treeId = result.Tree!.Id
                    },
                    result.Tree),

            TreeOperationStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin cây không hợp lệ"
                }),

            TreeOperationStatus.InvalidSpecies =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Loài cây không tồn tại"
                }),

            TreeOperationStatus.InvalidVariety =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Giống cây không thuộc loài đã chọn"
                }),

            TreeOperationStatus.InvalidPlantingSpot =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Vị trí trồng không thuộc vườn"
                }),

            TreeOperationStatus.DuplicateCode =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Mã cây đã tồn tại trong vườn"
                }),

            TreeOperationStatus.OccupiedPlantingSpot =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Vị trí đã có cây đang hoạt động"
                }),

            TreeOperationStatus.DataConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu cây xung đột với dữ liệu hiện có"
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy vườn hoặc không có quyền"
            })
        };
    }

    [HttpPut("{treeId:guid}")]
    public async Task<ActionResult<TreeResponse>> Update(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid treeId,
        [FromBody] UpdateTreeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _treeService.UpdateAsync(
            User,
            gardenId,
            treeId,
            request,
            cancellationToken);

        return result.Status switch
        {
            TreeOperationStatus.Succeeded =>
                Ok(result.Tree),

            TreeOperationStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin cập nhật không hợp lệ"
                }),

            TreeOperationStatus.InvalidSpecies =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Loài cây không tồn tại"
                }),

            TreeOperationStatus.InvalidVariety =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Giống cây không thuộc loài đã chọn"
                }),

            TreeOperationStatus.InvalidPlantingSpot =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Vị trí trồng không thuộc vườn"
                }),

            TreeOperationStatus.DuplicateCode =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Mã cây đã tồn tại"
                }),

            TreeOperationStatus.OccupiedPlantingSpot =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Vị trí đã có cây đang hoạt động"
                }),

            TreeOperationStatus.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Hồ sơ cây đã thay đổi",
                    Detail = "Vui lòng tải lại dữ liệu trước khi cập nhật."
                }),

            TreeOperationStatus.DataConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu cây xung đột với dữ liệu hiện có"
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy cây hoặc không có quyền"
            })
        };
    }
}
