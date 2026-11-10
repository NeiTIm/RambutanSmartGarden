
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Plants.DTOs;

public sealed class CreateTreeVarietyRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }
}
