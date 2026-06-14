using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public sealed class AuthServiceFactory : IAuthServiceFactory
{
    private readonly AuthService _memory;
    private readonly IServiceProvider _serviceProvider;

    public AuthServiceFactory(AuthService memory, IServiceProvider serviceProvider)
    {
        _memory = memory;
        _serviceProvider = serviceProvider;
    }

    public ISessionStore GetService(CacheBackend backend) => backend switch
    {
        CacheBackend.Redis => _serviceProvider.GetRequiredService<RedisAuthService>(),
        CacheBackend.Memory => _memory,
        _ => _memory
    };
}
