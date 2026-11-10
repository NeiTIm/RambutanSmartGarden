
namespace RambutanSmartGarden.Api.Features.Trees.DTOs;

public sealed record TreeResponse(
    Guid Id,
    Guid GardenId,
    Guid PlantSpeciesId,
    Guid? TreeVarietyId,
    Guid? PlantingSpotId,
    string Code,
    string? Name,
    DateOnly? PlantedAt,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion
);
