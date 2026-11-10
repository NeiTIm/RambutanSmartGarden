
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Auth.Models;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Features.Gardens.Areas.Models;
namespace RambutanSmartGarden.Api.Infrastructure.Persistence;

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
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
