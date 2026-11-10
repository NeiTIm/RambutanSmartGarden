
namespace RambutanSmartGarden.Api.Features.Auth.DTOs;

public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    LoginUserDto User
);

public sealed record LoginUserDto(
    Guid Id,
    string Username,
    string DisplayName,
    bool IsSystemAdmin
);
