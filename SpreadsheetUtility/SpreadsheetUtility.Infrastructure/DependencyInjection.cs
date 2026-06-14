using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Domain.Repositories;
using SpreadsheetUtility.Infrastructure.Abstractions;
using SpreadsheetUtility.Infrastructure.Excel;
using SpreadsheetUtility.Infrastructure.Options;
using SpreadsheetUtility.Infrastructure.Providers;
using SpreadsheetUtility.Infrastructure.Repositories;
using SpreadsheetUtility.Infrastructure.Services;
using StackExchange.Redis;

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
        services.AddScoped<ISessionStore, AuthService>();
        services.AddScoped<AuthService>();
        services.AddScoped<IAuthServiceFactory, AuthServiceFactory>();
        services.AddScoped<IDoubleEntryGeneratorService, DoubleEntryGeneratorService>();

        return services;
    }

    public static void AddRedis(this WebApplicationBuilder builder)
    {
        // Bind RedisOptions from appsettings
        builder.Services.Configure<RedisOptions>(
            builder.Configuration.GetSection(RedisOptions.SectionName));

        // Validate options eagerly at startup — catches missing/blank ConnectionString
        // before the first request hits the app.
        builder.Services.AddOptions<RedisOptions>()
            .Bind(builder.Configuration.GetSection(RedisOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.ConnectionString),
                "Redis:ConnectionString must not be empty.")
            .ValidateOnStart();

        // Register IConnectionMultiplexer as a singleton.
        // ConnectionMultiplexer is thread-safe and designed to be shared — one instance
        // per process is the StackExchange.Redis recommendation.
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(options.ConnectionString);
        });

        // Swap the ISessionStore registration.
        // If you previously had: builder.Services.AddMemoryCache() + AddScoped<ISessionStore, AuthService>()
        // replace both of those with:
        builder.Services.AddScoped<ISessionStore, RedisAuthService>();
        builder.Services.AddScoped<RedisAuthService>();
    }
}

