# Session Management Architecture & Flow Diagrams

## Session Identification Architecture

```
User Email (Primary Identifier)
        ↓
  SessionService Cache
   (Dictionary<string, SessionState>)
        ↓
    Session GUID (Secondary Identifier)
        ↓
 Remote API Session Storage
        ↓
   Session Data
 (Projects, Tasks, Teams)
```

## Complete User Journey

```
┌─────────────────────────────────────────────────────────────────┐
│                         PAGE LOAD                               │
│  - Only Email Input + Session Status Alert visible              │
│  - Main content hidden (isSessionInitialized = false)           │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                 USER ENTERS EMAIL & CLICKS INIT               │
│  - Email validation occurs                                      │
│  - SessionService.InitiateSession(email) called                 │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│              SESSION INITIALIZATION IN CACHE                    │
│  - Remote API creates new GUID                                  │
│  - New SessionState object created                              │
│  - Stored in _sessionCache[email]                               │
│  - SessionId = GUID, IsInitialized = true                       │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│              MAIN CONTENT BECOMES VISIBLE                       │
│  - All sections now shown (Projects, Tasks, Team)               │
│  - User can see: Projects textarea + grid                       │
│  - User can see: Tasks textarea + grid                          │
│  - User can see: Team textarea + grid                           │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│           ATTEMPT TO LOAD CACHED SESSION DATA                   │
│  - LoadCachedSessionData() is called                             │
│  - SessionService.LoadCachedSessionData(email)                  │
│  - If data exists:                                              │
│    • excelProjectData populated from cache                      │
│    • projectDtoListOutput populated from cache                  │
│    • Similar for tasks and team                                 │
│  - Textareas and grids show previous data                       │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│          USER PASTES DATA & PARSES (PROJECTS EXAMPLE)           │
│  - User pastes project data into textarea                       │
│  - Clicks "Parse Projects" button                               │
│  - ParseProjects() method called                                │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│             DATA PARSING & CACHE STORAGE                        │
│  1. GanttMapperHelper.ConvertToGanttProjects()                  │
│  2. projectDtoListOutput updated with DTOs                      │
│  3. Grid displays parsed data                                   │
│  4. SessionService.SaveProjectData() called                     │
│     a. Cache updated: sessionState.ProjectData = excelData      │
│     b. LastModifiedAt = DateTime.UtcNow                         │
│     c. Remote API also updated via UpdateSession()              │
└─────────────────────────────────────────────────────────────────┘
                              ↓
        ┌─────────────┬──────────────┬──────────────┐
        ↓             ↓              ↓              ↓
    [Projects]   [Tasks]       [Teams]     [Generate Charts]
       (same      (same)        (same)          ↓
      flow)       flow)         flow)    └──────────────────────┐
                                         │ LoadGanttChartTasks()│
                                         │ • Get all data       │
                                         │ • Call GanttService  │
                                         │ • Render charts      │
                                         └──────────────────────┘
```

## In-Memory Cache Structure

```
SessionService._sessionCache
│
├─ "user1@example.com" → SessionState
│  ├─ Email: "user1@example.com"
│  ├─ SessionId: Guid (ABC-123...)
│  ├─ ProjectData: "Col1\tCol2\n..."
│  ├─ TaskData: "Col1\tCol2\n..."
│  ├─ TeamData: "Col1\tCol2\n..."
│  ├─ CreatedAt: 2024-01-01 10:00:00
│  ├─ LastModifiedAt: 2024-01-01 10:15:00
│  └─ IsInitialized: true
│
├─ "user2@example.com" → SessionState
│  └─ (similar structure)
│
└─ "user3@example.com" → SessionState
   └─ (similar structure)
```

## Cache Synchronization Flow

```
┌────────────────────────────────┐
│  User Parses Project Data      │
└────────────────────────────────┘
         ↓
┌────────────────────────────────┐
│  ParseProjects() called         │
└────────────────────────────────┘
         ↓
┌────────────────────────────────────────────────────┐
│  SessionService.SaveProjectData() called           │
│  (email, sessionId, projectData)                   │
└────────────────────────────────────────────────────┘
         ↓
    ┌────────────────────────────────────────┐
    │  lock (_cacheLock)                     │
    │  {                                     │
    │    _sessionCache[email]                │
    │      .ProjectData = projectData        │
    │      .LastModifiedAt = Now             │
    │  }                                     │
    └────────────────────────────────────────┘
         ↓
┌──────────────────────────────────────────────────┐
│  UpdateSession() called                          │
│  - Remote API updated                            │
│  - Cache marked as recently modified             │
└──────────────────────────────────────────────────┘
```

