
namespace RambutanSmartGarden.Api.Features.Plants.Models;

public sealed class TreeVariety
{
    public Guid Id { get; set; }

    public Guid PlantSpeciesId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
