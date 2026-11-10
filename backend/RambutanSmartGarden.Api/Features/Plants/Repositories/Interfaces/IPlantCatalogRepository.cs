
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Plants.Models;

namespace RambutanSmartGarden.Api.Features.Plants.Repositories.Interfaces;

public interface IPlantCatalogRepository
{
    Task<List<PlantSpecies>> GetSpeciesAsync(
        CancellationToken cancellationToken);

    Task<PlantSpecies?> FindSpeciesAsync(
        Guid speciesId,
        CancellationToken cancellationToken);

    Task<List<TreeVariety>> GetVarietiesAsync(
        Guid speciesId,
        CancellationToken cancellationToken);

    void AddSpecies(PlantSpecies species);

    void AddVariety(TreeVariety variety);

    void AddAuditLog(AuditLog auditLog);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
