using Microsoft.Extensions.Caching.Memory;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IMemoryCache _memoryCache;

    public AuthService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public string InitiateSession(string email)
    {
        var guid = Guid.NewGuid();
        var emailCacheKey = email;
        var cacheValue = guid.ToString();
        _memoryCache.Set(emailCacheKey, cacheValue);
        return cacheValue;
    }

    public string? GetSession(string email, Guid sessionId)
    {
        var emailCacheKey = email;
        if (sessionId == Guid.Empty)
        {
            if (_memoryCache.TryGetValue<string>(emailCacheKey, out var cacheValue))
            {
                return cacheValue ?? string.Empty;
            }
        }
        else
        {
            if (_memoryCache.TryGetValue<string>(emailCacheKey, out var cacheValue))
            {
                if (cacheValue is not null && cacheValue == sessionId.ToString())
                {
                    var guidCacheKey = sessionId.ToString();
                    _memoryCache.TryGetValue<string>(guidCacheKey, out var guidCacheValue);
                    return guidCacheValue ?? string.Empty;
                }
            }
        }
        return null;
    }

    public string UpdateSession(string email, Guid sessionId, string newValue)
    {
        var emailCacheKey = email;
        if (_memoryCache.TryGetValue<string>(emailCacheKey, out var cacheValue))
        {
            if (cacheValue is not null && cacheValue == sessionId.ToString())
            {
                var guidCacheKey = sessionId.ToString();
                _memoryCache.Set(guidCacheKey, newValue);
                return newValue;
            }
        }
        throw new System.Security.Authentication.AuthenticationException("Invalid session.");
    }
}