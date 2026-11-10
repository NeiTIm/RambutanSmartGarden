
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Plants.Models;
using RambutanSmartGarden.Api.Features.Plants.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Plants.Repositories;

public sealed class PlantCatalogRepository : IPlantCatalogRepository
{
    private readonly AppDbContext _dbContext;

    public PlantCatalogRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<PlantSpecies>> GetSpeciesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.PlantSpecies
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<PlantSpecies?> FindSpeciesAsync(
        Guid speciesId,
        CancellationToken cancellationToken)
    {
        return _dbContext.PlantSpecies
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == speciesId,
                cancellationToken);
    }

    public Task<List<TreeVariety>> GetVarietiesAsync(
        Guid speciesId,
        CancellationToken cancellationToken)
    {
        return _dbContext.TreeVarieties
            .AsNoTracking()
            .Where(x => x.PlantSpeciesId == speciesId)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public void AddSpecies(PlantSpecies species)
    {
        _dbContext.PlantSpecies.Add(species);
    }

    public void AddVariety(TreeVariety variety)
    {
        _dbContext.TreeVarieties.Add(variety);
    }

    public void AddAuditLog(AuditLog auditLog)
    {
        _dbContext.AuditLogs.Add(auditLog);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
