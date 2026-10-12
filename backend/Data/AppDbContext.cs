using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Data;

// EF Core context: the bridge between the models and the
// managed PostgreSQL database hosted in the cloud (Lab 11).
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<MenuItem>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Category).HasMaxLength(40).IsRequired();
            e.Property(x => x.Price).HasPrecision(10, 2); // money = decimal
        });

        b.Entity<AppUser>(e =>
        {
            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Email).IsUnique(); // one account per email
        });

        // Seed the menu so a freshly migrated cloud database is not empty.
        b.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "Kottu Roti", Price = 750m, Category = "Mains", Available = true },
            new MenuItem { Id = 2, Name = "Fried Rice", Price = 850m, Category = "Mains", Available = true },
            new MenuItem { Id = 3, Name = "Watalappan", Price = 350m, Category = "Dessert", Available = true },
            new MenuItem { Id = 4, Name = "String Hoppers", Price = 420m, Category = "Mains", Available = true },
            new MenuItem { Id = 5, Name = "Faluda", Price = 400m, Category = "Drinks", Available = true }
        );
    }
}
