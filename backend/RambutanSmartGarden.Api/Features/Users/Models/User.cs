
namespace RambutanSmartGarden.Api.Features.Users.Models;

public sealed class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string NormalizedUsername { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? NormalizedPhoneNumber { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsSystemAdmin { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
