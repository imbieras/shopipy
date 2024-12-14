using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopipy.Persistence.Models;

namespace Shopipy.Persistence.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Business> Businesses { get; set; }
    
    public DbSet<Service> Services { get; set; }
    
    public DbSet<Appointment> Appointments { get; set; }
    
    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductVariation> ProductVariations { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderDiscount> OrderDiscounts { get; set; }
}