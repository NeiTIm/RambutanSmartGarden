
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Features.Auth.Services;

public sealed class PasswordService : IPasswordService
{
    private readonly IPasswordRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordService(
        IPasswordRepository repository,
        IPasswordHasher<User> passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<PasswordActionResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsValidNewPassword(request.NewPassword))
        {
            return PasswordActionResult.InvalidNewPassword;
        }

        var user = await _repository.FindUserAsync(
            userId,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            return PasswordActionResult.UserNotFound;
        }

        var currentPasswordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword);

        if (currentPasswordResult ==
            PasswordVerificationResult.Failed)
        {
            return PasswordActionResult.InvalidCurrentPassword;
        }

        if (string.Equals(
                request.CurrentPassword,
                request.NewPassword,
                StringComparison.Ordinal))
        {
            return PasswordActionResult.PasswordUnchanged;
        }

        var now = DateTime.UtcNow;

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.NewPassword);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = userId,
            Action = "User.PasswordChanged",
            EntityType = "User",
            EntityId = userId.ToString(),
            Description = "Người dùng tự đổi mật khẩu.",
            OccurredAt = now
        };

        try
        {
            await _repository.SavePasswordChangeAsync(
                user,
                auditLog,
                null,
                now,
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return PasswordActionResult.ConcurrencyConflict;
        }

        return PasswordActionResult.Succeeded;
    }

    public async Task<PasswordActionResult> AdminResetPasswordAsync(
        Guid adminUserId,
        Guid targetUserId,
        AdminResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsValidNewPassword(request.NewPassword))
        {
            return PasswordActionResult.InvalidNewPassword;
        }

        if (adminUserId == targetUserId)
        {
            return PasswordActionResult.InvalidTarget;
        }

        var targetUser = await _repository.FindUserAsync(
            targetUserId,
            cancellationToken);

        // API này chỉ đặt lại mật khẩu cho thành viên thông thường.
        if (targetUser is null || targetUser.IsSystemAdmin)
        {
            return PasswordActionResult.UserNotFound;
        }

        var now = DateTime.UtcNow;

        targetUser.PasswordHash =
            _passwordHasher.HashPassword(
                targetUser,
                request.NewPassword);

        var reason = string.IsNullOrWhiteSpace(request.Reason)
            ? null
            : request.Reason.Trim();

        var resetRecord = new PasswordResetRecord
        {
            Id = Guid.NewGuid(),
            UserId = targetUser.Id,
            PerformedByAdminId = adminUserId,
            ResetAt = now,
            Reason = reason
        };

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = adminUserId,
            Action = "User.PasswordResetByAdmin",
            EntityType = "User",
            EntityId = targetUser.Id.ToString(),
            Description = "Quản trị viên đặt lại mật khẩu tài khoản.",
            OccurredAt = now
        };

        try
        {
            await _repository.SavePasswordChangeAsync(
                targetUser,
                auditLog,
                resetRecord,
                now,
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return PasswordActionResult.ConcurrencyConflict;
        }

        return PasswordActionResult.Succeeded;
    }

    private static bool IsValidNewPassword(string? password)
    {
        return !string.IsNullOrWhiteSpace(password) &&
               password.Length is >= 12 and <= 128;
    }
}
