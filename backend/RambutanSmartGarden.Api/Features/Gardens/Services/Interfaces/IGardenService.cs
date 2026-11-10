
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

public interface IGardenService
{
    // Phase 03.3.1
    Task<CreateGardenResult> CreateAsync(
        CreateGardenRequest request,
        Guid creatorUserId,
        CancellationToken cancellationToken);

    // Phase 03.3.2
    Task<List<GardenListItemResponse>?> GetAccessibleGardensAsync(
        Guid currentUserId,
        CancellationToken cancellationToken);
}
