using System.Text.Json;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Infrastructure.Models;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Configuration;

namespace SpreadsheetUtility.Infrastructure.Services
{
    public class SessionService
    {
        private readonly SessionStorageSelector _storageSelector;
        private readonly SessionCacheService _cacheService;
        private readonly ISessionCookieService _cookieService;

        public SessionService(SessionStorageSelector storageSelector, SessionCacheService cacheService, ISessionCookieService cookieService)
        {
            _storageSelector = storageSelector;
            _cacheService = cacheService;
            _cookieService = cookieService;
        }

        public string InitiateSession(string email)
        {
            var activeStorage = _storageSelector.GetActiveStorage();
            var sessionId = activeStorage.InitiateSession(email);

            _cacheService.Set(email, new SessionState
            {
                Email = email,
                SessionId = Guid.Parse(sessionId),
                IsInitialized = true
            });

            return sessionId;
        }

        public SessionState? GetSessionState(string email)
        {
            // 1. Check cache first
            var cached = _cacheService.TryGet(email);
            if (cached != null)
                return cached;

            // 2. Fall back to storage backend
            var activeStorage = _storageSelector.GetActiveStorage();
            var found = activeStorage.TryFindSessionByEmail(email);
            if (found != null)
            {
                // 3. Hydrate cache so subsequent calls find it
                var hydrated = new SessionState
                {
                    Email = found.Email,
                    SessionId = found.SessionId,
                    IsInitialized = true,
                    CreatedAt = found.CreatedAt,
                    LastModifiedAt = found.LastModifiedAt
                };

                // 4. Restore data fields from the combined JSON in SessionData
                if (found.SessionData != null)
                {
                    try
                    {
                        var combined = JsonSerializer.Deserialize<CombinedSessionData>(found.SessionData);
                        if (combined != null)
                        {
                            hydrated.ProjectData = combined.ProjectData;
                            hydrated.TaskData = combined.TaskData;
                            hydrated.TeamData = combined.TeamData;
                        }
                    }
                    catch (JsonException)
                    {
                        // Legacy format: SessionData contains a plain project data string
                        hydrated.ProjectData = found.SessionData;
                    }
                }

                _cacheService.Set(email, hydrated);
                return hydrated;
            }

            return null;
        }

        public string GetSession(string email, Guid sessionId)
        {
            var activeStorage = _storageSelector.GetActiveStorage();
            return activeStorage.GetSession(email, sessionId) ?? string.Empty;
        }

        public string UpdateSession(string email, Guid sessionId, string serializedObject)
        {
            var activeStorage = _storageSelector.GetActiveStorage();
            var result = activeStorage.UpdateSession(email, sessionId, serializedObject);

            _cacheService.UpdateLastModified(email);

            return result;
        }

        public void SaveProjectData(string email, Guid sessionId, string projectData)
        {
            _cacheService.UpdateProjectData(email, projectData);
            var combined = BuildCombinedSessionData(email);
            UpdateSession(email, sessionId, JsonSerializer.Serialize(combined));
        }

        public void SaveTaskData(string email, Guid sessionId, string taskData)
        {
            _cacheService.UpdateTaskData(email, taskData);
            var combined = BuildCombinedSessionData(email);
            UpdateSession(email, sessionId, JsonSerializer.Serialize(combined));
        }

        public void SaveTeamData(string email, Guid sessionId, string teamData)
        {
            _cacheService.UpdateTeamData(email, teamData);
            var combined = BuildCombinedSessionData(email);
            UpdateSession(email, sessionId, JsonSerializer.Serialize(combined));
        }

        public SessionState? LoadCachedSessionData(string email)
        {
            return _cacheService.TryGet(email);
        }

        public void ClearSession(string email)
        {
            _cacheService.Remove(email);
        }

        public IReadOnlyCollection<SessionState> GetAllSessions()
        {
            return _cacheService.GetAll();
        }

        public async System.Threading.Tasks.Task<List<SessionInfoDto>> FetchAllSessionsFromApiAsync()
        {
            var activeStorage = _storageSelector.GetActiveStorage();
            return activeStorage.GetAllSessions().ToList();
        }

        public async System.Threading.Tasks.Task<List<SessionInfoDto>> FetchSessionsFromLocationAsync(SessionStorageLocation location)
        {
            var storage = _storageSelector.GetStorage(location);
            return storage.GetAllSessions().ToList();
        }

        private CombinedSessionData BuildCombinedSessionData(string email)
        {
            var cached = _cacheService.TryGet(email);
            return new CombinedSessionData
            {
                ProjectData = cached?.ProjectData,
                TaskData = cached?.TaskData,
                TeamData = cached?.TeamData
            };
        }
    }
}
