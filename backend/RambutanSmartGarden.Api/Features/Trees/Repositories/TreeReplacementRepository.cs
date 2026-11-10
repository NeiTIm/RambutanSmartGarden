
using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Trees.DTOs;
using RambutanSmartGarden.Api.Features.Trees.Models;
using RambutanSmartGarden.Api.Features.Trees.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Trees.Repositories;

public sealed class TreeReplacementRepository
    : ITreeReplacementRepository
{
    private readonly AppDbContext _dbContext;

    public TreeReplacementRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TreeReplacementResponse?> FindAsync(
        Guid gardenId,
        Guid replacementId,
        CancellationToken cancellationToken)
    {
        var item = await (
            from replacement in _dbContext.TreeReplacements
                .AsNoTracking()
            join tree in _dbContext.Trees.AsNoTracking()
                on replacement.NewTreeId equals tree.Id
            where replacement.GardenId == gardenId
                && replacement.Id == replacementId
            select new { Replacement = replacement, Tree = tree }
        ).FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? null
            : ToResponse(item.Replacement, item.Tree);
    }

    public async Task<List<TreeReplacementResponse>> GetAllAsync(
        Guid gardenId,
        CancellationToken cancellationToken)
    {
        var items = await (
            from replacement in _dbContext.TreeReplacements
                .AsNoTracking()
            join tree in _dbContext.Trees.AsNoTracking()
                on replacement.NewTreeId equals tree.Id
            where replacement.GardenId == gardenId
            orderby replacement.ReplacedAt descending,
                    replacement.Id
            select new { Replacement = replacement, Tree = tree }
        ).ToListAsync(cancellationToken);

        return items
            .Select(x => ToResponse(x.Replacement, x.Tree))
            .ToList();
    }

    public async Task<ReplaceTreeResult> ReplaceAsync(
        Guid gardenId,
        Guid oldTreeId,
        Guid actorUserId,
        ReplaceTreeRequest request,
        byte[] originalRowVersion,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            var oldTree = await _dbContext.Trees
                .FirstOrDefaultAsync(
                    x => x.Id == oldTreeId &&
                         x.GardenId == gardenId,
                    cancellationToken);

            if (oldTree is null)
            {
                return new(ReplaceTreeStatus.NotFound);
            }

            if (oldTree.Status == "Removed")
            {
                return new(ReplaceTreeStatus.AlreadyReplaced);
            }

            if (oldTree.Status is not ("Active" or "Dead"))
            {
                return new(ReplaceTreeStatus.InvalidRequest);
            }

            if (!oldTree.RowVersion.SequenceEqual(
                    originalRowVersion))
            {
                return new(ReplaceTreeStatus.ConcurrencyConflict);
            }

            if (oldTree.PlantingSpotId is not Guid spotId)
            {
                return new(ReplaceTreeStatus.MissingPlantingSpot);
            }

            var alreadyReplaced = await _dbContext.TreeReplacements
                .AnyAsync(
                    x => x.OldTreeId == oldTreeId,
                    cancellationToken);

            if (alreadyReplaced)
            {
                return new(ReplaceTreeStatus.AlreadyReplaced);
            }

            var speciesExists = await _dbContext.PlantSpecies
                .AnyAsync(
                    x => x.Id == request.NewPlantSpeciesId,
                    cancellationToken);

            if (!speciesExists)
            {
                return new(ReplaceTreeStatus.InvalidSpecies);
            }

            if (request.NewTreeVarietyId is Guid varietyId)
            {
                var varietyIsValid = await _dbContext.TreeVarieties
                    .AnyAsync(
                        x => x.Id == varietyId &&
                             x.PlantSpeciesId == request.NewPlantSpeciesId,
                        cancellationToken);

                if (!varietyIsValid)
                {
                    return new(ReplaceTreeStatus.InvalidVariety);
                }
            }

            var newCode = request.NewTreeCode.Trim()
                .ToUpperInvariant();

            var duplicateCode = await _dbContext.Trees
                .AnyAsync(
                    x => x.GardenId == gardenId &&
                         x.Code == newCode,
                    cancellationToken);

            if (duplicateCode)
            {
                return new(ReplaceTreeStatus.DuplicateCode);
            }

            // Một cây Active khác không được chiếm
            // vị trí cần thay thế.
            var spotOccupied = await _dbContext.Trees
                .AnyAsync(
                    x => x.PlantingSpotId == spotId &&
                         x.Status == "Active" &&
                         x.Id != oldTreeId,
                    cancellationToken);

            if (spotOccupied)
            {
                return new(ReplaceTreeStatus.OccupiedPlantingSpot);
            }

            var now = DateTime.UtcNow;
            var previousStatus = oldTree.Status;

            // Bước 1: giải phóng vị trí Active.
            oldTree.Status = "Removed";
            oldTree.UpdatedAt = now;

            _dbContext.Entry(oldTree)
                .Property(x => x.RowVersion)
                .OriginalValue = originalRowVersion;

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            // Bước 2: tạo cây mới tại cùng vị trí.
            var newTree = new Tree
            {
                Id = Guid.NewGuid(),
                GardenId = gardenId,
                PlantSpeciesId = request.NewPlantSpeciesId,
                TreeVarietyId = request.NewTreeVarietyId,
                PlantingSpotId = spotId,
                Code = newCode,
                Name = NormalizeOptional(request.NewTreeName),
                PlantedAt = request.NewTreePlantedAt,
                Status = "Active",
                Notes = NormalizeOptional(request.NewTreeNotes),
                CreatedAt = now
            };

            var replacement = new TreeReplacement
            {
                Id = Guid.NewGuid(),
                GardenId = gardenId,
                OldTreeId = oldTree.Id,
                NewTreeId = newTree.Id,
                PlantingSpotId = spotId,
                OldTreePreviousStatus = previousStatus,
                Reason = request.Reason.Trim(),
                ReplacedAt = now,
                PerformedByUserId = actorUserId
            };

            var audit = new AuditLog
            {
                Id = Guid.NewGuid(),
                ActorUserId = actorUserId,
                GardenId = gardenId,
                Action = "Tree.Replaced",
                EntityType = "TreeReplacement",
                EntityId = replacement.Id.ToString(),
                OldValues = JsonSerializer.Serialize(new
                {
                    TreeId = oldTree.Id,
                    oldTree.Code,
                    Status = previousStatus
                }),
                NewValues = JsonSerializer.Serialize(new
                {
                    NewTreeId = newTree.Id,
                    NewTreeCode = newTree.Code,
                    replacement.PlantingSpotId,
                    replacement.Reason
                }),
                OccurredAt = now
            };

            _dbContext.Trees.Add(newTree);
            _dbContext.TreeReplacements.Add(replacement);
            _dbContext.AuditLogs.Add(audit);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new(
                ReplaceTreeStatus.Succeeded,
                ToResponse(replacement, newTree));
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(ReplaceTreeStatus.ConcurrencyConflict);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException
            {
                Number: 2601 or 2627 or 547
            })
        {
            return new(ReplaceTreeStatus.DataConflict);
        }
        catch (SqlException exception)
            when (exception.Number == 1205)
        {
            return new(ReplaceTreeStatus.ConcurrencyConflict);
        }
    }

    private static TreeReplacementResponse ToResponse(
        TreeReplacement replacement,
        Tree newTree)
    {
        return new TreeReplacementResponse(
            replacement.Id,
            replacement.GardenId,
            replacement.OldTreeId,
            replacement.NewTreeId,
            replacement.PlantingSpotId,
            replacement.OldTreePreviousStatus,
            replacement.Reason,
            replacement.ReplacedAt,
            replacement.PerformedByUserId,
            Convert.ToBase64String(replacement.RowVersion),
            new TreeResponse(
                newTree.Id,
                newTree.GardenId,
                newTree.PlantSpeciesId,
                newTree.TreeVarietyId,
                newTree.PlantingSpotId,
                newTree.Code,
                newTree.Name,
                newTree.PlantedAt,
                newTree.Status,
                newTree.Notes,
                newTree.CreatedAt,
                newTree.UpdatedAt,
                Convert.ToBase64String(newTree.RowVersion)));
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
