
using RambutanSmartGarden.Api.Features.Users.DTOs;

namespace RambutanSmartGarden.Api.Features.Users.Services.Interfaces;

public interface IAdminUserService
{
    Task<CreateUserResult> CreateAsync(
        CreateUserRequest request,
        Guid adminUserId,
        CancellationToken cancellationToken);
}

public sealed record CreateUserResult(
    CreateUserResponse? User,
    string? ConflictMessage
);
