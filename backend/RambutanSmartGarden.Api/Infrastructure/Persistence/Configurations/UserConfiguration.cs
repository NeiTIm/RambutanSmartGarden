
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Tên bảng và khóa chính.
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        // Tên đăng nhập.
        builder.Property(user => user.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(user => user.NormalizedUsername)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(user => user.NormalizedUsername)
            .IsUnique()
            .HasDatabaseName("UX_Users_NormalizedUsername");

        // Số điện thoại.
        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(user => user.NormalizedPhoneNumber)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.HasIndex(user => user.NormalizedPhoneNumber)
            .IsUnique()
            .HasFilter("[NormalizedPhoneNumber] IS NOT NULL")
            .HasDatabaseName("UX_Users_NormalizedPhoneNumber");

        // Thông tin tài khoản.
        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        // Quyền và trạng thái.
        builder.Property(user => user.IsSystemAdmin)
            .HasDefaultValue(false);

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true);

        // Thời gian UTC.
        builder.Property(user => user.CreatedAt)
            .HasPrecision(3)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(user => user.UpdatedAt)
            .HasPrecision(3);

        // Optimistic concurrency.
        builder.Property(user => user.RowVersion)
            .IsRowVersion();
    }
}
