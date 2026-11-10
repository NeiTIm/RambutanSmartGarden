
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;

        var keyBytes = Convert.FromBase64String(
            jwtOptions.SigningKey);

        var securityKey = new SymmetricSecurityKey(keyBytes);

        _signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    public AccessTokenResult CreateAccessToken(
        User user,
        Guid sessionId)
    {
        var now = DateTime.UtcNow;

        var expiresAt = now.AddMinutes(
            _jwtOptions.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString("N")),

            new(
                "sid",
                sessionId.ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = _signingCredentials
        };

        var tokenHandler = new JsonWebTokenHandler();

        return new AccessTokenResult(
            tokenHandler.CreateToken(descriptor),
            expiresAt);
    }
}
