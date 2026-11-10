
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Plants.DTOs;
using RambutanSmartGarden.Api.Features.Plants.Models;
using RambutanSmartGarden.Api.Features.Plants.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Plants.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Plants.Services;

public sealed class PlantCatalogService : IPlantCatalogService
{
    private readonly IPlantCatalogRepository _repository;

    public PlantCatalogService(IPlantCatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PlantSpeciesResponse>> GetSpeciesAsync(
        CancellationToken cancellationToken)
    {
        var species = await _repository.GetSpeciesAsync(
            cancellationToken);

        return species.Select(ToSpeciesResponse).ToList();
    }

    public async Task<PlantSpeciesResponse?> FindSpeciesAsync(
        Guid speciesId,
        CancellationToken cancellationToken)
    {
        var species = await _repository.FindSpeciesAsync(
            speciesId,
            cancellationToken);

        return species is null
            ? null
            : ToSpeciesResponse(species);
    }

    public async Task<CatalogResult<PlantSpeciesResponse>>
        CreateSpeciesAsync(
            Guid actorUserId,
            CreatePlantSpeciesRequest request,
            CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();
        var scientificName = NormalizeOptional(
            request.ScientificName);
        var description = NormalizeOptional(
            request.Description);

        if (actorUserId == Guid.Empty ||
            !ValidText(name, 150) ||
            scientificName?.Length > 200 ||
            description?.Length > 2000)
        {
            return new(PlantCatalogStatus.InvalidRequest);
        }

        var now = DateTime.UtcNow;

        var species = new PlantSpecies
        {
            Id = Guid.NewGuid(),
            Name = name!,
            ScientificName = scientificName,
            Description = description,
            CreatedAt = now
        };

        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            Action = "PlantSpecies.Created",
            EntityType = "PlantSpecies",
            EntityId = species.Id.ToString(),
            NewValues = JsonSerializer.Serialize(new
            {
                species.Name,
                species.ScientificName
            }),
            OccurredAt = now
        };

        _repository.AddSpecies(species);
        _repository.AddAuditLog(audit);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicate(ex))
        {
            return new(PlantCatalogStatus.DuplicateName);
        }

        return new(
            PlantCatalogStatus.Succeeded,
            ToSpeciesResponse(species));
    }

    public async Task<CatalogResult<List<TreeVarietyResponse>>>
        GetVarietiesAsync(
            Guid speciesId,
            CancellationToken cancellationToken)
    {
        var species = await _repository.FindSpeciesAsync(
            speciesId,
            cancellationToken);

        if (species is null)
        {
            return new(PlantCatalogStatus.NotFound);
        }

        var varieties = await _repository.GetVarietiesAsync(
            speciesId,
            cancellationToken);

        return new(
            PlantCatalogStatus.Succeeded,
            varieties.Select(ToVarietyResponse).ToList());
    }

    public async Task<CatalogResult<TreeVarietyResponse>>
        CreateVarietyAsync(
            Guid actorUserId,
            Guid speciesId,
            CreateTreeVarietyRequest request,
            CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();
        var description = NormalizeOptional(
            request.Description);

        if (actorUserId == Guid.Empty ||
            speciesId == Guid.Empty ||
            !ValidText(name, 150) ||
            description?.Length > 2000)
        {
            return new(PlantCatalogStatus.InvalidRequest);
        }

        var species = await _repository.FindSpeciesAsync(
            speciesId,
            cancellationToken);

        if (species is null)
        {
            return new(PlantCatalogStatus.NotFound);
        }

        var now = DateTime.UtcNow;

        var variety = new TreeVariety
        {
            Id = Guid.NewGuid(),
            PlantSpeciesId = speciesId,
            Name = name!,
            Description = description,
            CreatedAt = now
        };

        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            Action = "TreeVariety.Created",
            EntityType = "TreeVariety",
            EntityId = variety.Id.ToString(),
            NewValues = JsonSerializer.Serialize(new
            {
                variety.PlantSpeciesId,
                variety.Name
            }),
            OccurredAt = now
        };

        _repository.AddVariety(variety);
        _repository.AddAuditLog(audit);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicate(ex))
        {
            return new(PlantCatalogStatus.DuplicateName);
        }

        return new(
            PlantCatalogStatus.Succeeded,
            ToVarietyResponse(variety));
    }

    private static PlantSpeciesResponse ToSpeciesResponse(
        PlantSpecies species)
    {
        return new(
            species.Id,
            species.Name,
            species.ScientificName,
            species.Description,
            species.CreatedAt,
            Convert.ToBase64String(species.RowVersion));
    }

    private static TreeVarietyResponse ToVarietyResponse(
        TreeVariety variety)
    {
        return new(
            variety.Id,
            variety.PlantSpeciesId,
            variety.Name,
            variety.Description,
            variety.CreatedAt,
            Convert.ToBase64String(variety.RowVersion));
    }

    private static bool ValidText(
        string? value,
        int maxLength)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length <= maxLength;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static bool IsDuplicate(DbUpdateException ex)
    {
        return ex.InnerException is SqlException
        {
            Number: 2601 or 2627
        };
    }
}
