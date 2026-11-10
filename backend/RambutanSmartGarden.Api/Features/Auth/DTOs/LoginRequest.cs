
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Auth.DTOs;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Tên đăng nhập hoặc số điện thoại là bắt buộc.")]
    [StringLength(50)]
    public string Identifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    public string Password { get; set; } = string.Empty;
}
