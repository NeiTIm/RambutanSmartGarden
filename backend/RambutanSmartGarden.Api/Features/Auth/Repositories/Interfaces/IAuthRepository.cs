
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<User?> FindByIdentifierAsync(
        string normalizedIdentifier,
        CancellationToken cancellationToken);

    Task<User?> FindUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserSession?> FindSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken);

    void AddSession(UserSession session);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<bool> TryRotateRefreshTokenAsync(
        Guid sessionId,
        string currentTokenHash,
        string nextTokenHash,
        DateTime now,
        DateTime newExpiresAt,
        CancellationToken cancellationToken);

    Task<bool> RevokeSessionAsync(
        Guid sessionId,
        Guid userId,
        DateTime now,
        CancellationToken cancellationToken);
}
