
using RambutanSmartGarden.Api.Features.Auth.DTOs;

namespace RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;

public enum PasswordActionResult
{
    Succeeded,
    UserNotFound,
    InvalidCurrentPassword,
    PasswordUnchanged,
    InvalidNewPassword,
    InvalidTarget,
    ConcurrencyConflict
}

public interface IPasswordService
{
    Task<PasswordActionResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken);

    Task<PasswordActionResult> AdminResetPasswordAsync(
        Guid adminUserId,
        Guid targetUserId,
        AdminResetPasswordRequest request,
        CancellationToken cancellationToken);
}
