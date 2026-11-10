
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories;

public sealed class GardenRepository : IGardenRepository
{
    private readonly AppDbContext _dbContext;

    public GardenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddGarden(Garden garden)
    {
        _dbContext.Gardens.Add(garden);
    }

    public void AddMembership(GardenMembership membership)
    {
        _dbContext.GardenMemberships.Add(membership);
    }

    public void AddAuditLog(AuditLog auditLog)
    {
        _dbContext.AuditLogs.Add(auditLog);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
