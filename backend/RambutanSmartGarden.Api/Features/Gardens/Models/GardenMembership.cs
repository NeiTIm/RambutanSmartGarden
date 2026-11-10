
namespace RambutanSmartGarden.Api.Features.Gardens.Models;

public sealed class GardenMembership
{
    public Guid Id { get; set; }

    public Guid GardenId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = "Viewer";

    public DateTime JoinedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
