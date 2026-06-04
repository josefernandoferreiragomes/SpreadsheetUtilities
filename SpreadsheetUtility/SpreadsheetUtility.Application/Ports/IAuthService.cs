namespace SpreadsheetUtility.Application.Ports;

public interface IAuthService
{
    string InitiateSession(string email);
    string GetSession(string email, Guid sessionId);
    string UpdateSession(string email, Guid sessionId, string newValue);
}
