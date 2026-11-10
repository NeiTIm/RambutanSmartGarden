
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Infrastructure.Authorization;

public sealed class GardenAccessService : IGardenAccessService
{
    private readonly AppDbContext _dbContext;

    public GardenAccessService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasPermissionAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        GardenPermission permission,
        CancellationToken cancellationToken)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var subject = principal.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var userId) ||
            gardenId == Guid.Empty)
        {
            return false;
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && u.IsActive)
            .Select(u => new
            {
                u.IsSystemAdmin
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return false;
        }

        var gardenExists = await _dbContext.Gardens
            .AsNoTracking()
            .AnyAsync(
                garden => garden.Id == gardenId,
                cancellationToken);

        if (!gardenExists)
        {
            return false;
        }

        // System Admin có quyền trên mọi Garden hiện có.
        if (user.IsSystemAdmin)
        {
            return true;
        }

        var membership = await _dbContext.GardenMemberships
            .AsNoTracking()
            .Where(m =>
                m.GardenId == gardenId &&
                m.UserId == userId)
            .Select(m => new
            {
                m.Role
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (membership is null)
        {
            return false;
        }

        return membership.Role switch
        {
            "Admin" => true,

            "Member" =>
                permission is GardenPermission.View
                    or GardenPermission.RecordCare,

            "Viewer" =>
                permission == GardenPermission.View,

            _ => false
        };
    }
}
