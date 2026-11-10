
namespace RambutanSmartGarden.Api.Features.Users.DTOs;

public sealed record CreateUserResponse(
    Guid Id,
    string Username,
    string DisplayName,
    string? PhoneNumber,
    bool IsActive,
    DateTime CreatedAt
);
