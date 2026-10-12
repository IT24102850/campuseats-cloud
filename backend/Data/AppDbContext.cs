using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Data;

// EF Core context: the bridge between the models and the managed
// PostgreSQL database hosted in the cloud (Lab 11). The seed data is
// the same CampusEats menu and order list the Lab 02 frontend used
// against json-server, so the deployed app shows real, familiar data.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<MenuItem>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Category).HasMaxLength(40).IsRequired();
            e.Property(x => x.Outlet).HasMaxLength(60);
            e.Property(x => x.Price).HasPrecision(10, 2); // money = decimal
            e.HasIndex(x => x.Category); // the menu is filtered by category
        });

        b.Entity<Order>(e =>
        {
            e.Property(x => x.StudentId).HasMaxLength(20).IsRequired();
            e.Property(x => x.StudentName).HasMaxLength(80);
            e.Property(x => x.Items).HasMaxLength(400);
            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.Total).HasPrecision(10, 2);
            e.HasIndex(x => x.StudentId); // "my orders" is queried per student
        });

        b.Entity<AppUser>(e =>
        {
            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Email).IsUnique(); // one account per email
        });

        // Seed so a freshly migrated cloud database is never empty.
        b.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "Chicken Kottu", Category = "Rice & Kottu", Price = 650m, Outlet = "Main Canteen", IsFavourite = false, Available = true },
            new MenuItem { Id = 2, Name = "Vegetable Rice & Curry", Category = "Rice & Kottu", Price = 400m, Outlet = "Main Canteen", IsFavourite = true, Available = true },
            new MenuItem { Id = 3, Name = "Fish Bun", Category = "Short Eats", Price = 120m, Outlet = "Juice Bar", IsFavourite = false, Available = true },
            new MenuItem { Id = 4, Name = "Egg Hopper (3 pcs)", Category = "Hoppers", Price = 300m, Outlet = "Night Kade", IsFavourite = false, Available = true },
            new MenuItem { Id = 5, Name = "Cheese Kottu", Category = "Rice & Kottu", Price = 800m, Outlet = "Main Canteen", IsFavourite = false, Available = true },
            new MenuItem { Id = 6, Name = "Vegetable Roti", Category = "Short Eats", Price = 100m, Outlet = "Juice Bar", IsFavourite = false, Available = true },
            new MenuItem { Id = 7, Name = "Chicken Fried Rice", Category = "Rice & Kottu", Price = 700m, Outlet = "Main Canteen", IsFavourite = false, Available = true },
            new MenuItem { Id = 8, Name = "Plain Hopper (5 pcs)", Category = "Hoppers", Price = 250m, Outlet = "Night Kade", IsFavourite = false, Available = true },
            new MenuItem { Id = 9, Name = "Wood Apple Juice", Category = "Drinks", Price = 200m, Outlet = "Juice Bar", IsFavourite = false, Available = true },
            new MenuItem { Id = 10, Name = "Iced Milo", Category = "Drinks", Price = 180m, Outlet = "Juice Bar", IsFavourite = true, Available = true },
            new MenuItem { Id = 11, Name = "String Hoppers (10 pcs)", Category = "Hoppers", Price = 350m, Outlet = "Night Kade", IsFavourite = false, Available = false },
            new MenuItem { Id = 12, Name = "Devilled Chicken", Category = "Rice & Kottu", Price = 900m, Outlet = "Main Canteen", IsFavourite = false, Available = true },
            new MenuItem { Id = 13, Name = "Faluda", Category = "Drinks", Price = 350m, Outlet = "Juice Bar", IsFavourite = false, Available = true },
            new MenuItem { Id = 14, Name = "Isso Wade", Category = "Short Eats", Price = 150m, Outlet = "Night Kade", IsFavourite = false, Available = true },
            new MenuItem { Id = 15, Name = "Seeni Sambol Bun", Category = "Short Eats", Price = 110m, Outlet = "Juice Bar", IsFavourite = false, Available = true },
            new MenuItem { Id = 16, Name = "Nasi Goreng", Category = "Rice & Kottu", Price = 750m, Outlet = "Main Canteen", IsFavourite = false, Available = true }
        );

        b.Entity<Order>().HasData(
            new Order { Id = 101, StudentId = "IT22045678", StudentName = "Kasun Perera", Items = "Chicken Kottu x1, Iced Milo x1", Total = 830m, Status = "Pending", PlacedAt = new DateTime(2026, 7, 17, 11, 20, 0, DateTimeKind.Utc) },
            new Order { Id = 102, StudentId = "IT22098765", StudentName = "Nethmi Silva", Items = "Rice & Curry x2", Total = 800m, Status = "Preparing", PlacedAt = new DateTime(2026, 7, 17, 11, 35, 0, DateTimeKind.Utc) },
            new Order { Id = 103, StudentId = "IT22011223", StudentName = "Tharindu Jayasinghe", Items = "Egg Hopper x3, Faluda x1", Total = 1250m, Status = "Ready", PlacedAt = new DateTime(2026, 7, 17, 11, 40, 0, DateTimeKind.Utc) },
            new Order { Id = 104, StudentId = "IT22045678", StudentName = "Kasun Perera", Items = "Cheese Kottu x1", Total = 800m, Status = "Delivered", PlacedAt = new DateTime(2026, 7, 16, 13, 5, 0, DateTimeKind.Utc) },
            new Order { Id = 105, StudentId = "IT22076543", StudentName = "Dilki Fernando", Items = "Fish Bun x2, Wood Apple Juice x1", Total = 440m, Status = "Pending", PlacedAt = new DateTime(2026, 7, 17, 11, 50, 0, DateTimeKind.Utc) }
        );
    }
}
