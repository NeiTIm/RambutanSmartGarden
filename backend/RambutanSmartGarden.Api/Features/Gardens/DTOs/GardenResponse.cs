
namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed record GardenResponse(
    Guid Id,
    string Name,
    string? Description,
    string? LocationDescription,
    decimal? AreaSquareMeters,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
