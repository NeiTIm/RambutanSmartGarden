
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Trees.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class TreeReplacementConfiguration
    : IEntityTypeConfiguration<TreeReplacement>
{
    public void Configure(
        EntityTypeBuilder<TreeReplacement> builder)
    {
        builder.ToTable("TreeReplacements", table =>
        {
            table.HasCheckConstraint(
                "CK_TreeReplacements_OldStatus",
                "[OldTreePreviousStatus] IN (N'Active', N'Dead')");

            table.HasCheckConstraint(
                "CK_TreeReplacements_DifferentTrees",
                "[OldTreeId] <> [NewTreeId]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.OldTreePreviousStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ReplacedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => x.OldTreeId)
            .IsUnique()
            .HasDatabaseName(
                "UX_TreeReplacements_OldTreeId");

        builder.HasIndex(x => x.NewTreeId)
            .IsUnique()
            .HasDatabaseName(
                "UX_TreeReplacements_NewTreeId");

        builder.HasIndex(x => new
        {
            x.GardenId,
            x.ReplacedAt
        });

        builder.HasOne<Garden>()
            .WithMany()
            .HasForeignKey(x => x.GardenId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Tree>()
            .WithMany()
            .HasForeignKey(x => new
            {
                x.GardenId,
                x.OldTreeId
            })
            .HasPrincipalKey(x => new
            {
                x.GardenId,
                x.Id
            })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Tree>()
            .WithMany()
            .HasForeignKey(x => new
            {
                x.GardenId,
                x.NewTreeId
            })
            .HasPrincipalKey(x => new
            {
                x.GardenId,
                x.Id
            })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<PlantingSpot>()
            .WithMany()
            .HasForeignKey(x => new
            {
                x.GardenId,
                x.PlantingSpotId
            })
            .HasPrincipalKey(x => new
            {
                x.GardenId,
                x.Id
            })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.PerformedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
