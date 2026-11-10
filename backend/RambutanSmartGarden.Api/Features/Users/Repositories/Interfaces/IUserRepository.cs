
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Features.Users.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> UsernameExistsAsync(
        string normalizedUsername,
        CancellationToken cancellationToken);

    Task<bool> PhoneExistsAsync(
        string normalizedPhoneNumber,
        CancellationToken cancellationToken);

    void AddUser(User user);

    void AddAuditLog(AuditLog auditLog);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
