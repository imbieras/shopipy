using Shopipy.OrderManagement.Repositories;
using Shopipy.OrderManagement.Services;

namespace Shopipy.OrderManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderManagement(this IServiceCollection services)
    {
        services.AddScoped<OrderRepository>();
        services.AddScoped<OrderService>();
        services.AddScoped<PaymentService>();
        return services;
    }
}