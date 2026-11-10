
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;

public interface IPlantingSpotRepository
{
    Task<List<PlantingSpot>> GetByGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<PlantingSpot?> FindAsync(
        Guid gardenId,
        Guid spotId,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<bool> GardenAreaBelongsToGardenAsync(
        Guid gardenId,
        Guid gardenAreaId,
        CancellationToken cancellationToken);

    void Add(PlantingSpot spot);

    void AddAuditLog(AuditLog auditLog);

    void SetOriginalRowVersion(
        PlantingSpot spot,
        byte[] originalRowVersion);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
