
namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed record PlantingSpotResponse(
    Guid Id,
    Guid GardenId,
    Guid? GardenAreaId,
    string Code,
    string? Description,
    decimal? Latitude,
    decimal? Longitude,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion
);
