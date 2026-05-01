using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Library.DataAccess.DbContexts;
using SpreadsheetUtility.Library.DataAccess.Models;
using SpreadsheetUtility.Library.DataAccess.Repositories;
using UnitOfWorkType = SpreadsheetUtility.Library.DataAccess.UnitOfWork.UnitOfWork;
using IUnitOfWorkType = SpreadsheetUtility.Library.DataAccess.UnitOfWork.IUnitOfWork;
using SpreadsheetUtility.Library.Identity.Services;

namespace SpreadsheetUtility.Library.Identity.Extensions;

/// <summary>
/// Extension methods for registering authentication and data access services.
/// </summary>
public static class AuthenticationServiceExtensions
{
    /// <summary>
    /// Adds authentication services to the dependency injection container.
    /// Registers ApplicationDbContext, Identity, repositories, and authentication service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        // Register ApplicationDbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register Identity services
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                // Password requirements
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // SignIn settings
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Register repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register Unit of Work
        services.AddScoped<IUnitOfWorkType, UnitOfWorkType>();

        // Register authentication service
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}
