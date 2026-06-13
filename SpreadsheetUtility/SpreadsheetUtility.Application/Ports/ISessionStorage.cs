using SpreadsheetUtility.Application.DTOs.Session;

namespace SpreadsheetUtility.Application.Ports;

public interface ISessionStorage
{
    string InitiateSession(string email);
    string? GetSession(string email, Guid sessionId);
    string UpdateSession(string email, Guid sessionId, string newValue);
    IReadOnlyCollection<SessionInfoDto> GetAllSessions();
}
