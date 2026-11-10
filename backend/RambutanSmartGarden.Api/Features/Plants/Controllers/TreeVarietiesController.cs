
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Plants.DTOs;
using RambutanSmartGarden.Api.Features.Plants.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Plants.Controllers;

[ApiController]
[Authorize]
[Route("api/plant-species/{speciesId:guid}/varieties")]
public sealed class TreeVarietiesController : ControllerBase
{
    private readonly IPlantCatalogService _service;

    public TreeVarietiesController(
        IPlantCatalogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromRoute] Guid speciesId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetVarietiesAsync(
            speciesId,
            cancellationToken);

        if (result.Status == PlantCatalogStatus.NotFound)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Không tìm thấy loài cây"
            });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "SystemAdmin")]
    public async Task<IActionResult> Create(
        [FromRoute] Guid speciesId,
        [FromBody] CreateTreeVarietyRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirst("sub")?.Value,
                out var actorUserId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateVarietyAsync(
            actorUserId,
            speciesId,
            request,
            cancellationToken);

        return result.Status switch
        {
            PlantCatalogStatus.Succeeded =>
                StatusCode(
                    StatusCodes.Status201Created,
                    result.Data),

            PlantCatalogStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin giống cây không hợp lệ"
                }),

            PlantCatalogStatus.NotFound =>
                NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Không tìm thấy loài cây"
                }),

            PlantCatalogStatus.DuplicateName =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Tên giống đã tồn tại trong loài cây này"
                }),

            _ => StatusCode(500)
        };
    }
}
