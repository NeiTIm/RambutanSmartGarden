
using Microsoft.AspNetCore.Identity;
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Authentication;

namespace RambutanSmartGarden.Api.Features.Auth.Services;

public sealed class AuthService : IAuthService
{
    private const int RefreshTokenDays = 7;

    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IAuthRepository repository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var identifier = request.Identifier?.Trim();

        if (string.IsNullOrWhiteSpace(identifier) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var normalizedIdentifier =
            NormalizeIdentifier(identifier);

        var user = await _repository.FindByIdentifierAsync(
            normalizedIdentifier,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verificationResult ==
            PasswordVerificationResult.Failed)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var sessionId = Guid.NewGuid();

        var refreshToken = RefreshTokenFactory.Create(
            sessionId);

        var session = new UserSession
        {
            Id = sessionId,
            UserId = user.Id,
            RefreshTokenHash = refreshToken.Hash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(RefreshTokenDays),
            LastSeenAt = now
        };

        var accessToken = _jwtTokenService
            .CreateAccessToken(user, sessionId);

        _repository.AddSession(session);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return BuildResponse(
            user,
            accessToken,
            refreshToken.Token,
            session.ExpiresAt);
    }

    public async Task<LoginResponse?> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var presentedToken = request.RefreshToken;

        if (!RefreshTokenFactory.TryGetSessionId(
                presentedToken,
                out var sessionId))
        {
            return null;
        }

        var session = await _repository.FindSessionAsync(
            sessionId,
            cancellationToken);

        var now = DateTime.UtcNow;

        if (session is null ||
            session.RevokedAt is not null ||
            session.ExpiresAt <= now ||
            string.IsNullOrWhiteSpace(session.RefreshTokenHash))
        {
            return null;
        }

        var currentHash = RefreshTokenFactory.ComputeHash(
            presentedToken);

        // Kiểm tra sơ bộ trước khi tạo token mới.
        if (!string.Equals(
                session.RefreshTokenHash,
                currentHash,
                StringComparison.Ordinal))
        {
            return null;
        }

        var user = await _repository.FindUserByIdAsync(
            session.UserId,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var nextRefreshToken = RefreshTokenFactory.Create(
            session.Id);

        var nextExpiresAt = now.AddDays(RefreshTokenDays);

        var nextAccessToken = _jwtTokenService
            .CreateAccessToken(user, session.Id);

        // Chỉ request sở hữu token hiện hành
        // mới có thể xoay vòng thành công.
        var rotated = await _repository.TryRotateRefreshTokenAsync(
            session.Id,
            currentHash,
            nextRefreshToken.Hash,
            now,
            nextExpiresAt,
            cancellationToken);

        if (!rotated)
        {
            return null;
        }

        return BuildResponse(
            user,
            nextAccessToken,
            nextRefreshToken.Token,
            nextExpiresAt);
    }

    public Task<bool> LogoutAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        return _repository.RevokeSessionAsync(
            sessionId,
            userId,
            DateTime.UtcNow,
            cancellationToken);
    }

    private static LoginResponse BuildResponse(
        User user,
        AccessTokenResult accessToken,
        string refreshToken,
        DateTime refreshTokenExpiresAt)
    {
        return new LoginResponse(
            accessToken.Token,
            "Bearer",
            accessToken.ExpiresAt,
            refreshToken,
            refreshTokenExpiresAt,
            new LoginUserDto(
                user.Id,
                user.Username,
                user.DisplayName,
                user.IsSystemAdmin));
    }

    private static string NormalizeIdentifier(
        string identifier)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(
                identifier,
                @"^0[35789]\d{8}$"))
        {
            return "+84" + identifier[1..];
        }

        return identifier.ToUpperInvariant();
    }
}
