
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories;

public sealed class PlantingSpotRepository : IPlantingSpotRepository
{
    private readonly AppDbContext _dbContext;

    public PlantingSpotRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<PlantingSpot>> GetByGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        return _dbContext.PlantingSpots
            .AsNoTracking()
            .Where(x => x.GardenId == gardenId)
            .OrderBy(x => x.Code)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<PlantingSpot?> FindAsync(
        Guid gardenId,
        Guid spotId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        IQueryable<PlantingSpot> query =
            _dbContext.PlantingSpots;

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.Id == spotId &&
                 x.GardenId == gardenId,
            cancellationToken);
    }

    public Task<bool> GardenAreaBelongsToGardenAsync(
        Guid gardenId,
        Guid gardenAreaId,
        CancellationToken cancellationToken)
    {
        return _dbContext.GardenAreas
            .AsNoTracking()
            .AnyAsync(
                area => area.Id == gardenAreaId &&
                        area.GardenId == gardenId,
                cancellationToken);
    }

    public void Add(PlantingSpot spot)
    {
        _dbContext.PlantingSpots.Add(spot);
    }

    public void AddAuditLog(AuditLog auditLog)
    {
        _dbContext.AuditLogs.Add(auditLog);
    }

    public void SetOriginalRowVersion(
        PlantingSpot spot,
        byte[] originalRowVersion)
    {
        _dbContext.Entry(spot)
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
