
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public interface IJwtTokenService
{
    LoginResponse CreateAccessToken(User user);
}
