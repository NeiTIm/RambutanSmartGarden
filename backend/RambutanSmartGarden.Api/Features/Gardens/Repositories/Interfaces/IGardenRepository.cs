
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;

public interface IGardenRepository
{
    // Phase 03.3.1 - Create.
    void AddGarden(Garden garden);

    void AddMembership(GardenMembership membership);

    void AddAuditLog(AuditLog auditLog);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);

    // Phase 03.3.2 - List.
    Task<bool?> GetSystemAdminStatusAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<List<GardenListItemResponse>>
        GetAccessibleGardensAsync(
            Guid userId,
            bool isSystemAdmin,
            CancellationToken cancellationToken);

    // Phase 03.3.3 - Detail and Update.
    Task<Garden?> FindGardenAsync(
        Guid gardenId,
        bool trackChanges,
        CancellationToken cancellationToken);

    void SetOriginalRowVersion(
        Garden garden,
        byte[] originalRowVersion);
}
