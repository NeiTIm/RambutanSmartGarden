
using Microsoft.AspNetCore.Identity;
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Authentication;

namespace RambutanSmartGarden.Api.Features.Auth.Services;

public sealed class AuthService : IAuthService
{
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
        var identifier = request.Identifier.Trim();

        if (string.IsNullOrWhiteSpace(identifier) ||
            string.IsNullOrEmpty(request.Password))
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

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return _jwtTokenService.CreateAccessToken(user);
    }

    private static string NormalizeIdentifier(string identifier)
    {
        // Chuẩn hóa số di động Việt Nam về +84.
        if (System.Text.RegularExpressions.Regex.IsMatch(
                identifier,
                @"^0[35789]\d{8}$"))
        {
            return "+84" + identifier[1..];
        }

        return identifier.ToUpperInvariant();
    }
}
