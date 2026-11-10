
using System.ComponentModel.DataAnnotations;

namespace RambutanSmartGarden.Api.Features.Plants.DTOs;

public sealed class CreatePlantSpeciesRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? ScientificName { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }
}
