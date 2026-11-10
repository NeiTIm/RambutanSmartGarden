
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Trees.Models;

namespace RambutanSmartGarden.Api.Features.Trees.Repositories.Interfaces;

public interface ITreeRepository
{
    Task<List<Tree>> GetByGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<Tree?> FindAsync(
        Guid gardenId,
        Guid treeId,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<bool> SpeciesExistsAsync(
        Guid speciesId,
        CancellationToken cancellationToken);

    Task<bool> VarietyBelongsToSpeciesAsync(
        Guid speciesId,
        Guid varietyId,
        CancellationToken cancellationToken);

    Task<bool> SpotBelongsToGardenAsync(
        Guid gardenId,
        Guid spotId,
        CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(
        Guid gardenId,
        string code,
        Guid? excludeTreeId,
        CancellationToken cancellationToken);

    Task<bool> SpotOccupiedAsync(
        Guid spotId,
        Guid? excludeTreeId,
        CancellationToken cancellationToken);

    void Add(Tree tree);

    void AddAuditLog(AuditLog auditLog);

    void SetOriginalRowVersion(
        Tree tree,
        byte[] originalRowVersion);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
