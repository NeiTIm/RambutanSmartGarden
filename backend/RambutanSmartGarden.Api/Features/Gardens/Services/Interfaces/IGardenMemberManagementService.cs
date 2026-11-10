
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

public interface IGardenMemberManagementService
{
    Task<GardenMemberMutationStatus> ChangeRoleAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        UpdateGardenMemberRoleRequest request,
        CancellationToken cancellationToken);

    Task<GardenMemberMutationStatus> RemoveMemberAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        CancellationToken cancellationToken);
}
