
namespace RambutanSmartGarden.Api.Features.Gardens.Models;

public sealed class Garden
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? LocationDescription { get; set; }

    public decimal? AreaSquareMeters { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
