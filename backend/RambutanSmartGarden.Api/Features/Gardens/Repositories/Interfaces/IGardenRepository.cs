
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;

public interface IGardenRepository
{
    void AddGarden(Garden garden);

    void AddMembership(GardenMembership membership);

    void AddAuditLog(AuditLog auditLog);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
