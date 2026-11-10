
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Plants.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class PlantSpeciesConfiguration
    : IEntityTypeConfiguration<PlantSpecies>
{
    public void Configure(EntityTypeBuilder<PlantSpecies> builder)
    {
        builder.ToTable("PlantSpecies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ScientificName)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("UX_PlantSpecies_Name");
    }
}
