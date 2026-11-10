
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.Areas.DTOs;

public sealed class CreateGardenAreaRequest
{
    [Required(ErrorMessage = "Tên khu vực là bắt buộc.")]
    [StringLength(150, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal? AreaSquareMeters { get; set; }
}
