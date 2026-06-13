using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public class RedisSessionStorage : ISessionStorage
{
    public string InitiateSession(string email)
        => throw new NotImplementedException("Redis cache is under development.");

    public string? GetSession(string email, Guid sessionId)
        => throw new NotImplementedException("Redis cache is under development.");

    public string UpdateSession(string email, Guid sessionId, string newValue)
        => throw new NotImplementedException("Redis cache is under development.");

    public IReadOnlyCollection<SessionInfoDto> GetAllSessions()
        => throw new NotImplementedException("Redis cache is under development.");
}
