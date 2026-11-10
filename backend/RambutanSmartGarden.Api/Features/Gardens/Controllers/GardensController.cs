
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

    [HttpPost]
    [Authorize(Policy = "SystemAdmin")]
    [ProducesResponseType(
        typeof(GardenResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
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
                Status = StatusCodes.Status400BadRequest,
                Title = "Thông tin vườn không hợp lệ"
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Garden);
    }
}
