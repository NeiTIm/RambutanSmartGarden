
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class PasswordResetRecordConfiguration
    : IEntityTypeConfiguration<PasswordResetRecord>
{
    public void Configure(
        EntityTypeBuilder<PasswordResetRecord> builder)
    {
        builder.ToTable("PasswordResetRecords");

        builder.HasKey(record => record.Id);

        builder.Property(record => record.Id)
            .ValueGeneratedNever();

        builder.Property(record => record.ResetAt)
            .HasPrecision(3)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(record => record.Reason)
            .HasMaxLength(500);

        builder.Property(record => record.RowVersion)
            .IsRowVersion();

        builder.HasIndex(record => new
        {
            record.UserId,
            record.ResetAt
        })
        .HasDatabaseName(
            "IX_PasswordResetRecords_UserId_ResetAt");

        builder.HasIndex(record => record.PerformedByAdminId);

        // Tài khoản được đặt lại mật khẩu.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(record => record.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Admin thực hiện thao tác đặt lại mật khẩu.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(record => record.PerformedByAdminId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
