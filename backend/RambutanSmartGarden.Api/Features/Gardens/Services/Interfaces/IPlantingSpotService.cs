
using System.Security.Claims;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;

namespace RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

public enum PlantingSpotStatus
{
    Succeeded,
    NotFound,
    InvalidRequest,
    InvalidGardenArea,
    DuplicateCode,
    ConcurrencyConflict
}

public sealed record PlantingSpotResult(
    PlantingSpotStatus Status,
    PlantingSpotResponse? Spot = null
);

public sealed record PlantingSpotListResult(
    PlantingSpotStatus Status,
    List<PlantingSpotResponse>? Spots = null
);

public interface IPlantingSpotService
{
    Task<PlantingSpotListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<PlantingSpotResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid spotId,
        CancellationToken cancellationToken);

    Task<PlantingSpotResult> CreateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CreatePlantingSpotRequest request,
        CancellationToken cancellationToken);

    Task<PlantingSpotResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid spotId,
        UpdatePlantingSpotRequest request,
        CancellationToken cancellationToken);
}
