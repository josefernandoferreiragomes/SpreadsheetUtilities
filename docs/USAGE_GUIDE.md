# Session Management Usage Guide

## Quick Start

### 1. User Login Flow

```razor
<!-- User enters email -->
<InputText @bind-Value="eMail" />

<!-- User clicks button -->
<button @onclick="InitializeSession">Initialize Session</button>
```

**What happens:**
- Session is created on remote API
- Session GUID is generated
- Local cache stores session state
- Main content becomes visible
- Previous session data is auto-loaded if available

### 2. Parse and Save Data

**Parse Projects:**
```csharp
private async Task ParseProjects()
{
    try
    {
        // Convert Excel data to DTOs
        var tempProjectDtoList = GanttMapperHelper.ConvertToGanttProjects(excelProjectData);
        projectDtoListOutput = tempProjectDtoList.AsQueryable();

        // Save to cache and API
        if (sessionIdentifierGuid.HasValue)
        {
            SessionService.SaveProjectData(eMail, sessionIdentifierGuid.Value, excelProjectData);
        }
    }
    catch (Exception ex)
    {
        await JS.InvokeVoidAsync("console.error", $"Error: {ex.Message}");
    }
}
```

**What happens:**
- Data is parsed and validated
- DTOs are displayed in grid
- Data is stored in cache: `_sessionCache[email].ProjectData = excelProjectData`
- Remote API is updated
- User can see parsed data immediately
- Session is persisted for future visits

## SessionService API Reference

### Initialize Session
```csharp
public string InitiateSession(string email)
```
**Purpose:** Create new session for user
**Parameters:** 
- `email`: User's email address
**Returns:** Session GUID as string
**Side Effects:** 
- Creates remote session
- Caches SessionState object
- Returns sessionId for UI display

**Example:**
```csharp
var sessionIdString = SessionService.InitiateSession("john@company.com");
var sessionGuid = Guid.Parse(sessionIdString);
```

### Load Cached Session Data
```csharp
public SessionState? LoadCachedSessionData(string email)
```
**Purpose:** Retrieve all cached data for a session
**Parameters:** 
- `email`: User's email address
**Returns:** SessionState object or null if not found
**Side Effects:** None (read-only)

**Example:**
```csharp
var cachedState = SessionService.LoadCachedSessionData("john@company.com");
if (cachedState != null)
{
    excelProjectData = cachedState.ProjectData ?? "";
    excelTaskData = cachedState.TaskData ?? "";
    excelTeamData = cachedState.TeamData ?? "";
}
```

### Save Project Data
```csharp
public void SaveProjectData(string email, Guid sessionId, string projectData)
```
**Purpose:** Save project data to cache and remote API
**Parameters:** 
- `email`: User's email
- `sessionId`: Session GUID
- `projectData`: Raw Excel/tab-separated project data
**Returns:** void
**Side Effects:** 
- Updates cache: `_sessionCache[email].ProjectData`
- Updates remote API
- Sets `LastModifiedAt` timestamp

**Example:**
```csharp
SessionService.SaveProjectData(
    "john@company.com",
    Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
    excelProjectData
);
```

### Save Task Data
```csharp
public void SaveTaskData(string email, Guid sessionId, string taskData)
```
**Purpose:** Save task data to cache and remote API
**Parameters:** 
- `email`: User's email
- `sessionId`: Session GUID
- `taskData`: Raw Excel/tab-separated task data
**Returns:** void
**Side Effects:** 
- Updates cache: `_sessionCache[email].TaskData`
- Updates remote API
- Sets `LastModifiedAt` timestamp

**Example:**
```csharp
SessionService.SaveTaskData(
    "john@company.com",
    sessionGuid,
    excelTaskData
);
```

### Save Team Data
```csharp
public void SaveTeamData(string email, Guid sessionId, string teamData)
```
**Purpose:** Save team data to cache and remote API
**Parameters:** 
- `email`: User's email
- `sessionId`: Session GUID
- `teamData`: Raw Excel/tab-separated team data
**Returns:** void
**Side Effects:** 
- Updates cache: `_sessionCache[email].TeamData`
- Updates remote API
- Sets `LastModifiedAt` timestamp

**Example:**
```csharp
SessionService.SaveTeamData(
    "john@company.com",
    sessionGuid,
    excelTeamData
);
```

### Get Session State
```csharp
public SessionState? GetSessionState(string email)
```
**Purpose:** Get the SessionState object for a user
**Parameters:** 
- `email`: User's email address
**Returns:** SessionState or null
**Side Effects:** None

