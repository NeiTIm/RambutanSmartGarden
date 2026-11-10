
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class GardenMembershipConfiguration
    : IEntityTypeConfiguration<GardenMembership>
{
    public void Configure(EntityTypeBuilder<GardenMembership> builder)
    {
        builder.ToTable("GardenMemberships", table =>
        {
            table.HasCheckConstraint(
                "CK_GardenMemberships_Role",
                "[Role] IN ('Admin', 'Member', 'Viewer')");
        });

        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.Id)
            .ValueGeneratedNever();

        builder.Property(membership => membership.Role)
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(membership => membership.JoinedAt)
            .HasPrecision(3)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(membership => membership.RowVersion)
            .IsRowVersion();

        builder.HasIndex(membership => new
        {
            membership.GardenId,
            membership.UserId
        })
        .IsUnique()
        .HasDatabaseName("UX_GardenMemberships_GardenId_UserId");

        builder.HasIndex(membership => membership.UserId);

        builder.HasOne<Garden>()
            .WithMany()
            .HasForeignKey(membership => membership.GardenId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
