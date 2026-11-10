
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Trees.DTOs;

public sealed class ReplaceTreeRequest
{
    [Required]
    [StringLength(30)]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$")]
    public string NewTreeCode { get; set; } = string.Empty;

    public Guid NewPlantSpeciesId { get; set; }

    public Guid? NewTreeVarietyId { get; set; }

    [StringLength(150)]
    public string? NewTreeName { get; set; }

    public DateOnly? NewTreePlantedAt { get; set; }

    [StringLength(2000)]
    public string? NewTreeNotes { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public string OldTreeRowVersion { get; set; } = string.Empty;
}
