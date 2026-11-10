
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAt
);

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(
        User user,
        Guid sessionId);
}
