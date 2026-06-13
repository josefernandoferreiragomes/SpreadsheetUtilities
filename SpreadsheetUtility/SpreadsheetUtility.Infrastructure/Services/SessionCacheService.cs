using SpreadsheetUtility.Infrastructure.Models;

namespace SpreadsheetUtility.Infrastructure.Services;

public class SessionCacheService
{
    private readonly Dictionary<string, SessionState> _sessionCache = new();
    private readonly object _cacheLock = new();

    public SessionState? TryGet(string email)
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

    public void Set(string email, SessionState state)
    {
        lock (_cacheLock)
        {
            _sessionCache[email] = state;
        }
    }

    public void Remove(string email)
    {
        lock (_cacheLock)
        {
            _sessionCache.Remove(email);
        }
    }

    public void UpdateLastModified(string email)
    {
        lock (_cacheLock)
        {
            if (_sessionCache.TryGetValue(email, out var sessionState))
            {
                sessionState.LastModifiedAt = DateTime.UtcNow;
            }
        }
    }

    public void UpdateProjectData(string email, string projectData)
    {
        lock (_cacheLock)
        {
            if (_sessionCache.TryGetValue(email, out var sessionState))
            {
                sessionState.ProjectData = projectData;
                sessionState.LastModifiedAt = DateTime.UtcNow;
            }
        }
    }

    public void UpdateTaskData(string email, string taskData)
    {
        lock (_cacheLock)
        {
            if (_sessionCache.TryGetValue(email, out var sessionState))
            {
                sessionState.TaskData = taskData;
                sessionState.LastModifiedAt = DateTime.UtcNow;
            }
        }
    }

    public void UpdateTeamData(string email, string teamData)
    {
        lock (_cacheLock)
        {
            if (_sessionCache.TryGetValue(email, out var sessionState))
            {
                sessionState.TeamData = teamData;
                sessionState.LastModifiedAt = DateTime.UtcNow;
            }
        }
    }

    public IReadOnlyCollection<SessionState> GetAll()
    {
        lock (_cacheLock)
        {
            return _sessionCache.Values.ToList().AsReadOnly();
        }
    }
}
