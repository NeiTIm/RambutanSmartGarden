
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed class AddGardenMemberRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [RegularExpression(
        "^(Member|Viewer)$",
        ErrorMessage = "Vai trò phải là Member hoặc Viewer.")]
    public string Role { get; set; } = "Viewer";
}
