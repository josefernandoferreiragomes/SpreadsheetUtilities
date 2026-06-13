using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Infrastructure.ApiClients;
using Newtonsoft.Json;

namespace SpreadsheetUtility.Infrastructure.Services;

public class AuthApiSessionStorage : ISessionStorage
{
    private readonly SpreadsheetUtilitiesAuthApiClient _client;

    public AuthApiSessionStorage()
    {
        _client = new SpreadsheetUtilitiesAuthApiClient(new HttpClient());
    }

    public string InitiateSession(string email)
        => _client.InitiateSessionAsync(email, null).Result;

    public string? GetSession(string email, Guid sessionId)
        => _client.GetSessionAsync(email, sessionId).Result;

    public string UpdateSession(string email, Guid sessionId, string newValue)
        => _client.UpdateSessionAsync(email, sessionId, newValue).Result;

    public IReadOnlyCollection<SessionInfoDto> GetAllSessions()
    {
        var json = _client.ListSessionsAsync().Result;
        return JsonConvert.DeserializeObject<List<SessionInfoDto>>(json) ?? new List<SessionInfoDto>();
    }
}
