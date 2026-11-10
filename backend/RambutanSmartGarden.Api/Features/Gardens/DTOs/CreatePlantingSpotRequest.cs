
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed class CreatePlantingSpotRequest
{
    [Required]
    [StringLength(30)]
    [RegularExpression(
        @"^[a-zA-Z0-9_-]+$",
        ErrorMessage = "Mã vị trí chỉ gồm chữ, số, dấu gạch ngang hoặc gạch dưới.")]
    public string Code { get; set; } = string.Empty;

    public Guid? GardenAreaId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "-90", "90")]
    public decimal? Latitude { get; set; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal? Longitude { get; set; }
}
