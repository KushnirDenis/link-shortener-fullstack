using System.Collections.Immutable;
using LinkShortener.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.DAL;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<Click> Clicks { get; set; }

    public AppDbContext()
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasAlternateKey(u => u.Email);
        modelBuilder.Entity<Link>()
            .HasIndex(l => l.ShortLink)
            .IsUnique();
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamps();
        return await base.SaveChangesAsync();
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && x.State == EntityState.Added);

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAt = now;
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("PGSQL_STRING") + "Database=LinkShortenerDB;";
        optionsBuilder.UseNpgsql(connectionString);
    }
}