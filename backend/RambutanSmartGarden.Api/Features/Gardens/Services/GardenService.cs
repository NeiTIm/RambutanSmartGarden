
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authorization;

namespace RambutanSmartGarden.Api.Features.Gardens.Services;

public sealed class GardenService : IGardenService
{
    private readonly IGardenRepository _gardenRepository;
    private readonly IGardenAccessService _gardenAccess;

    public GardenService(
        IGardenRepository gardenRepository,
        IGardenAccessService gardenAccess)
    {
        _gardenRepository = gardenRepository;
        _gardenAccess = gardenAccess;
    }

    // Phase 03.3.1 - Create.

    public async Task<CreateGardenResult> CreateAsync(
        CreateGardenRequest request,
        Guid creatorUserId,
        CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();

        if (creatorUserId == Guid.Empty ||
            string.IsNullOrWhiteSpace(name) ||
            name.Length > 150 ||
            request.AreaSquareMeters is <= 0)
        {
            return new CreateGardenResult(
                CreateGardenStatus.InvalidRequest);
        }

        var description = NormalizeOptional(
            request.Description);

        var locationDescription = NormalizeOptional(
            request.LocationDescription);

        if (description?.Length > 2000 ||
            locationDescription?.Length > 500)
        {
            return new CreateGardenResult(
                CreateGardenStatus.InvalidRequest);
        }

        var now = DateTime.UtcNow;

        var garden = new Garden
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            LocationDescription = locationDescription,
            AreaSquareMeters = request.AreaSquareMeters,
            CreatedAt = now
        };

