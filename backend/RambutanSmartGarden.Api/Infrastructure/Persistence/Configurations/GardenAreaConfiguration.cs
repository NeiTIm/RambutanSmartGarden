
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class GardenAreaConfiguration
    : IEntityTypeConfiguration<GardenArea>
{
    public void Configure(EntityTypeBuilder<GardenArea> builder)
    {
        builder.ToTable("GardenAreas", table =>
        {
            table.HasCheckConstraint(
                "CK_GardenAreas_AreaSquareMeters",
                "[AreaSquareMeters] IS NULL OR [AreaSquareMeters] > 0");
        });

        builder.HasKey(area => area.Id);

        builder.Property(area => area.Id)
            .ValueGeneratedNever();

        builder.Property(area => area.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(area => area.Description)
            .HasMaxLength(2000);

        builder.Property(area => area.AreaSquareMeters)
            .HasColumnType("decimal(18,2)");

        builder.Property(area => area.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(area => area.UpdatedAt)
            .HasColumnType("datetime2");

        builder.Property(area => area.RowVersion)
            .IsRowVersion();

        builder.HasIndex(area => new
        {
            area.GardenId,
            area.Name
        })
        .IsUnique()
        .HasDatabaseName("UX_GardenAreas_GardenId_Name");

        builder.HasOne<Garden>()
            .WithMany()
            .HasForeignKey(area => area.GardenId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
