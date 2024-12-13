using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shopipy.ApiService.Authorization;
using Shopipy.ApiService.ExceptionFilters;
using Shopipy.ApiService.Services;
using Shopipy.BusinessManagement;
using Shopipy.BusinessManagement.Mappings;
using Shopipy.CategoryManagement;
using Shopipy.CategoryManagement.Mappings;
using Shopipy.ProductManagement.Mappings;
using Shopipy.Persistence.Data;
using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared;
using Shopipy.ServiceManagement;
using Shopipy.ServiceManagement.Mappings;
using Shopipy.UserManagement;
using Shopipy.UserManagement.Mappings;
using Shopipy.ProductManagement;
using Shopipy.TaxManagement;
using Shopipy.TaxManagement.Mappings;
using Shopipy.ServiceManagement.Interfaces;
using Shopipy.ServiceManagement.Services;
using Shopipy.DiscountManagement;
using Shopipy.DiscountManagement.Mappings;


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
    typeof(AppointmentMappingProfile), typeof(CategoryMappingProfile), typeof(ProductMappingProfile), typeof(DiscountMappingProfile), typeof(TaxRateMappingProfile));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddBusinessManagement();
builder.Services.AddServiceManagement();
builder.Services.AddCategoryManagement();
builder.Services.AddProductManagement();
builder.Services.AddDiscountManagement();
builder.Services.AddTaxManagement();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UnauthorizedAccessExceptionFilter>();
    options.Filters.Add<ArgumentExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter bearer token here"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.RequireSuperAdmin,
        policy => policy.RequireClaim(ClaimTypes.Role, UserRole.SuperAdmin.ToString()));
    options.AddPolicy(AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin,
        policy => policy.RequireClaim(ClaimTypes.Role, UserRole.BusinessOwner.ToString(),
            UserRole.SuperAdmin.ToString()));
    options.AddPolicy(AuthorizationPolicies.RequireBusinessAccess, policy =>
    {
        policy.AddRequirements(new RequireBusinessAccessRequirement());
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, RequireBusinessAccessRequirementHandler>();

builder.Services.AddScoped<AuthService>(_ => new AuthService(signingCredentials, issuer, audience));
builder.Services.AddShared();
builder.Services.AddUserManagement();

builder.Services.AddSingleton<ISMSService>(provider => 
    new TwilioSMSService(
        builder.Configuration["TwilioAccountSid"]!,
        builder.Configuration["TwilioAuthToken"]!,
        builder.Configuration["TwilioPhoneNumber"]!
    ));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapDefaultEndpoints();

app.MapControllers();



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.InjectJavascript("/swagger-extension.js", "module");
});

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("Applying migrations...");
    dbContext.Database.Migrate();
    Console.WriteLine("Migrations applied successfully.");

    // Seed database
    if (!dbContext.Businesses.Any(b => b.BusinessId == 1))
    {
        Console.WriteLine("Seeding default business...");
        dbContext.Businesses.Add(new Business
        {
            BusinessId = 1,
            Name = "Default Business",
            Address = "123 Default St, Default City",
            Email = "contact@defaultbusiness.com",
            Phone = "+1234567890",
            VatNumber = "VAT123456",
            BusinessType = BusinessType.Retail,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        dbContext.SaveChanges();
        Console.WriteLine("Default business seeded successfully.");
    }

    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await userManager.CreateAsync(new User("seeded_superuser")
    {
        Name = "Admin",
        Role = UserRole.SuperAdmin,
        Email = "admin@shopipy.com",
        PhoneNumber = "+37065011111"
    }, "Seeded_password1234");

    await userManager.CreateAsync(new User("test_worker")
    {
        Name = "Employee",
        Role = UserRole.Employee,
        BusinessId = 1
    }, "Test_password1234");

    dbContext.SaveChanges();

    var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

app.Run();