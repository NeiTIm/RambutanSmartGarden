
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
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

    // Phase 03.3.1 - Create.

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

    // Phase 03.3.2 - List.

    public Task<bool?> GetSystemAdminStatusAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                user.Id == userId &&
                user.IsActive)
            .Select(user => (bool?)user.IsSystemAdmin)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<GardenListItemResponse>>
        GetAccessibleGardensAsync(
            Guid userId,
            bool isSystemAdmin,
            CancellationToken cancellationToken)
    {
        var gardens = _dbContext.Gardens
            .AsNoTracking()
            .Where(garden =>
                isSystemAdmin ||
                _dbContext.GardenMemberships.Any(
                    membership =>
                        membership.GardenId == garden.Id &&
                        membership.UserId == userId));

        return gardens
            .OrderBy(garden => garden.Name)
            .ThenBy(garden => garden.Id)
            .Select(garden => new GardenListItemResponse(
                garden.Id,
                garden.Name,
                garden.Description,
                garden.LocationDescription,
                garden.AreaSquareMeters,
                garden.CreatedAt,
                garden.UpdatedAt,
                isSystemAdmin
                    ? "SystemAdmin"
                    : _dbContext.GardenMemberships
                        .Where(membership =>
                            membership.GardenId == garden.Id &&
                            membership.UserId == userId)
                        .Select(membership => membership.Role)
                        .FirstOrDefault()!
            ))
            .ToListAsync(cancellationToken);
    }

    // Phase 03.3.3 - Detail and Update.

    public Task<Garden?> FindGardenAsync(
        Guid gardenId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        IQueryable<Garden> query = _dbContext.Gardens;

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            garden => garden.Id == gardenId,
            cancellationToken);
    }

    public void SetOriginalRowVersion(
        Garden garden,
        byte[] originalRowVersion)
    {
        _dbContext.Entry(garden)
            .Property(entity => entity.RowVersion)
            .OriginalValue = originalRowVersion;
    }
}
