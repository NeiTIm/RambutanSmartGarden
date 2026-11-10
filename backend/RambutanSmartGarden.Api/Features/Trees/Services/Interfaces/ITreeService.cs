
using System.Security.Claims;
using RambutanSmartGarden.Api.Features.Trees.DTOs;

namespace RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;

public enum TreeOperationStatus
{
    Succeeded,
    NotFound,
    InvalidRequest,
    InvalidSpecies,
    InvalidVariety,
    InvalidPlantingSpot,
    DuplicateCode,
    OccupiedPlantingSpot,
    ConcurrencyConflict,
    DataConflict
}

public sealed record TreeResult(
    TreeOperationStatus Status,
    TreeResponse? Tree = null
);

public sealed record TreeListResult(
    TreeOperationStatus Status,
    List<TreeResponse>? Trees = null
);

public interface ITreeService
{
    Task<TreeListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<TreeResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid treeId,
        CancellationToken cancellationToken);

    Task<TreeResult> CreateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CreateTreeRequest request,
        CancellationToken cancellationToken);

    Task<TreeResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid treeId,
        UpdateTreeRequest request,
        CancellationToken cancellationToken);
}
