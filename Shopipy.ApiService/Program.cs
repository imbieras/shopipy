using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shopipy.ApiService.Services;
using Shopipy.BusinessManagement;
using Shopipy.BusinessManagement.Mappings;
using Shopipy.Persistence.Data;
using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.UserManagement.Mappings;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var isLocalDevelopment = builder.Environment.IsEnvironment("LocalDevelopment");

if (isLocalDevelopment)
{
    Console.WriteLine("Using Local Development PostgresSQL Database");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    Console.WriteLine("Using Aspire's PostgresSQL Database");
    builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgresdb");
}

builder.Services.AddAutoMapper(typeof(UserMappingProfile), typeof(BusinessMappingProfile));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddBusinessManagement();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Jwt:Key"]!));
var signingCredentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
var issuer = builder.Configuration["Authentication:Jwt:Issuer"]!;
var audience = builder.Configuration["Authentication:Jwt:Audience"]!;

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.IssuerSigningKey = jwtKey;
        options.TokenValidationParameters.ValidIssuer = issuer;
        options.TokenValidationParameters.ValidAudience = audience;
    });

builder.Services.AddScoped<AuthService>(_ => new AuthService(signingCredentials, issuer, audience));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    Console.WriteLine("Applying migrations...");
    dbContext.Database.Migrate();
    Console.WriteLine("Migrations applied successfully.");    
    // Seed database
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await userManager.CreateAsync(new User("seeded_superuser") {Name = "Admin", Role = UserRole.SuperAdmin, Email = "admin@shopipy.com", PhoneNumber = "+37065011111"}, "Seeded_password1234");

    dbContext.SaveChanges();
    
    var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

app.Run();
