
using System.Security.Claims;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;

namespace RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

public enum AddGardenMemberStatus
{
    Succeeded,
    Forbidden,
    InvalidRequest,
    UserNotFound,
    AlreadyMember
}

public sealed record AddGardenMemberResult(
    AddGardenMemberStatus Status,
    GardenMemberResponse? Member = null
);

public interface IGardenMemberService
{
    Task<List<GardenMemberResponse>?> GetMembersAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<AddGardenMemberResult> AddMemberAsync(
        ClaimsPrincipal principal,
        Guid gardenId,
        AddGardenMemberRequest request,
        CancellationToken cancellationToken);
}
