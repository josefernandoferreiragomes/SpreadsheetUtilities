using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public sealed class AuthServiceFactory : IAuthServiceFactory
{
    private readonly AuthService _memory;
    private readonly RedisAuthService _redis;

    public AuthServiceFactory(AuthService memory, RedisAuthService redis)
    {
        _memory = memory;
        _redis = redis;
    }

    public IAuthService GetService(CacheBackend backend) => backend switch
    {
        CacheBackend.Redis => _redis,
        CacheBackend.Memory => _memory,
        _ => _memory
    };
}