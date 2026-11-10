
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
    Task<CreateGardenResult> CreateAsync(
        CreateGardenRequest request,
        Guid creatorUserId,
        CancellationToken cancellationToken);
}
