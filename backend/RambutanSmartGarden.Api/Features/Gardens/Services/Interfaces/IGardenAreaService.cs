
using System.Security.Claims;
using RambutanSmartGarden.Api.Features.Gardens.Areas.DTOs;

namespace RambutanSmartGarden.Api.Features.Gardens.Areas.Services.Interfaces;

public enum GardenAreaStatus
{
    Succeeded,
    NotFound,
    InvalidRequest,
    DuplicateName,
    ConcurrencyConflict
}

public sealed record GardenAreaResult(
    GardenAreaStatus Status,
    GardenAreaResponse? Area = null
);

public sealed record GardenAreaListResult(
    GardenAreaStatus Status,
    List<GardenAreaResponse>? Areas = null
);

public interface IGardenAreaService
{
    Task<GardenAreaListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<GardenAreaResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid areaId,
        CancellationToken cancellationToken);

    Task<GardenAreaResult> CreateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CreateGardenAreaRequest request,
        CancellationToken cancellationToken);

    Task<GardenAreaResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid areaId,
        UpdateGardenAreaRequest request,
        CancellationToken cancellationToken);
}
