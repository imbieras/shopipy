using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shopipy.ApiService.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext(options);