using Shopipy.ServiceManagement.Services;
using Shopipy.Shared.Services;

namespace Shopipy.ServiceManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceManagement(this IServiceCollection services)
    {
        services.AddScoped<IServiceManagementService, ServiceManagementService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        return services;
    }
}