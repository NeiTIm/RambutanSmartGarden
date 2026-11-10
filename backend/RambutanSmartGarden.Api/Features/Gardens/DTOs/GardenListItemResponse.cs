
namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed record GardenListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    string? LocationDescription,
    decimal? AreaSquareMeters,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string AccessRole
);
