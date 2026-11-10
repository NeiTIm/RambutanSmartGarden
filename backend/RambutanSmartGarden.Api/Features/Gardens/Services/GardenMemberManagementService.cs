
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Services;

public sealed class GardenMemberManagementService
    : IGardenMemberManagementService
{
    private readonly IGardenMemberManagementRepository _repository;

    public GardenMemberManagementService(
        IGardenMemberManagementRepository repository)
    {
        _repository = repository;
    }

    public Task<GardenMemberMutationStatus> ChangeRoleAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        UpdateGardenMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        if (gardenId == Guid.Empty ||
            targetUserId == Guid.Empty ||
            actorUserId == Guid.Empty)
        {
            return Task.FromResult(
                GardenMemberMutationStatus.NotFound);
        }

        if (request.Role is not ("Admin" or "Member" or "Viewer"))
        {
            return Task.FromResult(
                GardenMemberMutationStatus.InvalidRole);
        }

        return _repository.ChangeRoleAsync(
            actorUserId,
            gardenId,
            targetUserId,
            request.Role,
            cancellationToken);
    }

    public Task<GardenMemberMutationStatus> RemoveMemberAsync(
        Guid actorUserId,
        Guid gardenId,
        Guid targetUserId,
        CancellationToken cancellationToken)
    {
        if (gardenId == Guid.Empty ||
            targetUserId == Guid.Empty ||
            actorUserId == Guid.Empty)
        {
            return Task.FromResult(
                GardenMemberMutationStatus.NotFound);
        }

        return _repository.RemoveMemberAsync(
            actorUserId,
            gardenId,
            targetUserId,
            cancellationToken);
    }
}
