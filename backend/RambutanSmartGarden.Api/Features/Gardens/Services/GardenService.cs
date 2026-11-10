
using System.Text.Json;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Services;

public sealed class GardenService : IGardenService
{
    private readonly IGardenRepository _gardenRepository;

    public GardenService(IGardenRepository gardenRepository)
    {
        _gardenRepository = gardenRepository;
    }

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

        // System Admin tạo vườn được gán Garden Admin.
        var membership = new GardenMembership
        {
            Id = Guid.NewGuid(),
            GardenId = garden.Id,
            UserId = creatorUserId,
            Role = "Admin",
            JoinedAt = now
        };

        // Không ghi nhận dữ liệu nhạy cảm trong AuditLog.
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

        var response = new GardenResponse(
            garden.Id,
            garden.Name,
            garden.Description,
            garden.LocationDescription,
            garden.AreaSquareMeters,
            garden.CreatedAt,
            garden.UpdatedAt);

        return new CreateGardenResult(
            CreateGardenStatus.Succeeded,
            response);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
