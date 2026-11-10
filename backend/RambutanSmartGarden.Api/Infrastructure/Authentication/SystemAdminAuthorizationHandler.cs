
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public sealed class SystemAdminAuthorizationHandler
    : AuthorizationHandler<SystemAdminRequirement>
{
    private readonly AppDbContext _dbContext;

    public SystemAdminAuthorizationHandler(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SystemAdminRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var subject = context.User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var userId))
        {
            return;
        }

        var isAdmin = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user =>
                user.Id == userId &&
                user.IsActive &&
                user.IsSystemAdmin);

        if (isAdmin)
        {
            context.Succeed(requirement);
        }
    }
}
