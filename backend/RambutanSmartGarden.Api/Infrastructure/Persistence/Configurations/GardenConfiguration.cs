
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class GardenConfiguration : IEntityTypeConfiguration<Garden>
{
    public void Configure(EntityTypeBuilder<Garden> builder)
    {
        builder.ToTable("Gardens", table =>
        {
            table.HasCheckConstraint(
                "CK_Gardens_AreaSquareMeters",
                "[AreaSquareMeters] IS NULL OR [AreaSquareMeters] > 0");
        });

        builder.HasKey(garden => garden.Id);

        builder.Property(garden => garden.Id)
            .ValueGeneratedNever();

        builder.Property(garden => garden.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(garden => garden.Description)
            .HasMaxLength(2000);

        builder.Property(garden => garden.LocationDescription)
            .HasMaxLength(500);

        builder.Property(garden => garden.AreaSquareMeters)
            .HasPrecision(18, 2);

        builder.Property(garden => garden.CreatedAt)
            .HasPrecision(3)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(garden => garden.UpdatedAt)
            .HasPrecision(3);

        builder.Property(garden => garden.RowVersion)
            .IsRowVersion();
    }
}
