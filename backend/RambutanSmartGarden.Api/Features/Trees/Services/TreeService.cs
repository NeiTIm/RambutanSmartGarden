
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Trees.DTOs;
using RambutanSmartGarden.Api.Features.Trees.Models;
using RambutanSmartGarden.Api.Features.Trees.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Authorization;

namespace RambutanSmartGarden.Api.Features.Trees.Services;

public sealed class TreeService : ITreeService
{
    private readonly ITreeRepository _repository;
    private readonly IGardenAccessService _gardenAccess;

    public TreeService(
        ITreeRepository repository,
        IGardenAccessService gardenAccess)
    {
        _repository = repository;
        _gardenAccess = gardenAccess;
    }

    public async Task<TreeListResult> GetAllAsync(
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
            return new(TreeOperationStatus.NotFound);
        }

        var trees = await _repository.GetByGardenAsync(
            gardenId,
            cancellationToken);

        return new(
            TreeOperationStatus.Succeeded,
            trees.Select(ToResponse).ToList());
    }

    public async Task<TreeResult> GetByIdAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid treeId,
        CancellationToken cancellationToken)
    {
        if (treeId == Guid.Empty ||
            !await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.View,
                cancellationToken))
        {
            return new(TreeOperationStatus.NotFound);
        }

        var tree = await _repository.FindAsync(
            gardenId,
            treeId,
            false,
            cancellationToken);

        return tree is null
            ? new(TreeOperationStatus.NotFound)
            : new(TreeOperationStatus.Succeeded, ToResponse(tree));
    }

    public async Task<TreeResult> CreateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CreateTreeRequest request,
        CancellationToken cancellationToken)
    {
        if (!await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new(TreeOperationStatus.NotFound);
        }

        var code = NormalizeCode(request.Code);
        var name = NormalizeOptional(request.Name);
        var notes = NormalizeOptional(request.Notes);

        if (!IsValid(
        code,
        name,
        notes,
        request.Status,
        request.PlantedAt,
        request.PlantSpeciesId,
        request.TreeVarietyId,
        request.PlantingSpotId) ||
    request.Status == "Removed")
        {
            return new(TreeOperationStatus.InvalidRequest);
        }

        var relationStatus = await ValidateReferencesAsync(
            gardenId,
            request.PlantSpeciesId,
            request.TreeVarietyId,
            request.PlantingSpotId,
            cancellationToken);

        if (relationStatus != TreeOperationStatus.Succeeded)
        {
            return new(relationStatus);
        }

        if (await _repository.CodeExistsAsync(
                gardenId,
                code!,
                null,
                cancellationToken))
        {
            return new(TreeOperationStatus.DuplicateCode);
        }

        if (request.Status == "Active" &&
            request.PlantingSpotId is Guid spotId &&
            await _repository.SpotOccupiedAsync(
                spotId,
                null,
                cancellationToken))
        {
            return new(TreeOperationStatus.OccupiedPlantingSpot);
        }

        if (!TryGetActorId(principal, out var actorId))
        {
            return new(TreeOperationStatus.NotFound);
        }

        var now = DateTime.UtcNow;

        var tree = new Tree
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            PlantSpeciesId = request.PlantSpeciesId,
            TreeVarietyId = request.TreeVarietyId,
            PlantingSpotId = request.PlantingSpotId,
            Code = code!,
            Name = name,
            PlantedAt = request.PlantedAt,
            Status = request.Status,
            Notes = notes,
            CreatedAt = now
        };

        _repository.Add(tree);
        _repository.AddAuditLog(
            CreateAudit(actorId, tree, "Tree.Created", null, now));

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsConstraintViolation(exception))
        {
            return new(TreeOperationStatus.DataConflict);
        }

        return new(
            TreeOperationStatus.Succeeded,
            ToResponse(tree));
    }

    public async Task<TreeResult> UpdateAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        Guid treeId,
        UpdateTreeRequest request,
        CancellationToken cancellationToken)
    {
        if (treeId == Guid.Empty ||
            !await HasAccessAsync(
                principal,
                gardenId,
                GardenPermission.ManageGarden,
                cancellationToken))
        {
            return new(TreeOperationStatus.NotFound);
        }

        var code = NormalizeCode(request.Code);
        var name = NormalizeOptional(request.Name);
        var notes = NormalizeOptional(request.Notes);

        if (!IsValid(
                code,
                name,
                notes,
                request.Status,
                request.PlantedAt,
                request.PlantSpeciesId,
                request.TreeVarietyId,
                request.PlantingSpotId) ||
            !TryDecodeRowVersion(
                request.RowVersion,
                out var originalVersion))
        {
            return new(TreeOperationStatus.InvalidRequest);
        }

        var relationStatus = await ValidateReferencesAsync(
            gardenId,
            request.PlantSpeciesId,
            request.TreeVarietyId,
            request.PlantingSpotId,
            cancellationToken);

        if (relationStatus != TreeOperationStatus.Succeeded)
        {
            return new(relationStatus);
        }

        var tree = await _repository.FindAsync(
            gardenId,
            treeId,
            true,
            cancellationToken);

        if (tree is null)
        {
            return new(TreeOperationStatus.NotFound);
        }
        // Không cho sửa hồ sơ cây đã bị loại bỏ.
        // Không cho bỏ cây qua API cập nhật thông thường.
        if (tree.Status == "Removed" ||
            request.Status == "Removed")
        {
            return new(TreeOperationStatus.InvalidRequest);
        }

        if (!tree.RowVersion.SequenceEqual(originalVersion))
        {
            return new(TreeOperationStatus.ConcurrencyConflict);
        }

        if (await _repository.CodeExistsAsync(
                gardenId,
                code!,
                treeId,
                cancellationToken))
        {
            return new(TreeOperationStatus.DuplicateCode);
        }

        if (request.Status == "Active" &&
            request.PlantingSpotId is Guid spotId &&
            await _repository.SpotOccupiedAsync(
                spotId,
                treeId,
                cancellationToken))
        {
            return new(TreeOperationStatus.OccupiedPlantingSpot);
        }

        if (!TryGetActorId(principal, out var actorId))
        {
            return new(TreeOperationStatus.NotFound);
        }

        var oldValues = SerializeTreeValues(tree);
        var now = DateTime.UtcNow;

        tree.PlantSpeciesId = request.PlantSpeciesId;
        tree.TreeVarietyId = request.TreeVarietyId;
        tree.PlantingSpotId = request.PlantingSpotId;
        tree.Code = code!;
        tree.Name = name;
        tree.PlantedAt = request.PlantedAt;
        tree.Status = request.Status;
        tree.Notes = notes;
        tree.UpdatedAt = now;

        _repository.SetOriginalRowVersion(
            tree,
            originalVersion);

        _repository.AddAuditLog(
            CreateAudit(
                actorId,
                tree,
                "Tree.Updated",
                oldValues,
                now));

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(TreeOperationStatus.ConcurrencyConflict);
        }
        catch (DbUpdateException exception)
            when (IsConstraintViolation(exception))
        {
            return new(TreeOperationStatus.DataConflict);
        }

        return new(
            TreeOperationStatus.Succeeded,
            ToResponse(tree));
    }

    private async Task<TreeOperationStatus> ValidateReferencesAsync(
        Guid gardenId,
        Guid speciesId,
        Guid? varietyId,
        Guid? spotId,
        CancellationToken cancellationToken)
    {
        if (!await _repository.SpeciesExistsAsync(
                speciesId,
                cancellationToken))
        {
            return TreeOperationStatus.InvalidSpecies;
        }

        if (varietyId is Guid variety &&
            !await _repository.VarietyBelongsToSpeciesAsync(
                speciesId,
                variety,
                cancellationToken))
        {
            return TreeOperationStatus.InvalidVariety;
        }

        if (spotId is Guid spot &&
            !await _repository.SpotBelongsToGardenAsync(
                gardenId,
                spot,
                cancellationToken))
        {
            return TreeOperationStatus.InvalidPlantingSpot;
        }

        return TreeOperationStatus.Succeeded;
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
        string? name,
        string? notes,
        string? status,
        DateOnly? plantedAt,
        Guid speciesId,
        Guid? varietyId,
        Guid? spotId)
    {
        if (string.IsNullOrWhiteSpace(code) ||
            code.Length > 30 ||
            !Regex.IsMatch(code, @"^[A-Z0-9_-]+$") ||
            name?.Length > 150 ||
            notes?.Length > 2000 ||
            status is not ("Active" or "Dead" or "Removed") ||
            speciesId == Guid.Empty ||
            varietyId == Guid.Empty ||
            spotId == Guid.Empty)
        {
            return false;
        }

        if (plantedAt.HasValue &&
            plantedAt.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return false;
        }

        return true;
    }

    private static string? NormalizeCode(string? code)
    {
        return code?.Trim().ToUpperInvariant();
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

    private static bool IsConstraintViolation(
        DbUpdateException exception)
    {
        return exception.InnerException is SqlException
        {
            Number: 2601 or 2627 or 547
        };
    }

    private static AuditLog CreateAudit(
        Guid actorId,
        Tree tree,
        string action,
        string? oldValues,
        DateTime occurredAt)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorId,
            GardenId = tree.GardenId,
            Action = action,
            EntityType = "Tree",
            EntityId = tree.Id.ToString(),
            OldValues = oldValues,
            NewValues = SerializeTreeValues(tree),
            OccurredAt = occurredAt
        };
    }

    private static string SerializeTreeValues(Tree tree)
    {
        return JsonSerializer.Serialize(new
        {
            tree.Code,
            tree.Name,
            tree.PlantSpeciesId,
            tree.TreeVarietyId,
            tree.PlantingSpotId,
            tree.PlantedAt,
            tree.Status,
            tree.Notes
        });
    }

    private static TreeResponse ToResponse(Tree tree)
    {
        return new TreeResponse(
            tree.Id,
            tree.GardenId,
            tree.PlantSpeciesId,
            tree.TreeVarietyId,
            tree.PlantingSpotId,
            tree.Code,
            tree.Name,
            tree.PlantedAt,
            tree.Status,
            tree.Notes,
            tree.CreatedAt,
            tree.UpdatedAt,
            Convert.ToBase64String(tree.RowVersion));
    }
}
