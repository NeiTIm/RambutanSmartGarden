
namespace RambutanSmartGarden.Api.Features.Plants.DTOs;

public sealed record PlantSpeciesResponse(
    Guid Id,
    string Name,
    string? ScientificName,
    string? Description,
    DateTime CreatedAt,
    string RowVersion
);
