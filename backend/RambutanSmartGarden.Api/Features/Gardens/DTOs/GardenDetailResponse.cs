
namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed record GardenDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string? LocationDescription,
    decimal? AreaSquareMeters,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion
);
