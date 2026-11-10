
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;
using RambutanSmartGarden.Api.Features.Trees.Models;


namespace RambutanSmartGarden.Api.Infrastructure.Persistence;

using RambutanSmartGarden.Api.Features.Plants.Models;


public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();

    public DbSet<Garden> Gardens => Set<Garden>();

    public DbSet<GardenMembership> GardenMemberships
        => Set<GardenMembership>();
    public DbSet<UserSession> UserSessions
   => Set<UserSession>();

    public DbSet<PasswordResetRecord> PasswordResetRecords
        => Set<PasswordResetRecord>();
    public DbSet<AuditLog> AuditLogs
 => Set<AuditLog>();
    public DbSet<GardenArea> GardenAreas => Set<GardenArea>();

    public DbSet<PlantSpecies> PlantSpecies => Set<PlantSpecies>();

    public DbSet<TreeVariety> TreeVarieties => Set<TreeVariety>();
    public DbSet<PlantingSpot> PlantingSpots => Set<PlantingSpot>();
    public DbSet<Tree> Trees => Set<Tree>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
