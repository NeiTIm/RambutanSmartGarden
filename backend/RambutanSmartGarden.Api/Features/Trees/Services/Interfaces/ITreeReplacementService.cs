
using System.Security.Claims;
using RambutanSmartGarden.Api.Features.Trees.DTOs;

namespace RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;

public enum ReplaceTreeStatus
{
    Succeeded,
    NotFound,
    InvalidRequest,
    InvalidSpecies,
    InvalidVariety,
    MissingPlantingSpot,
    OccupiedPlantingSpot,
    AlreadyReplaced,
    DuplicateCode,
    ConcurrencyConflict,
    DataConflict
}

public sealed record ReplaceTreeResult(
    ReplaceTreeStatus Status,
    TreeReplacementResponse? Replacement = null
);

public sealed record TreeReplacementListResult(
    ReplaceTreeStatus Status,
    List<TreeReplacementResponse>? Replacements = null
);

public interface ITreeReplacementService
{
    Task<TreeReplacementListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<ReplaceTreeResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid replacementId,
        CancellationToken cancellationToken);

    Task<ReplaceTreeResult> ReplaceAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid oldTreeId,
        ReplaceTreeRequest request,
        CancellationToken cancellationToken);
}
