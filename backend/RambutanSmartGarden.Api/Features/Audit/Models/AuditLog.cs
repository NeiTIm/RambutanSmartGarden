
namespace RambutanSmartGarden.Api.Features.Audit.Models;

public sealed class AuditLog
{
    public Guid Id { get; set; }

    public Guid? ActorUserId { get; set; }

    public Guid? GardenId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? Description { get; set; }

    public string? IpAddress { get; set; }

    public string? CorrelationId { get; set; }

    public DateTime OccurredAt { get; set; }
}
