
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Features.Users.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Users.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> UsernameExistsAsync(
        string normalizedUsername,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(
            user => user.NormalizedUsername == normalizedUsername,
            cancellationToken);
    }

    public Task<bool> PhoneExistsAsync(
        string normalizedPhoneNumber,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(
            user => user.NormalizedPhoneNumber == normalizedPhoneNumber,
            cancellationToken);
    }

    public void AddUser(User user)
    {
        _dbContext.Users.Add(user);
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
