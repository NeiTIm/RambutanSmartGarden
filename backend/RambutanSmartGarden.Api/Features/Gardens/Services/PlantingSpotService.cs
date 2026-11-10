
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authorization;

namespace RambutanSmartGarden.Api.Features.Gardens.Services;

public sealed class PlantingSpotService : IPlantingSpotService
{
    private readonly IPlantingSpotRepository _repository;
    private readonly IGardenAccessService _gardenAccess;

    public PlantingSpotService(
        IPlantingSpotRepository repository,
        IGardenAccessService gardenAccess)
    {
        _repository = repository;
        _gardenAccess = gardenAccess;
    }

    public async Task<PlantingSpotListResult> GetAllAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        if (!await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new(PlantingSpotStatus.NotFound);
        }

        var spots = await _repository.GetByGardenAsync(
            gardenId,
            cancellationToken);

        return new(
            PlantingSpotStatus.Succeeded,
            spots.Select(ToResponse).ToList());
    }

    public async Task<PlantingSpotResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid spotId,
        CancellationToken cancellationToken)
    {
        if (spotId == Guid.Empty ||
            !await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new(PlantingSpotStatus.NotFound);
        }

        var spot = await _repository.FindAsync(
            gardenId,
            spotId,
            false,
            cancellationToken);

        return spot is null
            ? new(PlantingSpotStatus.NotFound)
            : new(PlantingSpotStatus.Succeeded, ToResponse(spot));
    }

    public async Task<PlantingSpotResult> CreateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CreatePlantingSpotRequest request,
        CancellationToken cancellationToken)
    {
        if (!await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new(PlantingSpotStatus.NotFound);
        }

        var code = request.Code?.Trim().ToUpperInvariant();
        var description = NormalizeOptional(request.Description);

        if (!IsValid(
                code,
                description,
                request.Latitude,
                request.Longitude) ||
            request.GardenAreaId == Guid.Empty)
        {
            return new(PlantingSpotStatus.InvalidRequest);
        }

        if (request.GardenAreaId is Guid areaId &&
            !await _repository.GardenAreaBelongsToGardenAsync(
                gardenId,
                areaId,
                cancellationToken))
        {
            return new(PlantingSpotStatus.InvalidGardenArea);
        }

        if (!TryGetActorId(principal, out var actorId))
        {
            return new(PlantingSpotStatus.NotFound);
        }

        var now = DateTime.UtcNow;

        var spot = new PlantingSpot
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            GardenAreaId = request.GardenAreaId,
            Code = code!,
            Description = description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = now
        };

        var audit = CreateAudit(
            actorId,
            spot,
            "PlantingSpot.Created",
            null,
            now);

        _repository.Add(spot);
        _repository.AddAuditLog(audit);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return new(PlantingSpotStatus.DuplicateCode);
        }

        return new(
            PlantingSpotStatus.Succeeded,
            ToResponse(spot));
    }

    public async Task<PlantingSpotResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid spotId,
        UpdatePlantingSpotRequest request,
        CancellationToken cancellationToken)
    {
        if (spotId == Guid.Empty ||
            !await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new(PlantingSpotStatus.NotFound);
        }

        var code = request.Code?.Trim().ToUpperInvariant();
        var description = NormalizeOptional(request.Description);

        if (!IsValid(
                code,
                description,
                request.Latitude,
                request.Longitude) ||
            request.GardenAreaId == Guid.Empty ||
            !TryDecodeRowVersion(
                request.RowVersion,
                out var originalVersion))
        {
            return new(PlantingSpotStatus.InvalidRequest);
        }

        if (request.GardenAreaId is Guid areaId &&
            !await _repository.GardenAreaBelongsToGardenAsync(
                gardenId,
                areaId,
                cancellationToken))
        {
            return new(PlantingSpotStatus.InvalidGardenArea);
        }

        if (!TryGetActorId(principal, out var actorId))
        {
            return new(PlantingSpotStatus.NotFound);
        }

        var spot = await _repository.FindAsync(
            gardenId,
            spotId,
            true,
            cancellationToken);

        if (spot is null)
        {
            return new(PlantingSpotStatus.NotFound);
        }

        if (!spot.RowVersion.SequenceEqual(originalVersion))
        {
            return new(PlantingSpotStatus.ConcurrencyConflict);
        }

        var oldValues = JsonSerializer.Serialize(new
        {
            spot.Code,
            spot.GardenAreaId,
            spot.Description,
            spot.Latitude,
            spot.Longitude
        });

        var now = DateTime.UtcNow;

        spot.Code = code!;
        spot.GardenAreaId = request.GardenAreaId;
        spot.Description = description;
        spot.Latitude = request.Latitude;
        spot.Longitude = request.Longitude;
        spot.UpdatedAt = now;

        _repository.SetOriginalRowVersion(
            spot,
            originalVersion);

        var audit = CreateAudit(
            actorId,
            spot,
            "PlantingSpot.Updated",
            oldValues,
            now);

        _repository.AddAuditLog(audit);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(PlantingSpotStatus.ConcurrencyConflict);
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return new(PlantingSpotStatus.DuplicateCode);
        }

        return new(
            PlantingSpotStatus.Succeeded,
            ToResponse(spot));
    }

    private Task<bool> HasAccessAsync(
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
        string? code,
        string? description,
        decimal? latitude,
        decimal? longitude)
    {
        if (string.IsNullOrWhiteSpace(code) ||
            code.Length > 30 ||
            !Regex.IsMatch(code, @"^[A-Z0-9_-]+$") ||
            description?.Length > 1000)
        {
            return false;
        }

        if (latitude.HasValue != longitude.HasValue)
        {
            return false;
        }

        if (latitude is < -90 or > 90 ||
            longitude is < -180 or > 180)
        {
            return false;
        }

        // decimal(9,6): tối đa 6 chữ số thập phân.
        if (latitude.HasValue &&
            decimal.Round(latitude.Value, 6) != latitude.Value)
        {
            return false;
        }

        if (longitude.HasValue &&
            decimal.Round(longitude.Value, 6) != longitude.Value)
        {
            return false;
        }

        return true;
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

    private static AuditLog CreateAudit(
        Guid actorId,
        PlantingSpot spot,
        string action,
        string? oldValues,
        DateTime occurredAt)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorId,
            GardenId = spot.GardenId,
            Action = action,
            EntityType = "PlantingSpot",
            EntityId = spot.Id.ToString(),
            OldValues = oldValues,
            NewValues = JsonSerializer.Serialize(new
            {
                spot.Code,
                spot.GardenAreaId,
                spot.Description,
                spot.Latitude,
                spot.Longitude
            }),
            OccurredAt = occurredAt
        };
    }

    private static PlantingSpotResponse ToResponse(
        PlantingSpot spot)
    {
        return new PlantingSpotResponse(
            spot.Id,
            spot.GardenId,
            spot.GardenAreaId,
            spot.Code,
            spot.Description,
            spot.Latitude,
            spot.Longitude,
            spot.CreatedAt,
            spot.UpdatedAt,
            Convert.ToBase64String(spot.RowVersion));
    }
}
