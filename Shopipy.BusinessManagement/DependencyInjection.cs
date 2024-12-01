using Shopipy.BusinessManagement.Services;
using Shopipy.Shared.Services;

namespace Shopipy.BusinessManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessManagement(this IServiceCollection services)
    {
        services.AddScoped<IBusinessService, BusinessService>();
        return services;
    }
}