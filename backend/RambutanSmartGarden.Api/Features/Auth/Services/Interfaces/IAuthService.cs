
using RambutanSmartGarden.Api.Features.Auth.DTOs;

namespace RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<LoginResponse?> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken);

    Task<bool> LogoutAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken);
}
