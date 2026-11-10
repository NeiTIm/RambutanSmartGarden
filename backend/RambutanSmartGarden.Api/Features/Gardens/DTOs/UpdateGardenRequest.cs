
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed class UpdateGardenRequest
{
    [Required(ErrorMessage = "Tên vườn là bắt buộc.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? LocationDescription { get; set; }

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99")]
    public decimal? AreaSquareMeters { get; set; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; set; } = string.Empty;
}
