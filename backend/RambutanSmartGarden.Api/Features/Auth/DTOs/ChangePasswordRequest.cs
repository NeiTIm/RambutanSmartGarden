
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Auth.DTOs;

public sealed class ChangePasswordRequest
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
    [StringLength(
        128,
        MinimumLength = 12,
        ErrorMessage = "Mật khẩu mới phải từ 12 đến 128 ký tự.")]
    public string NewPassword { get; set; } = string.Empty;
}
