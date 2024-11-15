using Microsoft.Extensions.DependencyInjection;
using Shopipy.BusinessManagement.Services;

namespace Shopipy.BusinessManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessManagement(this IServiceCollection services)
    {
        services.AddScoped<BusinessService>();
        return services;
    }
}