## Thread Safety Mechanism

```
Multiple Users (Different Emails)
       ↓
    ┌─ lock (_cacheLock) ─┐
    │                     │
 User1           User2         User3
 email@1         email@2       email@3
    │                     │
    └─ Critical Section ──┘
       (Only one thread
        modifies cache
        at a time)
```

## Data Flow: Parse → Cache → Display

```
┌─────────────────┐
│  Excel Data     │
│  (Tab/Comma sep)│
└────────┬────────┘
         ↓
┌────────────────────────────┐
│  ParseProjects()           │
└────────┬───────────────────┘
         ↓
┌────────────────────────────┐
│  GanttMapperHelper.        │
│  ConvertToGanttProjects()  │
└────────┬───────────────────┘
         ↓
   ┌─────────────┬──────────────┐
   ↓             ↓              ↓
┌─────────┐ ┌──────────┐ ┌────────────┐
│ Cache   │ │  Grid    │ │ Serialized │
│ Storage │ │ Display  │ │ to JSON    │
└────┬────┘ └──────────┘ └────────────┘
     ↓
┌──────────────────────────┐
│ Remote API Update        │
│ (via UpdateSession)      │
└──────────────────────────┘
```

## UI Visibility States

```
State 1: Before Login
├─ Email Input: ✓ Visible, Enabled
├─ Init Button: ✓ Visible, Enabled
├─ Status Alert: ✓ Visible (shows "Please enter email...")
├─ Projects Section: ✗ Hidden
├─ Tasks Section: ✗ Hidden
├─ Team Section: ✗ Hidden
├─ Chart Config: ✗ Hidden
└─ Chart Results: ✗ Hidden

         ↓ (Click Initialize Session)

State 2: After Login (Before Chart Generation)
├─ Email Input: ✓ Visible, DISABLED
├─ Init Button: ✓ Visible, DISABLED
├─ Status Alert: ✓ Visible (shows "Logged in as...")
├─ Projects Section: ✓ Visible
├─ Tasks Section: ✓ Visible
├─ Team Section: ✓ Visible
├─ Chart Config: ✓ Visible
└─ Chart Results: ✗ Hidden

    ↓ (Click Generate Gantt Charts)

State 3: After Chart Generation
├─ Email Input: ✓ Visible, DISABLED
├─ Init Button: ✓ Visible, DISABLED
├─ Status Alert: ✓ Visible
├─ Projects Section: ✓ Visible
├─ Tasks Section: ✓ Visible
├─ Team Section: ✓ Visible
├─ Chart Config: ✓ Visible
└─ Chart Results: ✓ Visible (Charts + Grids)
```

## Error Handling Flow

```
User Action (e.g., Parse Projects)
     ↓
try {
  Parse Data
  Update Cache
  Update UI
}
     ↓
catch (Exception ex)
  ↓
JS.InvokeVoidAsync("console.error", message)
  ↓
Browser Console shows error
User sees graceful degradation
No page crash
```

## Security: Cookie Encryption Flow

```
Session Content (JSON String)
     ↓
┌─────────────────────────────────────┐
│ StoreSessionContentInCookie()        │
│ • UTF8.GetBytes(content)            │
│ • protector.Protect(bytes)          │
│ • Convert.ToBase64String()          │
└─────────────────────────────────────┘
     ↓
Encrypted Base64 String
     ↓
Set in Browser Cookie
     ↓
Later...
     ↓
┌──────────────────────────────────────┐
│ RetrieveSessionContentFromCookie()    │
│ • Convert.FromBase64String()         │
│ • protector.Unprotect(bytes)         │
│ • UTF8.GetString(bytes)              │
└──────────────────────────────────────┘
     ↓
Original Session Content Recovered
```

## Key Mappings

```
Primary Key (Email)
├─ User: "john@company.com"
│  └─→ Secondary Key (GUID): "550e8400-e29b-41d4-a716-446655440000"
│      └─→ Session Data
│          ├─ ProjectData: {...}
│          ├─ TaskData: {...}
│          └─ TeamData: {...}
│
├─ User: "jane@company.com"
│  └─→ Secondary Key (GUID): "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
│      └─→ Session Data
│          ├─ ProjectData: {...}
│          ├─ TaskData: {...}
│          └─ TeamData: {...}
```

This architecture ensures:
✅ Fast in-memory access
✅ Thread-safe operations
✅ Email-to-GUID mapping
✅ Automatic cache synchronization
✅ Secure cookie storage
