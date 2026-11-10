
namespace RambutanSmartGarden.Api.Features.Gardens.Areas.DTOs;

public sealed record GardenAreaResponse(
    Guid Id,
    Guid GardenId,
    string Name,
    string? Description,
    decimal? AreaSquareMeters,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion
);
