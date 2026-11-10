
using System.Security.Claims;

namespace RambutanSmartGarden.Api.Infrastructure.Authorization;

public interface IGardenAccessService
{
    Task<bool> HasPermissionAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        GardenPermission permission,
        CancellationToken cancellationToken);
}
