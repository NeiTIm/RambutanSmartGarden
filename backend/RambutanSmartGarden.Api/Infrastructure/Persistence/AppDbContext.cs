
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Users.Models;

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

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
