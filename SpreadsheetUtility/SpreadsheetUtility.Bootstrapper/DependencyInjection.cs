using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Application;
using SpreadsheetUtility.Infrastructure;

namespace SpreadsheetUtility.Bootstrapper;

public static class DependencyInjection
{
    public static IServiceCollection AddSpreadsheetUtilities(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure();
        return services;
    }
}
