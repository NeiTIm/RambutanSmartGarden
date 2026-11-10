
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<User?> FindByIdentifierAsync(
        string normalizedIdentifier,
        CancellationToken cancellationToken);
}
