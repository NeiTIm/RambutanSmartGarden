
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authorization;

namespace RambutanSmartGarden.Api.Features.Gardens.Areas.Services;

public sealed class GardenAreaService : IGardenAreaService
{
    private readonly IGardenAreaRepository _repository;
    private readonly IGardenAccessService _gardenAccess;

    public GardenAreaService(
        IGardenAreaRepository repository,
        IGardenAccessService gardenAccess)
    {
        _repository = repository;
        _gardenAccess = gardenAccess;
    }

    public async Task<GardenAreaListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new GardenAreaListResult(
                GardenAreaStatus.NotFound);
        }

        var areas = await _repository.GetByGardenAsync(
            gardenId,
            cancellationToken);

        return new GardenAreaListResult(
            GardenAreaStatus.Succeeded,
            areas.Select(ToResponse).ToList());
    }

    public async Task<GardenAreaResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid areaId,
        CancellationToken cancellationToken)
    {
        if (areaId == Guid.Empty ||
            !await HasPermissionAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new GardenAreaResult(GardenAreaStatus.NotFound);
        }

        var area = await _repository.FindByIdAsync(
            gardenId,
            areaId,
            false,
            cancellationToken);

        return area is null
            ? new GardenAreaResult(GardenAreaStatus.NotFound)
            : new GardenAreaResult(
                GardenAreaStatus.Succeeded,
                ToResponse(area));
    }

    public async Task<GardenAreaResult> CreateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CreateGardenAreaRequest request,
        CancellationToken cancellationToken)
    {
        if (!await HasPermissionAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new GardenAreaResult(GardenAreaStatus.NotFound);
        }

        var name = request.Name?.Trim();
        var description = NormalizeOptional(request.Description);

        if (!IsValid(name, description, request.AreaSquareMeters))
        {
            return new GardenAreaResult(
                GardenAreaStatus.InvalidRequest);
        }

        if (!TryGetActorId(principal, out var actorId))
        {
            return new GardenAreaResult(GardenAreaStatus.NotFound);
        }

        var now = DateTime.UtcNow;

        var area = new GardenArea
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            Name = name!,
            Description = description,
            AreaSquareMeters = request.AreaSquareMeters,
            CreatedAt = now
        };

        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorId,
            GardenId = gardenId,
            Action = "GardenArea.Created",
            EntityType = "GardenArea",
            EntityId = area.Id.ToString(),
            NewValues = JsonSerializer.Serialize(new
            {
                area.Name,
                area.Description,
                area.AreaSquareMeters
            }),
            OccurredAt = now
        };

        _repository.Add(area);
        _repository.AddAuditLog(audit);

        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return new GardenAreaResult(
                GardenAreaStatus.DuplicateName);
        }

        return new GardenAreaResult(
            GardenAreaStatus.Succeeded,
            ToResponse(area));
    }

    public async Task<GardenAreaResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid areaId,
        UpdateGardenAreaRequest request,
        CancellationToken cancellationToken)
    {
        if (areaId == Guid.Empty ||
            !await HasPermissionAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new GardenAreaResult(GardenAreaStatus.NotFound);
        }

        var name = request.Name?.Trim();
        var description = NormalizeOptional(request.Description);

        if (!IsValid(name, description, request.AreaSquareMeters))
        {
            return new GardenAreaResult(
                GardenAreaStatus.InvalidRequest);
        }

        if (!TryDecodeRowVersion(
                request.RowVersion,
                out var originalVersion))
        {
            return new GardenAreaResult(
                GardenAreaStatus.InvalidRequest);
        }

        if (!TryGetActorId(principal, out var actorId))
        {
            return new GardenAreaResult(GardenAreaStatus.NotFound);
        }

        var area = await _repository.FindByIdAsync(
            gardenId,
            areaId,
            true,
            cancellationToken);

        if (area is null)
        {
            return new GardenAreaResult(GardenAreaStatus.NotFound);
        }

        if (!area.RowVersion.SequenceEqual(originalVersion))
        {
            return new GardenAreaResult(
                GardenAreaStatus.ConcurrencyConflict);
        }

        var previousValues = JsonSerializer.Serialize(new
        {
            area.Name,
            area.Description,
            area.AreaSquareMeters
        });

        var now = DateTime.UtcNow;

        area.Name = name!;
        area.Description = description;
        area.AreaSquareMeters = request.AreaSquareMeters;
        area.UpdatedAt = now;

        _repository.SetOriginalRowVersion(
            area,
            originalVersion);

        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorId,
            GardenId = gardenId,
            Action = "GardenArea.Updated",
            EntityType = "GardenArea",
            EntityId = area.Id.ToString(),
            OldValues = previousValues,
            NewValues = JsonSerializer.Serialize(new
            {
                area.Name,
                area.Description,
                area.AreaSquareMeters
            }),
            OccurredAt = now
        };

        _repository.AddAuditLog(audit);

        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new GardenAreaResult(
                GardenAreaStatus.ConcurrencyConflict);
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return new GardenAreaResult(
                GardenAreaStatus.DuplicateName);
        }

        return new GardenAreaResult(
            GardenAreaStatus.Succeeded,
            ToResponse(area));
    }

    private Task<bool> HasPermissionAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        GardenPermission permission,
        CancellationToken cancellationToken)
    {
        if (gardenId == Guid.Empty)
        {
            return Task.FromResult(false);
        }

        return _gardenAccess.HasPermissionAsync(
            principal,
            gardenId,
            permission,
            cancellationToken);
    }

    private static bool TryGetActorId(
        ClaimsPrincipal principal,
        out Guid actorId)
    {
        return Guid.TryParse(
            principal.FindFirst("sub")?.Value,
            out actorId);
    }

    private static bool IsValid(
        string? name,
        string? description,
        decimal? area)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               name.Length <= 150 &&
               description?.Length is null or <= 2000 &&
               (area is null || area > 0);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static bool TryDecodeRowVersion(
        string? value,
        out byte[] rowVersion)
    {
        rowVersion = [];

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            rowVersion = Convert.FromBase64String(value);
            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException
        {
            Number: 2601 or 2627
        };
    }

    private static GardenAreaResponse ToResponse(
        GardenArea area)
    {
        return new GardenAreaResponse(
            area.Id,
            area.GardenId,
            area.Name,
            area.Description,
            area.AreaSquareMeters,
            area.CreatedAt,
            area.UpdatedAt,
            Convert.ToBase64String(area.RowVersion));
    }
}
