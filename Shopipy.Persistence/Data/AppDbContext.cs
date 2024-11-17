using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopipy.Persistence.Models;

namespace Shopipy.Persistence.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Business> Businesses { get; set; }
    
    public DbSet<Service> Services { get; set; }
}