
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed class UpdateGardenMemberRoleRequest
{
    [Required(ErrorMessage = "Vai trò là bắt buộc.")]
    [RegularExpression(
        "^(Admin|Member|Viewer)$",
        ErrorMessage = "Vai trò phải là Admin, Member hoặc Viewer.")]
    public string Role { get; set; } = string.Empty;
}
