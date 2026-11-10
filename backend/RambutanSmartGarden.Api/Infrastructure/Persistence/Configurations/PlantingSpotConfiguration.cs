
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;
namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class PlantingSpotConfiguration
    : IEntityTypeConfiguration<PlantingSpot>
{
    public void Configure(EntityTypeBuilder<PlantingSpot> builder)
    {
        builder.ToTable("PlantingSpots", table =>
        {
            table.HasCheckConstraint(
     "CK_PlantingSpots_Coordinates",
     @"(
        ([Latitude] IS NULL AND [Longitude] IS NULL)
        OR
        (
            [Latitude] IS NOT NULL
            AND [Longitude] IS NOT NULL
            AND [Latitude] BETWEEN -90 AND 90
            AND [Longitude] BETWEEN -180 AND 180
        )
    )");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Latitude)
            .HasColumnType("decimal(9,6)");

        builder.Property(x => x.Longitude)
            .HasColumnType("decimal(9,6)");

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new
        {
            x.GardenId,
            x.Code
        })
        .IsUnique()
        .HasDatabaseName(
            "UX_PlantingSpots_GardenId_Code");

        builder.HasIndex(x => x.GardenAreaId);

        builder.HasOne<Garden>()
            .WithMany()
            .HasForeignKey(x => x.GardenId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<GardenArea>()
            .WithMany()
            .HasForeignKey(x => x.GardenAreaId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
