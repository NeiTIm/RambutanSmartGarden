
namespace RambutanSmartGarden.Api.Features.Plants.DTOs;

public sealed record TreeVarietyResponse(
    Guid Id,
    Guid PlantSpeciesId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    string RowVersion
);
