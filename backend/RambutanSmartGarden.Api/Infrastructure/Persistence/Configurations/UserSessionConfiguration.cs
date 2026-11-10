
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class UserSessionConfiguration
    : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions", table =>
        {
            table.HasCheckConstraint(
                "CK_UserSessions_ExpiresAt",
                "[ExpiresAt] > [CreatedAt]");

            table.HasCheckConstraint(
                "CK_UserSessions_RevokedAt",
                "[RevokedAt] IS NULL OR [RevokedAt] >= [CreatedAt]");
        });

        builder.HasKey(session => session.Id);

        builder.Property(session => session.Id)
            .ValueGeneratedNever();

        builder.Property(session => session.RefreshTokenHash)
            .HasMaxLength(128)
            .IsUnicode(false);

        builder.Property(session => session.DeviceName)
            .HasMaxLength(200);

        builder.Property(session => session.IpAddress)
            .HasMaxLength(45)
            .IsUnicode(false);

        builder.Property(session => session.UserAgent)
            .HasMaxLength(1000);

        builder.Property(session => session.RevokedReason)
            .HasMaxLength(300);

        builder.Property(session => session.CreatedAt)
            .HasPrecision(3);

        builder.Property(session => session.ExpiresAt)
            .HasPrecision(3);

        builder.Property(session => session.LastSeenAt)
            .HasPrecision(3);

        builder.Property(session => session.RevokedAt)
            .HasPrecision(3);

        builder.Property(session => session.RowVersion)
            .IsRowVersion();

        builder.HasIndex(session => session.UserId)
            .HasDatabaseName("IX_UserSessions_UserId");

        builder.HasIndex(session => session.ExpiresAt)
            .HasDatabaseName("IX_UserSessions_ExpiresAt");

        builder.HasIndex(session => session.RefreshTokenHash)
            .IsUnique()
            .HasFilter("[RefreshTokenHash] IS NOT NULL")
            .HasDatabaseName("UX_UserSessions_RefreshTokenHash");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
