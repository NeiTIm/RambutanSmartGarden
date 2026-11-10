
namespace RambutanSmartGarden.Api.Features.Auth.Models;

public sealed class UserSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? RefreshTokenHash { get; set; }

    public string? DeviceName { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedReason { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
