
namespace RambutanSmartGarden.Api.Features.Gardens.Areas.Models;

public sealed class GardenArea
{
    public Guid Id { get; set; }

    public Guid GardenId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal? AreaSquareMeters { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
