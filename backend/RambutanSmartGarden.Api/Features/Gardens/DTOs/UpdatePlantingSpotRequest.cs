
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed class UpdatePlantingSpotRequest
{
    [Required]
    [StringLength(30)]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$")]
    public string Code { get; set; } = string.Empty;

    public Guid? GardenAreaId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "-90", "90")]
    public decimal? Latitude { get; set; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal? Longitude { get; set; }

    [Required]
    public string RowVersion { get; set; } = string.Empty;
}
