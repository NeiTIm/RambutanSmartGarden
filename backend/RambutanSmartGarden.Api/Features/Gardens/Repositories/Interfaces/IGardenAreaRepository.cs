
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Areas.Repositories.Interfaces;

public interface IGardenAreaRepository
{
    Task<List<GardenArea>> GetByGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<GardenArea?> FindByIdAsync(
        Guid gardenId,
        Guid areaId,
        bool trackChanges,
        CancellationToken cancellationToken);

    void Add(GardenArea area);

    void AddAuditLog(AuditLog auditLog);

    void SetOriginalRowVersion(
        GardenArea area,
        byte[] rowVersion);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
