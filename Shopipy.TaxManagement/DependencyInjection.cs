using Shopipy.Shared.Services;
using Shopipy.TaxManagement.Services;

namespace Shopipy.TaxManagement;


public static class DependencyInjection
{
    public static IServiceCollection AddTaxManagement(this IServiceCollection services)
    {
        services.AddScoped<ITaxService, TaxService>();
        return services;
    }
}