
using Microsoft.EntityFrameworkCore;
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
            .FirstOrDefaultAsync(user =>
                user.NormalizedUsername == normalizedIdentifier ||
                user.NormalizedPhoneNumber == normalizedIdentifier,
                cancellationToken);
    }
}
