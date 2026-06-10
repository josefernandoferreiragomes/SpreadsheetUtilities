using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Domain.Repositories;
using SpreadsheetUtility.Infrastructure.Abstractions;
using SpreadsheetUtility.Infrastructure.Excel;
using SpreadsheetUtility.Infrastructure.Providers;
using SpreadsheetUtility.Infrastructure.Repositories;
using SpreadsheetUtility.Infrastructure.Services;

namespace SpreadsheetUtility.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IHolidayProvider, HolidayFileProvider>();
        services.AddScoped<IHolidayRepository, HolidayRepository>();
        services.AddScoped<IDeveloperRepository, DeveloperRepository>();
        services.AddScoped<IExampleFileProvider, FolderExampleFileProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDoubleEntryGeneratorService, DoubleEntryGeneratorService>();

        return services;
    }
}
