using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopipy.ApiService.Data;
using Shopipy.ApiService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken()
    .AddGoogle(options =>
    {
        var config = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = config["ClientId"]!;
        options.ClientSecret = config["ClientSecret"]!;
        options.Events.OnTicketReceived = async (context) =>
        {
            var providerKey = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailAddress = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (providerKey == null) throw new InvalidOperationException("nameidentifier not found");
            if (emailAddress == null) throw new InvalidOperationException("email not found");
            
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByLoginAsync(GoogleDefaults.AuthenticationScheme, providerKey);
            if (user == null)
            {
                user = new User(emailAddress);
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
    dbContext.Database.Migrate();
    
    // Seed database
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await userManager.CreateAsync(new User("seeded_superuser"), "Seeded_password1234");

    dbContext.SaveChanges();
}

app.Run();
