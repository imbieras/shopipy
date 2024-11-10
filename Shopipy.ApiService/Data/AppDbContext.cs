using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopipy.UserManagement.Models;

namespace Shopipy.ApiService.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options);