
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;

public interface IPasswordRepository
{
    Task<User?> FindUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task SavePasswordChangeAsync(
        User user,
        AuditLog auditLog,
        PasswordResetRecord? resetRecord,
        DateTime changedAt,
        CancellationToken cancellationToken);
}
