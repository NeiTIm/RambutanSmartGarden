
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories;

public sealed class GardenMemberRepository : IGardenMemberRepository
{
    private readonly AppDbContext _dbContext;

    public GardenMemberRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<GardenMemberResponse>> GetMembersAsync(
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        return (
            from membership in _dbContext.GardenMemberships
                .AsNoTracking()
            join user in _dbContext.Users.AsNoTracking()
                on membership.UserId equals user.Id
            where membership.GardenId == gardenId
            orderby membership.JoinedAt, user.Username
            select new GardenMemberResponse(
                membership.Id,
                user.Id,
                user.Username,
                user.DisplayName,
                membership.Role,
                membership.JoinedAt)
        ).ToListAsync(cancellationToken);
    }

    public Task<User?> FindActiveUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Id == userId && user.IsActive,
                cancellationToken);
    }

    public Task<bool> MembershipExistsAsync(
        Guid gardenId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.GardenMemberships
            .AnyAsync(
                member =>
                    member.GardenId == gardenId &&
                    member.UserId == userId,
                cancellationToken);
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