        var membership = new GardenMembership
        {
            Id = Guid.NewGuid(),
            GardenId = garden.Id,
            UserId = creatorUserId,
            Role = "Admin",
            JoinedAt = now
        };

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = creatorUserId,
            GardenId = garden.Id,
            Action = "Garden.Created",
            EntityType = "Garden",
            EntityId = garden.Id.ToString(),
            NewValues = JsonSerializer.Serialize(new
            {
                garden.Id,
                garden.Name,
                garden.AreaSquareMeters
            }),
            OccurredAt = now
        };

        _gardenRepository.AddGarden(garden);
        _gardenRepository.AddMembership(membership);
        _gardenRepository.AddAuditLog(auditLog);

        await _gardenRepository.SaveChangesAsync(
            cancellationToken);

        return new CreateGardenResult(
            CreateGardenStatus.Succeeded,
            new GardenResponse(
                garden.Id,
                garden.Name,
                garden.Description,
                garden.LocationDescription,
                garden.AreaSquareMeters,
                garden.CreatedAt,
                garden.UpdatedAt));
    }

    // Phase 03.3.2 - List.

    public async Task<List<GardenListItemResponse>?>
        GetAccessibleGardensAsync(
            Guid currentUserId,
            CancellationToken cancellationToken)
    {
        if (currentUserId == Guid.Empty)
        {
            return null;
        }

        var isSystemAdmin =
            await _gardenRepository.GetSystemAdminStatusAsync(
                currentUserId,
                cancellationToken);

        if (isSystemAdmin is null)
        {
            return null;
        }

        return await _gardenRepository.GetAccessibleGardensAsync(
            currentUserId,
            isSystemAdmin.Value,
            cancellationToken);
    }

    // Phase 03.3.3 - Detail.

    public async Task<GardenDetailResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        var canView = await _gardenAccess.HasPermissionAsync(
            principal,
            gardenId,
            GardenPermission.View,
            cancellationToken);

        if (!canView)
        {
            return new GardenDetailResult(
                GardenOperationStatus.NotFound);
        }

        var garden = await _gardenRepository.FindGardenAsync(
            gardenId,
            trackChanges: false,
            cancellationToken);

        if (garden is null)
        {
            return new GardenDetailResult(
                GardenOperationStatus.NotFound);
        }

        return new GardenDetailResult(
            GardenOperationStatus.Succeeded,
            ToDetailResponse(garden));
    }

    // Phase 03.3.3 - Update.

    public async Task<GardenDetailResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        UpdateGardenRequest request,
        CancellationToken cancellationToken)
    {
        var canManage = await _gardenAccess.HasPermissionAsync(
            principal,
            gardenId,
            GardenPermission.ManageGarden,
            cancellationToken);

        if (!canManage)
        {
            return new GardenDetailResult(
                GardenOperationStatus.NotFound);
        }

        var name = request.Name?.Trim();
        var description = NormalizeOptional(request.Description);
        var location = NormalizeOptional(request.LocationDescription);

        if (string.IsNullOrWhiteSpace(name) ||
            name.Length > 150 ||
            description?.Length > 2000 ||
            location?.Length > 500 ||
            request.AreaSquareMeters is <= 0)
        {
            return new GardenDetailResult(
                GardenOperationStatus.InvalidRequest);
        }

        if (!TryDecodeRowVersion(
            request.RowVersion,
            out var originalRowVersion))
        {
            return new GardenDetailResult(
                GardenOperationStatus.InvalidRequest);
        }

        var actorText = principal.FindFirst("sub")?.Value;

        if (!Guid.TryParse(actorText, out var actorUserId))
        {
            return new GardenDetailResult(
                GardenOperationStatus.NotFound);
        }

        var garden = await _gardenRepository.FindGardenAsync(
            gardenId,
            trackChanges: true,
            cancellationToken);

        if (garden is null)
        {
            return new GardenDetailResult(
                GardenOperationStatus.NotFound);
        }

        // So sánh nhanh với phiên bản hiện tại đã đọc.
        if (!garden.RowVersion.SequenceEqual(originalRowVersion))
        {
            return new GardenDetailResult(
                GardenOperationStatus.ConcurrencyConflict);
        }

        var oldValues = JsonSerializer.Serialize(new
        {
            garden.Name,
            garden.Description,
            garden.LocationDescription,
            garden.AreaSquareMeters
        });

        garden.Name = name;
        garden.Description = description;
        garden.LocationDescription = location;
        garden.AreaSquareMeters = request.AreaSquareMeters;
        garden.UpdatedAt = DateTime.UtcNow;

        // EF Core sẽ dùng RowVersion Client gửi
        // để kiểm tra concurrency tại thời điểm UPDATE.
        _gardenRepository.SetOriginalRowVersion(
            garden,
            originalRowVersion);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            GardenId = garden.Id,
            Action = "Garden.Updated",
            EntityType = "Garden",
            EntityId = garden.Id.ToString(),
            OldValues = oldValues,
            NewValues = JsonSerializer.Serialize(new
            {
                garden.Name,
                garden.Description,
                garden.LocationDescription,
                garden.AreaSquareMeters
            }),
            OccurredAt = garden.UpdatedAt.Value
        };

        _gardenRepository.AddAuditLog(auditLog);

        try
        {
            await _gardenRepository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new GardenDetailResult(
                GardenOperationStatus.ConcurrencyConflict);
        }

        return new GardenDetailResult(
            GardenOperationStatus.Succeeded,
            ToDetailResponse(garden));
    }

    private static GardenDetailResponse ToDetailResponse(
        Garden garden)
    {
        return new GardenDetailResponse(
            garden.Id,
            garden.Name,
            garden.Description,
            garden.LocationDescription,
            garden.AreaSquareMeters,
            garden.CreatedAt,
            garden.UpdatedAt,
            Convert.ToBase64String(garden.RowVersion));
    }

    private static bool TryDecodeRowVersion(
        string? encoded,
        out byte[] rowVersion)
    {
        rowVersion = [];

        if (string.IsNullOrWhiteSpace(encoded))
        {
            return false;
        }

        try
        {
            rowVersion = Convert.FromBase64String(encoded);

            // SQL Server rowversion có độ dài 8 byte.
            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
