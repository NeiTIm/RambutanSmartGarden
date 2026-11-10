
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Auth.DTOs;

public sealed class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh Token là bắt buộc.")]
    public string RefreshToken { get; set; } = string.Empty;
}
