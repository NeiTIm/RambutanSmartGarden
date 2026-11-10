
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Trees.DTOs;
using RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Trees.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/tree-replacements")]
public sealed class TreeReplacementsController : ControllerBase
{
    private readonly ITreeReplacementService _service;

    public TreeReplacementsController(
        ITreeReplacementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromRoute] Guid gardenId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(
            User,
            gardenId,
            cancellationToken);

        return result.Status == ReplaceTreeStatus.Succeeded
            ? Ok(result.Replacements)
            : NotFound();
    }

    [HttpGet("{replacementId:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid replacementId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            User,
            gardenId,
            replacementId,
            cancellationToken);

        return result.Status == ReplaceTreeStatus.Succeeded
            ? Ok(result.Replacement)
            : NotFound();
    }

    [HttpPost("replace/{oldTreeId:guid}")]
    public async Task<IActionResult> Replace(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid oldTreeId,
        [FromBody] ReplaceTreeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ReplaceAsync(
            User,
            gardenId,
            oldTreeId,
            request,
            cancellationToken);

        return result.Status switch
        {
            ReplaceTreeStatus.Succeeded =>
                CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        gardenId,
                        replacementId = result.Replacement!.Id
                    },
                    result.Replacement),

            ReplaceTreeStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin thay cây không hợp lệ"
                }),

            ReplaceTreeStatus.InvalidSpecies =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Loài cây mới không tồn tại"
                }),

            ReplaceTreeStatus.InvalidVariety =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Giống cây mới không thuộc loài đã chọn"
                }),

            ReplaceTreeStatus.MissingPlantingSpot =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Cây cũ chưa có vị trí trồng",
                    Detail = "Cần xác định vị trí cây trước khi thay."
                }),

            ReplaceTreeStatus.OccupiedPlantingSpot =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Vị trí đã có cây đang hoạt động"
                }),

            ReplaceTreeStatus.AlreadyReplaced =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Cây đã được thay thế hoặc loại bỏ"
                }),

            ReplaceTreeStatus.DuplicateCode =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Mã cây mới đã tồn tại"
                }),

            ReplaceTreeStatus.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu cây đã thay đổi",
                    Detail = "Vui lòng tải lại hồ sơ cây trước khi thay."
                }),

            ReplaceTreeStatus.DataConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu thay cây xung đột"
                }),

            _ => NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy cây hoặc không có quyền"
            })
        };
    }
}
