
namespace RambutanSmartGarden.Api.Features.Auth.Models;

public sealed class PasswordResetRecord
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PerformedByAdminId { get; set; }

    public DateTime ResetAt { get; set; }

    public string? Reason { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
