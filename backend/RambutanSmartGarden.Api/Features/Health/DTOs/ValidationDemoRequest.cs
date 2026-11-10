
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Health.DTOs;

public sealed class ValidationDemoRequest
{
    [Required(ErrorMessage = "Mã cây không được để trống.")]
    [StringLength(
        20,
        MinimumLength = 3,
        ErrorMessage = "Mã cây phải có từ 3 đến 20 ký tự.")]
    public string TreeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên cây không được để trống.")]
    [StringLength(
        100,
        ErrorMessage = "Tên cây không được vượt quá 100 ký tự.")]
    public string TreeName { get; set; } = string.Empty;

    [Range(
        0,
        100,
        ErrorMessage = "Tuổi cây phải nằm trong khoảng 0 đến 100.")]
    public int Age { get; set; }
}
