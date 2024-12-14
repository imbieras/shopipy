using Shopipy.OrderManagement.Services;

namespace Shopipy.OrderManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderManagement(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        return services;
    }
}