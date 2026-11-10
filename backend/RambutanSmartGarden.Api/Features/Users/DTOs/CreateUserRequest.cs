
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Users.DTOs;

public sealed class CreateUserRequest
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(
        @"^[a-zA-Z0-9._-]+$",
        ErrorMessage = "Tên đăng nhập chứa ký tự không hợp lệ.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên hiển thị là bắt buộc.")]
    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [RegularExpression(
        @"^(0[35789]\d{8}|\+84[35789]\d{8})$",
        ErrorMessage = "Số điện thoại di động không hợp lệ.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; set; } = string.Empty;
}
