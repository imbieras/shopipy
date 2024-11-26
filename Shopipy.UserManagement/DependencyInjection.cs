using Shopipy.UserManagement.Services;

namespace Shopipy.UserManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddUserManagement(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        return services;
    }
}