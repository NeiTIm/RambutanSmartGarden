
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Auth.Repositories.Interfaces;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Features.Auth.Repositories;

public sealed class PasswordRepository : IPasswordRepository
{
    private readonly AppDbContext _dbContext;

    public PasswordRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> FindUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken);
    }

    public async Task SavePasswordChangeAsync(
        User user,
        AuditLog auditLog,
        PasswordResetRecord? resetRecord,
        DateTime changedAt,
        CancellationToken cancellationToken)
    {
        // Một transaction bao gồm:
        // 1. Đổi mật khẩu
        // 2. Ghi lịch sử
        // 3. Thu hồi toàn bộ phiên đăng nhập

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        user.UpdatedAt = changedAt;

        _dbContext.AuditLogs.Add(auditLog);

        if (resetRecord is not null)
        {
            _dbContext.PasswordResetRecords.Add(resetRecord);
        }

        // RowVersion của User sẽ được EF Core kiểm tra.
        await _dbContext.SaveChangesAsync(cancellationToken);

        // ExecuteUpdateAsync tác động trực tiếp vào SQL Server.
        // Phải nằm trong cùng transaction với SaveChangesAsync.
        await _dbContext.UserSessions
            .Where(session =>
                session.UserId == user.Id &&
                session.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        session => session.RevokedAt,
                        (DateTime?)changedAt)
                    .SetProperty(
                        session => session.RevokedReason,
                        "PasswordChanged")
                    .SetProperty(
                        session => session.RefreshTokenHash,
                        (string?)null),
                cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
