
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public static class RefreshTokenFactory
{
    public static (string Token, string Hash) Create(
        Guid sessionId)
    {
        var secret = RandomNumberGenerator.GetBytes(48);

        var token = $"{sessionId:N}.{Base64UrlEncoder.Encode(secret)}";

        var hash = ComputeHash(token);

        return (token, hash);
    }

    public static string ComputeHash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }

    public static bool TryGetSessionId(
        string? token,
        out Guid sessionId)
    {
        sessionId = Guid.Empty;

        if (string.IsNullOrWhiteSpace(token) ||
            token.Length > 200)
        {
            return false;
        }

        var separatorIndex = token.IndexOf('.');

        if (separatorIndex != 32 ||
            token.IndexOf('.', separatorIndex + 1) != -1)
        {
            return false;
        }

        return Guid.TryParseExact(
            token[..separatorIndex],
            "N",
            out sessionId);
    }
}