**Example:**
```csharp
var sessionState = SessionService.GetSessionState("john@company.com");
if (sessionState?.IsInitialized ?? false)
{
    Console.WriteLine($"Session ID: {sessionState.SessionId}");
}
```

### Update Session (Remote)
```csharp
public string UpdateSession(string email, Guid sessionId, string serializedObject)
```
**Purpose:** Update remote session with new data
**Parameters:** 
- `email`: User's email
- `sessionId`: Session GUID
- `serializedObject`: JSON or serialized data
**Returns:** Result from remote API
**Side Effects:** 
- Calls remote API
- Updates cache timestamp

**Example:**
```csharp
var result = SessionService.UpdateSession(
    "john@company.com",
    sessionGuid,
    JsonConvert.SerializeObject(input)
);
```

### Store in Cookie (Encrypted)
```csharp
public string StoreSessionContentInCookie(string sessionContent)
```
**Purpose:** Encrypt session content for cookie storage
**Parameters:** 
- `sessionContent`: Content to encrypt
**Returns:** Base64-encoded encrypted content
**Side Effects:** None

**Example:**
```csharp
var encryptedContent = SessionService.StoreSessionContentInCookie(
    JsonConvert.SerializeObject(sessionData)
);
// Now store encryptedContent in browser cookie
```

### Retrieve from Cookie (Decrypted)
```csharp
public string RetrieveSessionContentFromCookie(string encryptedContent)
```
**Purpose:** Decrypt session content from cookie
**Parameters:** 
- `encryptedContent`: Base64-encoded encrypted content
**Returns:** Decrypted original content
**Side Effects:** None
**Throws:** `InvalidOperationException` if decryption fails

**Example:**
```csharp
try
{
    var decrypted = SessionService.RetrieveSessionContentFromCookie(encryptedContent);
    var sessionData = JsonConvert.DeserializeObject(decrypted);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Failed to decrypt: {ex.Message}");
}
```

### Clear Session
```csharp
public void ClearSession(string email)
```
**Purpose:** Remove session from cache (e.g., on logout)
**Parameters:** 
- `email`: User's email address
**Returns:** void
**Side Effects:** 
- Removes from `_sessionCache`
- Does NOT affect remote API

**Example:**
```csharp
SessionService.ClearSession("john@company.com");
// User must re-authenticate on next page load
```

## SessionState Model

```csharp
public class SessionState
{
    public string Email { get; set; } = string.Empty;
    public Guid SessionId { get; set; } = Guid.Empty;

    // Data content
    public string? ProjectData { get; set; }  // Tab-separated project data
    public string? TaskData { get; set; }     // Tab-separated task data
    public string? TeamData { get; set; }     // Tab-separated team data

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public bool IsInitialized { get; set; } = false;
}
```

## Complete Example: Full User Session Lifecycle

```csharp
// 1. USER LOGS IN
var sessionId = SessionService.InitiateSession("john@company.com");
// Cache now contains:
// _sessionCache["john@company.com"] = new SessionState {
//     Email = "john@company.com",
//     SessionId = Guid.Parse(sessionId),
//     IsInitialized = true,
//     CreatedAt = now,
//     ProjectData = null,
//     TaskData = null,
//     TeamData = null
// }

// 2. USER ENTERS PROJECT DATA
excelProjectData = "ProjectID\tProjectName\tProjectGroup\n1\tProj A\t1\n2\tProj B\t1\n";

// 3. USER PARSES PROJECTS
var projects = GanttMapperHelper.ConvertToGanttProjects(excelProjectData);
projectDtoListOutput = projects.AsQueryable();

// 4. SAVE TO CACHE AND API
SessionService.SaveProjectData(
    "john@company.com",
    Guid.Parse(sessionId),
    excelProjectData
);
// Cache now updated:
// _sessionCache["john@company.com"].ProjectData = excelProjectData
// _sessionCache["john@company.com"].LastModifiedAt = now
// Remote API also updated

// 5. USER ENTERS TASK DATA
excelTaskData = "Id\tProjectID\tTaskName\tEffortHours\n1\t1\tTask A\t8\n";

// 6. USER PARSES TASKS
var tasks = GanttMapperHelper.ConvertToGanttTasks(excelTaskData);
taskDtoListOutput = tasks.AsQueryable();

// 7. SAVE TASKS
SessionService.SaveTaskData(
    "john@company.com",
    Guid.Parse(sessionId),
    excelTaskData
);

// 8. USER ENTERS TEAM DATA
excelTeamData = "TeamId\tTeam\tDeveloperId\tName\tDailyWorkHours\n1\tTeam A\t1\tJohn\t8\n";

// 9. USER PARSES TEAM
var team = GanttMapperHelper.ConvertToTeamData(excelTeamData);
teamDtoListOutput = team.AsQueryable();

// 10. SAVE TEAM
SessionService.SaveTeamData(
    "john@company.com",
    Guid.Parse(sessionId),
    excelTeamData
);

// 11. USER GENERATES CHARTS
var input = new CalculateGanttChartAllocationInput { ... };
var results = GanttService.CalculateGanttChartAllocation(input);
// Results displayed on UI

// 12. USER CLOSES BROWSER AND RETURNS LATER
// On page load, same email is used
// LoadCachedSessionData() is called
// All previous data is restored automatically
var cachedState = SessionService.LoadCachedSessionData("john@company.com");
// excelProjectData = cachedState.ProjectData
// excelTaskData = cachedState.TaskData
// excelTeamData = cachedState.TeamData
// All grids are repopulated
```

