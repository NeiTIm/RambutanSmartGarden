
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Trees.Models;
using RambutanSmartGarden.Api.Features.Trees.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Trees.Repositories;

public sealed class TreeRepository : ITreeRepository
{
    private readonly AppDbContext _dbContext;

    public TreeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Tree>> GetByGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Trees
            .AsNoTracking()
            .Where(x => x.GardenId == gardenId)
            .OrderBy(x => x.Code)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Tree?> FindAsync(
        Guid gardenId,
        Guid treeId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        IQueryable<Tree> query = _dbContext.Trees;

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.GardenId == gardenId &&
                 x.Id == treeId,
            cancellationToken);
    }

    public Task<bool> SpeciesExistsAsync(
        Guid speciesId,
        CancellationToken cancellationToken)
    {
        return _dbContext.PlantSpecies
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == speciesId,
                cancellationToken);
    }

    public Task<bool> VarietyBelongsToSpeciesAsync(
        Guid speciesId,
        Guid varietyId,
        CancellationToken cancellationToken)
    {
        return _dbContext.TreeVarieties
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == varietyId &&
                     x.PlantSpeciesId == speciesId,
                cancellationToken);
    }

    public Task<bool> SpotBelongsToGardenAsync(
        Guid gardenId,
        Guid spotId,
        CancellationToken cancellationToken)
    {
        return _dbContext.PlantingSpots
            .AsNoTracking()
            .AnyAsync(
                x => x.GardenId == gardenId &&
                     x.Id == spotId,
                cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        Guid gardenId,
        string code,
        Guid? excludeTreeId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Trees
            .AsNoTracking()
            .AnyAsync(
                x => x.GardenId == gardenId &&
                     x.Code == code &&
                     (!excludeTreeId.HasValue ||
                      x.Id != excludeTreeId.Value),
                cancellationToken);
    }

    public Task<bool> SpotOccupiedAsync(
        Guid spotId,
        Guid? excludeTreeId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Trees
            .AsNoTracking()
            .AnyAsync(
                x => x.PlantingSpotId == spotId &&
                     x.Status == "Active" &&
                     (!excludeTreeId.HasValue ||
                      x.Id != excludeTreeId.Value),
                cancellationToken);
    }

    public void Add(Tree tree)
    {
        _dbContext.Trees.Add(tree);
    }

    public void AddAuditLog(AuditLog auditLog)
    {
        _dbContext.AuditLogs.Add(auditLog);
    }

    public void SetOriginalRowVersion(
        Tree tree,
        byte[] originalRowVersion)
    {
        _dbContext.Entry(tree)
            .Property(x => x.RowVersion)
            .OriginalValue = originalRowVersion;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
