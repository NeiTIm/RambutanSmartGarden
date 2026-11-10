
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Gardens.Areas.Repositories;

public sealed class GardenAreaRepository
    : IGardenAreaRepository
{
    private readonly AppDbContext _dbContext;

    public GardenAreaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<GardenArea>> GetByGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        return _dbContext.GardenAreas
            .AsNoTracking()
            .Where(area => area.GardenId == gardenId)
            .OrderBy(area => area.Name)
            .ThenBy(area => area.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<GardenArea?> FindByIdAsync(
        Guid gardenId,
        Guid areaId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        IQueryable<GardenArea> query = _dbContext.GardenAreas;

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            area => area.Id == areaId &&
                    area.GardenId == gardenId,
            cancellationToken);
    }

    public void Add(GardenArea area)
    {
        _dbContext.GardenAreas.Add(area);
    }

    public void AddAuditLog(AuditLog auditLog)
    {
        _dbContext.AuditLogs.Add(auditLog);
    }

    public void SetOriginalRowVersion(
        GardenArea area,
        byte[] rowVersion)
    {
        _dbContext.Entry(area)
            .Property(entity => entity.RowVersion)
            .OriginalValue = rowVersion;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
