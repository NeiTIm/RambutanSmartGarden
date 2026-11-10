
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", table =>
        {
            table.HasCheckConstraint(
                "CK_AuditLogs_OldValues_IsJson",
                "[OldValues] IS NULL OR ISJSON([OldValues]) = 1");

            table.HasCheckConstraint(
                "CK_AuditLogs_NewValues_IsJson",
                "[NewValues] IS NULL OR ISJSON([NewValues]) = 1");
        });

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Id)
            .ValueGeneratedNever();

        builder.Property(log => log.Action)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(log => log.EntityType)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(log => log.EntityId)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(log => log.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(log => log.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(log => log.Description)
            .HasMaxLength(1000);

        builder.Property(log => log.IpAddress)
            .HasMaxLength(45)
            .IsUnicode(false);

        builder.Property(log => log.CorrelationId)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(log => log.OccurredAt)
            .HasPrecision(3)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(log => new
        {
            log.ActorUserId,
            log.OccurredAt
        })
        .HasDatabaseName("IX_AuditLogs_ActorUserId_OccurredAt");

        builder.HasIndex(log => new
        {
            log.GardenId,
            log.OccurredAt
        })
        .HasDatabaseName("IX_AuditLogs_GardenId_OccurredAt");

        builder.HasIndex(log => new
        {
            log.EntityType,
            log.EntityId,
            log.OccurredAt
        })
        .HasDatabaseName("IX_AuditLogs_EntityType_EntityId_OccurredAt");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(log => log.ActorUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Garden>()
            .WithMany()
            .HasForeignKey(log => log.GardenId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
