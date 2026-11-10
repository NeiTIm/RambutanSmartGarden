
using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories;

public sealed class GardenMemberManagementRepository
    : IGardenMemberManagementRepository
{
    private readonly AppDbContext _dbContext;

    public GardenMemberManagementRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<GardenMemberMutationStatus> ChangeRoleAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        string newRole,
        CancellationToken cancellationToken)
    {
        return ExecuteMutationAsync(
            actorUserId,
            gardenId,
            targetUserId,
            newRole,
            remove: false,
            cancellationToken);
    }

    public Task<GardenMemberMutationStatus> RemoveMemberAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        CancellationToken cancellationToken)
    {
        return ExecuteMutationAsync(
            actorUserId,
            gardenId,
            targetUserId,
            newRole: null,
            remove: true,
            cancellationToken);
    }

    private async Task<GardenMemberMutationStatus> ExecuteMutationAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        string? newRole,
        bool remove,
        CancellationToken cancellationToken)
    {
        if (actorUserId == targetUserId)
        {
            return GardenMemberMutationStatus
                .SelfModificationNotAllowed;
        }

        try
        {
            // SERIALIZABLE giúp bảo vệ điều kiện
            // còn ít nhất một Garden Admin.
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            var actor = await _dbContext.Users
                .AsNoTracking()
                .Where(user =>
                    user.Id == actorUserId &&
                    user.IsActive)
                .Select(user => new
                {
                    user.IsSystemAdmin
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (actor is null)
            {
                return GardenMemberMutationStatus.Forbidden;
            }

            var gardenExists = await _dbContext.Gardens
                .AsNoTracking()
                .AnyAsync(
                    garden => garden.Id == gardenId,
                    cancellationToken);

            if (!gardenExists)
            {
                return GardenMemberMutationStatus.NotFound;
            }

            var actorIsGardenAdmin = await _dbContext
                .GardenMemberships
                .AsNoTracking()
                .AnyAsync(
                    member =>
                        member.GardenId == gardenId &&
                        member.UserId == actorUserId &&
                        member.Role == "Admin",
                    cancellationToken);

            if (!actor.IsSystemAdmin &&
                !actorIsGardenAdmin)
            {
                return GardenMemberMutationStatus.Forbidden;
            }

            var membership = await _dbContext.GardenMemberships
                .FirstOrDefaultAsync(
                    member =>
                        member.GardenId == gardenId &&
                        member.UserId == targetUserId,
                    cancellationToken);

            if (membership is null)
            {
                return GardenMemberMutationStatus.NotFound;
            }

            var oldRole = membership.Role;

            // Chỉ System Admin được sửa hoặc gỡ Garden Admin.
            if (oldRole == "Admin" && !actor.IsSystemAdmin)
            {
                return GardenMemberMutationStatus.Forbidden;
            }

            if (!remove)
            {
                if (newRole is not ("Admin" or "Member" or "Viewer"))
                {
                    return GardenMemberMutationStatus.InvalidRole;
                }

                // Chỉ System Admin được cấp quyền Garden Admin.
                if (newRole == "Admin" && !actor.IsSystemAdmin)
                {
                    return GardenMemberMutationStatus.Forbidden;
                }

                if (oldRole == newRole)
                {
                    return GardenMemberMutationStatus.NoChange;
                }
            }

            // Nếu thao tác lấy đi vai trò Admin hiện tại,
            // Garden phải còn ít nhất một Admin khác.
            if (oldRole == "Admin" &&
                (remove || newRole != "Admin"))
            {
                var adminCount = await _dbContext
                    .GardenMemberships
                    .CountAsync(
                        member =>
                            member.GardenId == gardenId &&
                            member.Role == "Admin",
                        cancellationToken);

                if (adminCount <= 1)
                {
                    return GardenMemberMutationStatus
                        .LastAdminProtected;
                }
            }

            var now = DateTime.UtcNow;

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                ActorUserId = actorUserId,
                GardenId = gardenId,
                Action = remove
                    ? "GardenMember.Removed"
                    : "GardenMember.RoleChanged",
                EntityType = "GardenMembership",
                EntityId = membership.Id.ToString(),
                OldValues = JsonSerializer.Serialize(new
                {
                    membership.UserId,
                    Role = oldRole
                }),
                NewValues = JsonSerializer.Serialize(new
                {
                    membership.UserId,
                    Role = remove ? null : newRole,
                    Removed = remove
                }),
                OccurredAt = now
            };

            if (remove)
            {
                _dbContext.GardenMemberships.Remove(
                    membership);
            }
            else
            {
                membership.Role = newRole!;
            }

            _dbContext.AuditLogs.Add(auditLog);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return GardenMemberMutationStatus.Succeeded;
        }
        catch (DbUpdateConcurrencyException)
        {
            return GardenMemberMutationStatus
                .ConcurrencyConflict;
        }
        catch (SqlException exception)
            when (exception.Number == 1205)
        {
            // SQL Server có thể chọn transaction này
            // làm deadlock victim khi nhiều người cùng sửa.
            return GardenMemberMutationStatus
                .ConcurrencyConflict;
        }
    }
}
