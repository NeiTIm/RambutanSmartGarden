
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Users.DTOs;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Features.Users.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Users.Services;

public sealed class AdminUserService : IAdminUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AdminUserService(
        IUserRepository repository,
        IPasswordHasher<User> passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateUserResult> CreateAsync(
        CreateUserRequest request,
        Guid adminUserId,
        CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var displayName = request.DisplayName.Trim();

        if (!Regex.IsMatch(
                username,
                @"^[a-zA-Z0-9._-]{3,50}$") ||
            string.IsNullOrWhiteSpace(displayName) ||
            displayName.Length > 100 ||
            request.Password.Length is < 12 or > 128)
        {
            throw new ArgumentException(
                "Thông tin tạo tài khoản không hợp lệ.");
        }

        var normalizedUsername = username.ToUpperInvariant();

        string? phoneNumber = null;
        string? normalizedPhoneNumber = null;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            phoneNumber = request.PhoneNumber.Trim();

            if (!Regex.IsMatch(
                    phoneNumber,
                    @"^(0[35789]\d{8}|\+84[35789]\d{8})$"))
            {
                throw new ArgumentException(
                    "Số điện thoại không hợp lệ.");
            }

            normalizedPhoneNumber =
                phoneNumber.StartsWith("0", StringComparison.Ordinal)
                    ? "+84" + phoneNumber[1..]
                    : phoneNumber;
        }

        if (await _repository.UsernameExistsAsync(
                normalizedUsername,
                cancellationToken))
        {
            return new CreateUserResult(
                null,
                "Tên đăng nhập đã tồn tại.");
        }

        if (normalizedPhoneNumber is not null &&
            await _repository.PhoneExistsAsync(
                normalizedPhoneNumber,
                cancellationToken))
        {
            return new CreateUserResult(
                null,
                "Số điện thoại đã được sử dụng.");
        }

        var now = DateTime.UtcNow;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            NormalizedUsername = normalizedUsername,
            PhoneNumber = phoneNumber,
            NormalizedPhoneNumber = normalizedPhoneNumber,
            DisplayName = displayName,
            IsSystemAdmin = false,
            IsActive = true,
            CreatedAt = now
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = adminUserId,
            Action = "User.Created",
            EntityType = "User",
            EntityId = user.Id.ToString(),
            NewValues = JsonSerializer.Serialize(new
            {
                user.Id,
                user.Username,
                user.DisplayName,
                user.IsActive
            }),
            OccurredAt = now
        };

        _repository.AddUser(user);
        _repository.AddAuditLog(auditLog);

        try
        {
            await _repository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException
            {
                Number: 2601 or 2627
            })
        {
            return new CreateUserResult(
                null,
                "Tên đăng nhập hoặc số điện thoại đã tồn tại.");
        }

        var response = new CreateUserResponse(
            user.Id,
            user.Username,
            user.DisplayName,
            user.PhoneNumber,
            user.IsActive,
            user.CreatedAt);

        return new CreateUserResult(response, null);
    }
}
