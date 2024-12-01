using Shopipy.Shared.Services;

namespace Shopipy.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services.AddScoped<CurrentUserService>();
        return services;
    }
}