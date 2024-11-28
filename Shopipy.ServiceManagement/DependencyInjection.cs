using Shopipy.ServiceManagement.Services;

namespace Shopipy.ServiceManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceManagement(this IServiceCollection services)
    {
        services.AddScoped<ServiceManagementService>();
        services.AddScoped<AppointmentService>();
        return services;
    }
}