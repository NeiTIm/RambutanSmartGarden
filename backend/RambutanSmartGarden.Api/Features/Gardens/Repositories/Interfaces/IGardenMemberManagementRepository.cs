
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;

public interface IGardenMemberManagementRepository
{
    Task<GardenMemberMutationStatus> ChangeRoleAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        string newRole,
        CancellationToken cancellationToken);

    Task<GardenMemberMutationStatus> RemoveMemberAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        CancellationToken cancellationToken);
}
