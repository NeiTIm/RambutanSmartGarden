
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed class CreateGardenRequest
{
    [Required(ErrorMessage = "Tên vườn là bắt buộc.")]
    [StringLength(
        150,
        ErrorMessage = "Tên vườn không được vượt quá 150 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? LocationDescription { get; set; }

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99",
        ErrorMessage = "Diện tích phải lớn hơn 0 và nằm trong giới hạn cho phép.")]
    public decimal? AreaSquareMeters { get; set; }
}
