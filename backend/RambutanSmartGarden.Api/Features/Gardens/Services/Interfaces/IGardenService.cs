
using System.Security.Claims;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;

namespace RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

public enum CreateGardenStatus
{
    Succeeded,
    InvalidRequest
}

public sealed record CreateGardenResult(
    CreateGardenStatus Status,
    GardenResponse? Garden = null
);

public enum GardenOperationStatus
{
    Succeeded,
    NotFound,
    Forbidden,
    InvalidRequest,
    ConcurrencyConflict
}

public sealed record GardenDetailResult(
    GardenOperationStatus Status,
    GardenDetailResponse? Garden = null
);

public interface IGardenService
{
    // Phase 03.3.1.
    Task<CreateGardenResult> CreateAsync(
        CreateGardenRequest request,
        Guid creatorUserId,
        CancellationToken cancellationToken);

    // Phase 03.3.2.
    Task<List<GardenListItemResponse>?>
        GetAccessibleGardensAsync(
            Guid currentUserId,
            CancellationToken cancellationToken);

    // Phase 03.3.3.
    Task<GardenDetailResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<GardenDetailResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        UpdateGardenRequest request,
        CancellationToken cancellationToken);
}
