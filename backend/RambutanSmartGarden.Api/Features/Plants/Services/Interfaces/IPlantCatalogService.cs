
using RambutanSmartGarden.Api.Features.Plants.DTOs;

namespace RambutanSmartGarden.Api.Features.Plants.Services.Interfaces;

public enum PlantCatalogStatus
{
    Succeeded,
    NotFound,
    InvalidRequest,
    DuplicateName
}

public sealed record CatalogResult<T>(
    PlantCatalogStatus Status,
    T? Data = default
);

public interface IPlantCatalogService
{
    Task<List<PlantSpeciesResponse>> GetSpeciesAsync(
        CancellationToken cancellationToken);

    Task<PlantSpeciesResponse?> FindSpeciesAsync(
        Guid speciesId,
        CancellationToken cancellationToken);

    Task<CatalogResult<PlantSpeciesResponse>> CreateSpeciesAsync(
        Guid actorUserId,
        CreatePlantSpeciesRequest request,
        CancellationToken cancellationToken);

    Task<CatalogResult<List<TreeVarietyResponse>>> GetVarietiesAsync(
        Guid speciesId,
        CancellationToken cancellationToken);

    Task<CatalogResult<TreeVarietyResponse>> CreateVarietyAsync(
        Guid actorUserId,
        Guid speciesId,
        CreateTreeVarietyRequest request,
        CancellationToken cancellationToken);
}
