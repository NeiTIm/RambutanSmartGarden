
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;

        var signingKeyBytes = Convert.FromBase64String(
            jwtOptions.SigningKey);

        var securityKey = new SymmetricSecurityKey(
            signingKeyBytes);

        _signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    public LoginResponse CreateAccessToken(User user)
    {
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(
            _jwtOptions.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = _signingCredentials
        };

        var tokenHandler = new JsonWebTokenHandler();
        var accessToken = tokenHandler.CreateToken(descriptor);

        var userDto = new LoginUserDto(
            user.Id,
            user.Username,
            user.DisplayName,
            user.IsSystemAdmin);

        return new LoginResponse(
            accessToken,
            "Bearer",
            expiresAt,
            userDto);
    }
}
