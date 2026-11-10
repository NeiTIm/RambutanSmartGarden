
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Plants.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class TreeVarietyConfiguration
    : IEntityTypeConfiguration<TreeVariety>
{
    public void Configure(EntityTypeBuilder<TreeVariety> builder)
    {
        builder.ToTable("TreeVarieties");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new
        {
            x.PlantSpeciesId,
            x.Name
        })
        .IsUnique()
        .HasDatabaseName(
            "UX_TreeVarieties_PlantSpeciesId_Name");

        builder.HasOne<PlantSpecies>()
            .WithMany()
            .HasForeignKey(x => x.PlantSpeciesId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