## Error Handling Best Practices

```csharp
// ✅ GOOD: Try-catch with logging
private async Task ParseProjects()
{
    try
    {
        var tempProjectDtoList = GanttMapperHelper.ConvertToGanttProjects(excelProjectData);
        projectDtoListOutput = tempProjectDtoList.AsQueryable();

        if (sessionIdentifierGuid.HasValue)
        {
            SessionService.SaveProjectData(eMail, sessionIdentifierGuid.Value, excelProjectData);
        }
    }
    catch (Exception ex)
    {
        await JS.InvokeVoidAsync("console.error", $"Error parsing projects: {ex.Message}");
        // UI remains stable, error is logged
    }
}

// ❌ BAD: No error handling
private void ParseProjects()
{
    var tempProjectDtoList = GanttMapperHelper.ConvertToGanttProjects(excelProjectData);
    projectDtoListOutput = tempProjectDtoList.AsQueryable();
    SessionService.SaveProjectData(eMail, sessionIdentifierGuid.Value, excelProjectData);
    // If error occurs, page may crash
}

// ✅ GOOD: Null-safe cache checking
var cachedState = SessionService.LoadCachedSessionData(eMail);
if (cachedState != null && !string.IsNullOrEmpty(cachedState.ProjectData))
{
    excelProjectData = cachedState.ProjectData;
}

// ❌ BAD: Unsafe null access
excelProjectData = SessionService.LoadCachedSessionData(eMail).ProjectData;
// NullReferenceException if no cached data
```

## Testing Scenarios

### Scenario 1: New User (No Cache)
```
1. User enters email "new@company.com"
2. Clicks Initialize Session
3. Session created, GUID returned
4. LoadCachedSessionData returns null
5. All textareas are empty
6. User can enter new data
```

### Scenario 2: Returning User (Has Cache)
```
1. User enters email "returning@company.com"
2. Clicks Initialize Session
3. Session created (or retrieved)
4. LoadCachedSessionData returns previous data
5. All textareas auto-populated with previous data
6. All grids show previous parsed DTOs
7. User can generate charts immediately or modify data
```

### Scenario 3: Concurrent Users
```
1. User A logs in: email = "alice@company.com"
2. User B logs in: email = "bob@company.com"
3. Cache contains: 
   - _sessionCache["alice@company.com"] with Alice's data
   - _sessionCache["bob@company.com"] with Bob's data
4. Thread safety ensured via lock mechanism
5. User A's operations don't affect User B's session
```

### Scenario 4: Session Persistence
```
1. User enters projects, tasks, team data
2. All data saved to cache and API
3. User closes browser/tab
4. User navigates back to page on same device
5. User enters same email
6. All previous data is loaded automatically
7. No need to re-enter data
```

## Troubleshooting

### Issue: Cache not persisting
**Solution:** Cache is in-memory, so it's cleared when application restarts. This is by design. Data persists in the remote API. On next login, data is not auto-loaded unless you implement persistent storage (e.g., database).

### Issue: Multiple tabs/windows lose sync
**Solution:** Each browser tab is independent. Changes in one tab are stored in cache/API but won't immediately reflect in other tabs. Refresh the page to reload from API.

### Issue: Email validation fails
**Solution:** Ensure email is not empty. Component disables button if email is whitespace-only.

### Issue: Cookie encryption fails
**Solution:** Verify Data Protection API is registered in Program.cs: `builder.Services.AddDataProtection();`

### Issue: Session not found in cache
**Solution:** Check that email matches exactly (case-sensitive). SessionState is keyed by email string directly.
