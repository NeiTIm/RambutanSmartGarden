
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Trees.DTOs;

public sealed class CreateTreeRequest
{
    [Required]
    [StringLength(30)]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$")]
    public string Code { get; set; } = string.Empty;

    [Required]
    public Guid PlantSpeciesId { get; set; }

    public Guid? TreeVarietyId { get; set; }

    public Guid? PlantingSpotId { get; set; }

    [StringLength(150)]
    public string? Name { get; set; }

    public DateOnly? PlantedAt { get; set; }

    [Required]
    [RegularExpression("^(Active|Dead|Removed)$")]
    public string Status { get; set; } = "Active";

    [StringLength(2000)]
    public string? Notes { get; set; }
}
