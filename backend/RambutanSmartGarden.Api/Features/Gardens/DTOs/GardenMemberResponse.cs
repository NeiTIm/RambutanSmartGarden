
namespace RambutanSmartGarden.Api.Features.Gardens.DTOs;

public sealed record GardenMemberResponse(
    Guid Id,
    Guid UserId,
    string Username,
    string DisplayName,
    string Role,
    DateTime JoinedAt
);
