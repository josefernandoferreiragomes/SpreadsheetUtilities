using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, (Guid SessionId, DateTime CreatedAt, DateTime LastModifiedAt)> _sessionIndex = new();

    public AuthService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public string InitiateSession(string email)
    {
        if (_sessionIndex.ContainsKey(email))
        {
            throw new InvalidOperationException($"A session already exists for email '{email}'.");
        }

        var guid = Guid.NewGuid();
        var emailCacheKey = email;
        var cacheValue = guid.ToString();
        _memoryCache.Set(emailCacheKey, cacheValue);
        _memoryCache.Set(guid.ToString(), string.Empty);
        _sessionIndex[email] = (guid, DateTime.UtcNow, DateTime.UtcNow);
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
                if (_sessionIndex.TryGetValue(email, out var entry))
                {
                    _sessionIndex[email] = (entry.SessionId, entry.CreatedAt, DateTime.UtcNow);
                }
                return newValue;
            }
        }
        throw new System.Security.Authentication.AuthenticationException("Invalid session.");
    }

    public IReadOnlyCollection<SessionInfoDto> GetAllSessions()
    {
        return _sessionIndex.Select(kvp =>
        {
            var guidKey = kvp.Value.SessionId.ToString();
            _memoryCache.TryGetValue<string>(guidKey, out var sessionData);
            return new SessionInfoDto
            {
                Email = kvp.Key,
                SessionId = kvp.Value.SessionId,
                CreatedAt = kvp.Value.CreatedAt,
                LastModifiedAt = kvp.Value.LastModifiedAt,
                SessionData = sessionData
            };
        }).ToList().AsReadOnly();
    }
}