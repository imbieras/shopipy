using Shopipy.Web;
using Shopipy.Web.Middlewares;
using Shopipy.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddOutputCache();

builder.Services.AddHttpContextAccessor();

var isLocalDevelopment = builder.Environment.IsEnvironment("LocalDevelopment");

builder.Services.AddHttpClient("Shopipy.ApiService", client => {
        if (isLocalDevelopment)
        {
            Console.WriteLine("Using Local Development API");
            client.BaseAddress = new Uri("http://shopipy.apiservice:80/");
        }
        else
        {
            Console.WriteLine("Using Aspire's API");
            client.BaseAddress = new Uri("https://apiservice");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(serviceProvider => {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        return new TokenAppendingHandler(httpContextAccessor, new HttpClientHandler());
    });

builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<TokenMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
"default",
"{controller=Home}/{action=Index}/{id?}");

app.Run();