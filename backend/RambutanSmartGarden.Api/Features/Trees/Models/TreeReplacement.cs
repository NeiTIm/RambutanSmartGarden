
namespace RambutanSmartGarden.Api.Features.Trees.Models;

public sealed class TreeReplacement
{
    public Guid Id { get; set; }

    public Guid GardenId { get; set; }

    public Guid OldTreeId { get; set; }

    public Guid NewTreeId { get; set; }

    public Guid PlantingSpotId { get; set; }

    public string OldTreePreviousStatus { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public DateTime ReplacedAt { get; set; }

    public Guid PerformedByUserId { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
