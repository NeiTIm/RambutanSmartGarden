
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Auth.Repositories;

public sealed class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _dbContext;

    public AuthRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> FindByIdentifierAsync(
        string normalizedIdentifier,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user =>
                    user.NormalizedUsername == normalizedIdentifier ||
                    user.NormalizedPhoneNumber == normalizedIdentifier,
                cancellationToken);
    }

    public Task<User?> FindUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken);
    }

    public Task<UserSession?> FindSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        return _dbContext.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                session => session.Id == sessionId,
                cancellationToken);
    }

    public void AddSession(UserSession session)
    {
        _dbContext.UserSessions.Add(session);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<bool> TryRotateRefreshTokenAsync(
        Guid sessionId,
        string currentTokenHash,
        string nextTokenHash,
        DateTime now,
        DateTime newExpiresAt,
        CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.UserSessions
            .Where(session =>
                session.Id == sessionId &&
                session.RefreshTokenHash == currentTokenHash &&
                session.RevokedAt == null &&
                session.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    session => session.RefreshTokenHash,
                    nextTokenHash)
                .SetProperty(
                    session => session.ExpiresAt,
                    newExpiresAt)
                .SetProperty(
                    session => session.LastSeenAt,
                    (DateTime?)now),
                cancellationToken);

        return affectedRows == 1;
    }

    public async Task<bool> RevokeSessionAsync(
        Guid sessionId,
        Guid userId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.UserSessions
            .Where(session =>
                session.Id == sessionId &&
                session.UserId == userId &&
                session.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    session => session.RevokedAt,
                    (DateTime?)now)
                .SetProperty(
                    session => session.RevokedReason,
                    "UserLogout")
                .SetProperty(
                    session => session.RefreshTokenHash,
                    (string?)null),
                cancellationToken);

        return affectedRows == 1;
    }
}
