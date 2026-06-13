using SpreadsheetUtility.Infrastructure.ApiClients;
using SpreadsheetUtility.Infrastructure.Models;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using SpreadsheetUtility.Application.DTOs.Session;
using Newtonsoft.Json;

namespace SpreadsheetUtility.Infrastructure.Services
{
    public class SessionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string CookieProtectionPurpose = "SpreadsheetUtility.SessionCookie";

        private readonly Dictionary<string, SessionState> _sessionCache = new();
        private readonly object _cacheLock = new();

        public SessionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string InitiateSession(string email)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);
                var result = authApiClient.InitiateSessionAsync(email, null);
                var sessionId = result.Result;

                lock (_cacheLock)
                {
                    _sessionCache[email] = new SessionState
                    {
                        Email = email,
                        SessionId = Guid.Parse(sessionId),
                        IsInitialized = true
                    };
                }

                return sessionId;
            }
        }

        public SessionState? GetSessionState(string email)
        {
            lock (_cacheLock)
            {
                if (_sessionCache.TryGetValue(email, out var sessionState))
                {
                    return sessionState;
                }
            }
            return null;
        }

        public string GetSession(string email, Guid sessionId)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);
                var result = authApiClient.GetSessionAsync(email, sessionId);
                return result.Result;
            }
        }

        public string UpdateSession(string email, Guid sessionId, string serializedObject)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);
                var result = authApiClient.UpdateSessionAsync(email, sessionId, serializedObject);

                lock (_cacheLock)
                {
                    if (_sessionCache.TryGetValue(email, out var sessionState))
                    {
                        sessionState.LastModifiedAt = DateTime.UtcNow;
                    }
                }

                return result.Result;
            }
        }

        public void SaveProjectData(string email, Guid sessionId, string projectData)
        {
            lock (_cacheLock)
            {
                if (_sessionCache.TryGetValue(email, out var sessionState))
                {
                    sessionState.ProjectData = projectData;
                    sessionState.LastModifiedAt = DateTime.UtcNow;
                }
            }
            UpdateSession(email, sessionId, projectData);
        }

        public void SaveTaskData(string email, Guid sessionId, string taskData)
        {
            lock (_cacheLock)
            {
                if (_sessionCache.TryGetValue(email, out var sessionState))
                {
                    sessionState.TaskData = taskData;
                    sessionState.LastModifiedAt = DateTime.UtcNow;
                }
            }
            UpdateSession(email, sessionId, taskData);
        }

        public void SaveTeamData(string email, Guid sessionId, string teamData)
        {
            lock (_cacheLock)
            {
                if (_sessionCache.TryGetValue(email, out var sessionState))
                {
                    sessionState.TeamData = teamData;
                    sessionState.LastModifiedAt = DateTime.UtcNow;
                }
            }
            UpdateSession(email, sessionId, teamData);
        }

        public SessionState? LoadCachedSessionData(string email)
        {
            lock (_cacheLock)
            {
                if (_sessionCache.TryGetValue(email, out var sessionState))
                {
                    return sessionState;
                }
            }
            return null;
        }

        public string StoreSessionContentInCookie(string sessionContent)
        {
            try
            {
                var protector = _dataProtectionProvider.CreateProtector(CookieProtectionPurpose);
                var contentBytes = Encoding.UTF8.GetBytes(sessionContent);
                var encryptedBytes = protector.Protect(contentBytes);
                var encryptedContent = Convert.ToBase64String(encryptedBytes);
                return encryptedContent;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to encrypt session content for cookie storage.", ex);
            }
        }

        public string RetrieveSessionContentFromCookie(string encryptedContent)
        {
            try
            {
                var protector = _dataProtectionProvider.CreateProtector(CookieProtectionPurpose);
                var encryptedBytes = Convert.FromBase64String(encryptedContent);
                var decryptedBytes = protector.Unprotect(encryptedBytes);
                var sessionContent = Encoding.UTF8.GetString(decryptedBytes);
                return sessionContent;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt session content from cookie.", ex);
            }
        }

        public void ClearSession(string email)
        {
            lock (_cacheLock)
            {
                _sessionCache.Remove(email);
            }
        }

        public IReadOnlyCollection<SessionState> GetAllSessions()
        {
            lock (_cacheLock)
            {
                return _sessionCache.Values.ToList().AsReadOnly();
            }
        }
        public async System.Threading.Tasks.Task<List<SessionInfoDto>> FetchAllSessionsFromApiAsync()
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);
                var json = await authApiClient.ListSessionsAsync();
                return JsonConvert.DeserializeObject<List<SessionInfoDto>>(json) ?? new List<SessionInfoDto>();
            }
        }

    }
}
