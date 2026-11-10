
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Plants.Models;
using RambutanSmartGarden.Api.Features.Trees.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class TreeConfiguration
    : IEntityTypeConfiguration<Tree>
{
    public void Configure(EntityTypeBuilder<Tree> builder)
    {
        builder.ToTable("Trees", table =>
        {
            table.HasCheckConstraint(
                "CK_Trees_Status",
                "[Status] IN (N'Active', N'Dead', N'Removed')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150);

        builder.Property(x => x.PlantedAt)
            .HasColumnType("date");

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        // Mã cây duy nhất trong mỗi Garden.
        builder.HasIndex(x => new
        {
            x.GardenId,
            x.Code
        })
        .IsUnique()
        .HasDatabaseName("UX_Trees_GardenId_Code");

        // Chỉ một cây Active tại một vị trí.
        builder.HasIndex(x => x.PlantingSpotId)
            .IsUnique()
            .HasFilter(
                "[PlantingSpotId] IS NOT NULL " +
                "AND [Status] = N'Active'")
            .HasDatabaseName(
                "UX_Trees_Active_PlantingSpotId");

        // Garden phải tồn tại.
        builder.HasOne<Garden>()
            .WithMany()
            .HasForeignKey(x => x.GardenId)
            .OnDelete(DeleteBehavior.NoAction);

        // Loài cây phải tồn tại.
        builder.HasOne<PlantSpecies>()
            .WithMany()
            .HasForeignKey(x => x.PlantSpeciesId)
            .OnDelete(DeleteBehavior.NoAction);

        // Giống cây phải thuộc đúng loài.
        builder.HasOne<TreeVariety>()
            .WithMany()
            .HasForeignKey(x => new
            {
                x.PlantSpeciesId,
                x.TreeVarietyId
            })
            .HasPrincipalKey(x => new
            {
                x.PlantSpeciesId,
                x.Id
            })
            .OnDelete(DeleteBehavior.NoAction);

        // Vị trí phải thuộc đúng Garden.
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
    }
}
