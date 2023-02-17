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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("PGSQL_STRING") + "LinkShortenerDB";
        optionsBuilder.UseNpgsql(connectionString);
    }
    
}