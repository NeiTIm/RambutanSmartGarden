
namespace RambutanSmartGarden.Api.Features.Plants.Models;

public sealed class PlantSpecies
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ScientificName { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
