

namespace RambutanSmartGarden.Api.Features.Gardens.Models;

public sealed class PlantingSpot
{
    public Guid Id { get; set; }

    public Guid GardenId { get; set; }

    public Guid? GardenAreaId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
