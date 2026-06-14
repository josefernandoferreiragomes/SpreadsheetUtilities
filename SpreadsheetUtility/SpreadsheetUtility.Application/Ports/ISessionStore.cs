using SpreadsheetUtility.Application.DTOs.Session;

namespace SpreadsheetUtility.Application.Ports;

/// <summary>
/// Unified port interface for session storage operations.
/// Replaces both IAuthService and ISessionStorage.
/// </summary>
public interface ISessionStore
{
    string InitiateSession(string email);
    string? GetSession(string email, Guid sessionId);
    string UpdateSession(string email, Guid sessionId, string newValue);
    IReadOnlyCollection<SessionInfoDto> GetAllSessions();
    SessionInfoDto? TryFindSessionByEmail(string email);
}
