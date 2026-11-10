
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Plants.DTOs;
using RambutanSmartGarden.Api.Features.Plants.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Plants.Controllers;

[ApiController]
[Authorize]
[Route("api/plant-species")]
public sealed class PlantSpeciesController : ControllerBase
{
    private readonly IPlantCatalogService _service;

    public PlantSpeciesController(IPlantCatalogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlantSpeciesResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var species = await _service.GetSpeciesAsync(
            cancellationToken);

        return Ok(species);
    }

    [HttpGet("{speciesId:guid}")]
    public async Task<ActionResult<PlantSpeciesResponse>> GetById(
        Guid speciesId,
        CancellationToken cancellationToken)
    {
        var species = await _service.FindSpeciesAsync(
            speciesId,
            cancellationToken);

        return species is null
            ? NotFound()
            : Ok(species);
    }

    [HttpPost]
    [Authorize(Policy = "SystemAdmin")]
    public async Task<ActionResult<PlantSpeciesResponse>> Create(
        [FromBody] CreatePlantSpeciesRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirst("sub")?.Value,
                out var actorUserId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateSpeciesAsync(
            actorUserId,
            request,
            cancellationToken);

        return result.Status switch
        {
            PlantCatalogStatus.Succeeded =>
                CreatedAtAction(
                    nameof(GetById),
                    new { speciesId = result.Data!.Id },
                    result.Data),

            PlantCatalogStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin loài cây không hợp lệ"
                }),

            PlantCatalogStatus.DuplicateName =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Tên loài cây đã tồn tại"
                }),

            _ => StatusCode(500)
        };
    }
}
