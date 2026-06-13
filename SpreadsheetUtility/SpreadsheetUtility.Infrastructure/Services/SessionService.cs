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
            UpdateSession(email, sessionId, projectData);
        }

        public void SaveTaskData(string email, Guid sessionId, string taskData)
        {
            _cacheService.UpdateTaskData(email, taskData);
            UpdateSession(email, sessionId, taskData);
        }

        public void SaveTeamData(string email, Guid sessionId, string teamData)
        {
            _cacheService.UpdateTeamData(email, teamData);
            UpdateSession(email, sessionId, teamData);
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
    }
}
