
namespace RambutanSmartGarden.Api.Features.Trees.Models;

public sealed class Tree
{
    public Guid Id { get; set; }

    public Guid GardenId { get; set; }

    public Guid PlantSpeciesId { get; set; }

    public Guid? TreeVarietyId { get; set; }

    public Guid? PlantingSpotId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string? Name { get; set; }

    public DateOnly? PlantedAt { get; set; }

    public string Status { get; set; } = "Active";

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
