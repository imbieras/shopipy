using System.Security.Claims;
using System.Text.Json.Serialization;
using AppointmentManagement;
using AppointmentManagement.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopipy.BusinessManagement;
using Shopipy.BusinessManagement.Mappings;
using Shopipy.BusinessManagement.Services;
using Shopipy.CategoryManagement;
using Shopipy.CategoryManagement.Mappings;
using Shopipy.Persistence.Data;
using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.ServiceManagement;
using Shopipy.ServiceManagement.Mappings;
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

builder.Services.AddAutoMapper(typeof(UserMappingProfile), typeof(BusinessMappingProfile), typeof(ServiceMappingProfile), 
    typeof(AppointmentMappingProfile), typeof(CategoryMappingProfile));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddBusinessManagement();
builder.Services.AddServiceManagement();
builder.Services.AddAppointmentManagement();
builder.Services.AddCategoryManagement();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken()
    .AddGoogle(options => {
        var config = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = config["ClientId"]!;
        options.ClientSecret = config["ClientSecret"]!;
        options.Events.OnTicketReceived = async (context) => {
            var providerKey = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailAddress = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (providerKey == null) throw new InvalidOperationException("nameidentifier not found");
            if (emailAddress == null) throw new InvalidOperationException("email not found");
            
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByLoginAsync(GoogleDefaults.AuthenticationScheme, providerKey);
            if (user == null)
            {
                user = new User(emailAddress) {Name = "Google", Role = UserRole.SuperAdmin};
                await userManager.CreateAsync(user);
                await userManager.AddLoginAsync(user, new UserLoginInfo(GoogleDefaults.AuthenticationScheme, providerKey, null));
            }

            var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
            var principal = await signInManager.CreateUserPrincipalAsync(user);
            await context.HttpContext.SignInAsync(principal);
            context.HandleResponse();
        };

    });

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
    await userManager.CreateAsync(new User("seeded_superuser") {Name = "Admin", Role = UserRole.SuperAdmin}, "Seeded_password1234");

    dbContext.SaveChanges();
    
    var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

app.Run();
