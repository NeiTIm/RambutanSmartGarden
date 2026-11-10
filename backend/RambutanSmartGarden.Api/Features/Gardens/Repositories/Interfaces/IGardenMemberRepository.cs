
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;

public interface IGardenMemberRepository
{
    Task<List<GardenMemberResponse>> GetMembersAsync(
        Guid gardenId,
        CancellationToken cancellationToken);

    Task<User?> FindActiveUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> MembershipExistsAsync(
        Guid gardenId,
        Guid userId,
        CancellationToken cancellationToken);

    void AddMembership(GardenMembership membership);

    void AddAuditLog(AuditLog auditLog);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
