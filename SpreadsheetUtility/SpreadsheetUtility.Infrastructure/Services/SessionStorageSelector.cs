using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Application.Configuration;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public class SessionStorageSelector
{
    private const string LocationCacheKey = "__SessionStorageLocation";
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceProvider _serviceProvider;

    public SessionStorageSelector(IMemoryCache memoryCache, IServiceProvider serviceProvider)
    {
        _memoryCache = memoryCache;
        _serviceProvider = serviceProvider;
    }

    public SessionStorageLocation GetCurrentLocation()
    {
        if (_memoryCache.TryGetValue<SessionStorageLocation>(LocationCacheKey, out var location))
        {
            return location;
        }
        return SessionStorageLocation.AuthApiMemoryCache;
    }

    public void SetLocation(SessionStorageLocation location)
    {
        _memoryCache.Set(LocationCacheKey, location);
    }

    public ISessionStorage GetActiveStorage()
    {
        var location = GetCurrentLocation();
        return location switch
        {
            SessionStorageLocation.UiWebMemoryCache => _serviceProvider.GetRequiredService<LocalMemorySessionStorage>(),
            SessionStorageLocation.AuthApiMemoryCache => _serviceProvider.GetRequiredService<AuthApiSessionStorage>(),
            SessionStorageLocation.RedisCache => _serviceProvider.GetRequiredService<RedisSessionStorage>(),
            _ => _serviceProvider.GetRequiredService<AuthApiSessionStorage>()
        };
    }
}
