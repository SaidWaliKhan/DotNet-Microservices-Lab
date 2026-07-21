using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    // At firs we seed a couple of products so we have something to test with immediately
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Wireless Mouse", Price = 19.99m, StockQuantity = 50 },
            new Product { Id = 2, Name = "Mechanical Keyboard", Price = 79.99m, StockQuantity = 30 },
            new Product { Id = 3, Name = "USB-C Hub", Price = 24.50m, StockQuantity = 100 }
        );
    }
}