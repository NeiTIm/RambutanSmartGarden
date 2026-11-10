
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authorization;

namespace RambutanSmartGarden.Api.Features.Gardens.Services;

public sealed class GardenMemberService : IGardenMemberService
{
    private readonly IGardenMemberRepository _repository;
    private readonly IGardenAccessService _gardenAccess;

    public GardenMemberService(
        IGardenMemberRepository repository,
        IGardenAccessService gardenAccess)
    {
        _repository = repository;
        _gardenAccess = gardenAccess;
    }

    public async Task<List<GardenMemberResponse>?> GetMembersAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        var allowed = await _gardenAccess.HasPermissionAsync(
            principal,
            gardenId,
            GardenPermission.ManageMembers,
            cancellationToken);

        if (!allowed)
        {
            return null;
        }

        return await _repository.GetMembersAsync(
            gardenId,
            cancellationToken);
    }

    public async Task<AddGardenMemberResult> AddMemberAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        AddGardenMemberRequest request,
        CancellationToken cancellationToken)
    {
        var allowed = await _gardenAccess.HasPermissionAsync(
            principal,
            gardenId,
            GardenPermission.ManageMembers,
            cancellationToken);

        if (!allowed)
        {
            return new AddGardenMemberResult(
                AddGardenMemberStatus.Forbidden);
        }

        if (request.UserId == Guid.Empty ||
            request.Role is not ("Member" or "Viewer"))
        {
            return new AddGardenMemberResult(
                AddGardenMemberStatus.InvalidRequest);
        }

        var actorIdText = principal.FindFirst("sub")?.Value;

        if (!Guid.TryParse(actorIdText, out var actorUserId))
        {
            return new AddGardenMemberResult(
                AddGardenMemberStatus.Forbidden);
        }

        var user = await _repository.FindActiveUserAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return new AddGardenMemberResult(
                AddGardenMemberStatus.UserNotFound);
        }

        if (await _repository.MembershipExistsAsync(
                gardenId,
                user.Id,
                cancellationToken))
        {
            return new AddGardenMemberResult(
                AddGardenMemberStatus.AlreadyMember);
        }

        var now = DateTime.UtcNow;

        var membership = new GardenMembership
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            UserId = user.Id,
            Role = request.Role,
            JoinedAt = now
        };

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            GardenId = gardenId,
            Action = "GardenMember.Added",
            EntityType = "GardenMembership",
            EntityId = membership.Id.ToString(),
            NewValues = JsonSerializer.Serialize(new
            {
                membership.GardenId,
                membership.UserId,
                membership.Role
            }),
            OccurredAt = now
        };

        _repository.AddMembership(membership);
        _repository.AddAuditLog(auditLog);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException
            {
                Number: 2601 or 2627
            })
        {
            return new AddGardenMemberResult(
                AddGardenMemberStatus.AlreadyMember);
        }

        var response = new GardenMemberResponse(
            membership.Id,
            user.Id,
            user.Username,
            user.DisplayName,
            membership.Role,
            membership.JoinedAt);

        return new AddGardenMemberResult(
            AddGardenMemberStatus.Succeeded,
            response);
    }
}
