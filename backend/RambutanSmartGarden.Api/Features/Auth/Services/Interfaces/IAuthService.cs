
using RambutanSmartGarden.Api.Features.Auth.DTOs;

namespace RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}
