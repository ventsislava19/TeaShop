using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeaShop.Models;

namespace TeaShop.Data;

// This is the DbContext.

public class TeaShopContext : IdentityDbContext<ApplicationUser>
{
    public TeaShopContext(DbContextOptions<TeaShopContext> options) : base(options) {}
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    // Fine-tunes how EF maps the classes to tables like an extra configuration on top of the data annotations.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IdentityDbContext needs this to set up its own tables.
        base.OnModelCreating(modelBuilder);
        
        // Enum-to-string conversions.
        modelBuilder.Entity<Product>()
            .Property(p => p.Caffeine)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(30);
            
        // Relationships.
        // When a category is deleted, set CategoryId to null on its products.
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // When an order is deleted, cascade delete its items.
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}