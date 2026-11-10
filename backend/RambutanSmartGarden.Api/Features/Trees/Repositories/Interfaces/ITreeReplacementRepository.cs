
using RambutanSmartGarden.Api.Features.Trees.DTOs;
using RambutanSmartGarden.Api.Features.Trees.Services.Interfaces;
namespace RambutanSmartGarden.Api.Features.Trees.Repositories.Interfaces;

public interface ITreeReplacementRepository
{
    Task<TreeReplacementResponse?> FindAsync(
        Guid gardenId,
        Guid replacementId,
        CancellationToken cancellationToken);

    Task<List<TreeReplacementResponse>> GetAllAsync(
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<ReplaceTreeResult> ReplaceAsync(
        Guid gardenId,
        Guid oldTreeId,
        Guid actorUserId,
        ReplaceTreeRequest request,
        byte[] originalRowVersion,
        CancellationToken cancellationToken);
}
