using SpreadsheetUtility.UI.Web.Services.Generated;
using SpreadsheetUtility.UI.Web.Models;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace SpreadsheetUtility.UI.Web.Services
{
    public class SessionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string CookieProtectionPurpose = "SpreadsheetUtility.SessionCookie";

        // In-memory cache for session states (email -> SessionState)
        private readonly Dictionary<string, SessionState> _sessionCache = new();
        private readonly object _cacheLock = new();

        public SessionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        /// <summary>
        /// Initializes a new session for the given email and returns the session ID.
        /// </summary>
        public string InitiateSession(string email) 
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);  
                var result = authApiClient.InitiateSessionAsync(email, null);
                var sessionId = result.Result;

                // Create local session state
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

        /// <summary>
        /// Retrieves session state from cache. Returns null if session not found.
        /// </summary>
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

        /// <summary>
        /// Gets the encrypted session content from the remote API.
        /// </summary>
        public string GetSession(string email, Guid sessionId)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);
                var result = authApiClient.GetSessionAsync(email, sessionId);
                return result.Result;
            }
        }

        /// <summary>
        /// Updates the remote session and updates local cache.
        /// </summary>
        public string UpdateSession(string email, Guid sessionId, string serializedObject)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new SpreadsheetUtilitiesAuthApiClient(http);
                var result = authApiClient.UpdateSessionAsync(email, sessionId, serializedObject);

                // Update local cache
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

        /// <summary>
        /// Saves project data to session state and remote storage.
        /// </summary>
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

        /// <summary>
        /// Saves task data to session state and remote storage.
        /// </summary>
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

        /// <summary>
        /// Saves team data to session state and remote storage.
        /// </summary>
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

        /// <summary>
        /// Loads all cached data from the session state.
        /// </summary>
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

        /// <summary>
        /// Securely stores session content in a cookie using encryption.
        /// </summary>
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

        /// <summary>
        /// Retrieves and decrypts session content from a cookie.
        /// </summary>
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

        /// <summary>
        /// Clears the session from cache.
        /// </summary>
        public void ClearSession(string email)
        {
            lock (_cacheLock)
            {
                _sessionCache.Remove(email);
            }
        }
    }
}
