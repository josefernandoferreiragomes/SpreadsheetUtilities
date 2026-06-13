using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public class LocalMemorySessionStorage : ISessionStorage
{
    private const string SessionIndexCacheKey = "__SessionIndex";
    private readonly IMemoryCache _memoryCache;

    public LocalMemorySessionStorage(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    private ConcurrentDictionary<string, (Guid SessionId, DateTime CreatedAt, DateTime LastModifiedAt)> GetOrCreateSessionIndex()
    {
        if (_memoryCache.TryGetValue<ConcurrentDictionary<string, (Guid, DateTime, DateTime)>>(SessionIndexCacheKey, out var index))
        {
            return index!;
        }

        var newIndex = new ConcurrentDictionary<string, (Guid SessionId, DateTime CreatedAt, DateTime LastModifiedAt)>();
        _memoryCache.Set(SessionIndexCacheKey, newIndex);
        return newIndex;
    }

    public string InitiateSession(string email)
    {
        var index = GetOrCreateSessionIndex();

        if (index.ContainsKey(email))
        {
            throw new InvalidOperationException($"A session already exists for email '{email}'.");
        }

        var guid = Guid.NewGuid();
        var cacheValue = guid.ToString();
        _memoryCache.Set(email, cacheValue);
        _memoryCache.Set(guid.ToString(), string.Empty);
        index[email] = (guid, DateTime.UtcNow, DateTime.UtcNow);
        return cacheValue;
    }

    public string? GetSession(string email, Guid sessionId)
    {
        if (_memoryCache.TryGetValue<string>(email, out var cacheValue))
        {
            if (cacheValue is not null && cacheValue == sessionId.ToString())
            {
                var guidCacheKey = sessionId.ToString();
                _memoryCache.TryGetValue<string>(guidCacheKey, out var guidCacheValue);
                return guidCacheValue ?? string.Empty;
            }
        }
        return null;
    }

    public string UpdateSession(string email, Guid sessionId, string newValue)
    {
        if (_memoryCache.TryGetValue<string>(email, out var cacheValue))
        {
            if (cacheValue is not null && cacheValue == sessionId.ToString())
            {
                var guidCacheKey = sessionId.ToString();
                _memoryCache.Set(guidCacheKey, newValue);
                var index = GetOrCreateSessionIndex();
                if (index.TryGetValue(email, out var entry))
                {
                    index[email] = (entry.SessionId, entry.CreatedAt, DateTime.UtcNow);
                }
                return newValue;
            }
        }
        throw new System.Security.Authentication.AuthenticationException("Invalid session.");
    }

    public IReadOnlyCollection<SessionInfoDto> GetAllSessions()
    {
        var index = GetOrCreateSessionIndex();

        return index.Select(kvp =>
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
