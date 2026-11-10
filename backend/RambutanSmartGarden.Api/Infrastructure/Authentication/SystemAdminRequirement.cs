
using Microsoft.AspNetCore.Authorization;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public sealed class SystemAdminRequirement
    : IAuthorizationRequirement
{
}
