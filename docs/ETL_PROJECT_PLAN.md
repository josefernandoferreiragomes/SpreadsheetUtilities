# ETL Assistant — Project Execution Plan

## Objective

Build a Blazor page ("ETL Assistant") that lets users paste arbitrary tabular data in three
separate text areas (one each for projects, tasks, and team data). A local LLM
(qwen2.5-3b-instruct running at http://localhost:1234) transforms each paste into the
corresponding projects.txt, tasks.txt, or team.txt format — or explains why the ETL
is not possible.

## Deliverables

| # | File | Status |
|---|------|--------|
| 1 | docs/ETL_PROJECT_PLAN.md | ✅ |
| 2 | Application/Ports/ILlmTransformerService.cs | ✅ |
| 3 | Infrastructure/Options/LlmOptions.cs | ✅ |
| 4 | Infrastructure/Services/LlmTransformerService.cs | ✅ |
| 5 | UI.Web/ViewModels/EtlAssistantViewModel.cs | ✅ |
| 6 | UI.Web/wwwroot/js/file-download-etl.js | ✅ |
| 7 | UI.Web/Components/Pages/EtlAssistant.razor | ✅ |
| 8 | SpreadsheetUtility.Test/InfrastructureTests/LlmTransformerServiceTests.cs | ✅ |

## Files Modified

| # | File | Change | Status |
|---|------|--------|--------|
| 1 | UI.Web/Components/Layout/NavMenu.razor | Add nav link | ✅ |
| 2 | Infrastructure/DependencyInjection.cs | Register LlmTransformerService + options + HttpClient | ✅ |
| 3 | UI.Web/Program.cs | Register EtlAssistantViewModel | ✅ |
| 4 | UI.Web/appsettings.json | Add Llm config section | ✅ |
| 5 | CHANGELOG.md | Add unreleased entry | ✅ |
| 6 | docs/PROJECT_STRUCTURE.md | Update tables | ✅ |

## Implementation Order

1. Ports (interface + DTO + enum) ✅
2. Options class ✅
3. Service implementation ✅
4. ViewModel ✅
5. JavaScript helper ✅
6. Blazor page ✅
7. NavMenu link ✅
8. DI registrations + config ✅
9. Tests ✅
10. Build → Test → Smoke → Review → Governance → Commit ✅

## Target Schemas (LLM Output)

### projects.txt
ProjectID\tProject Name\tProject Group Id\tTeam Id

### tasks.txt
ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID

### team.txt
Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours

## LLM Protocol

If the LLM cannot extract sufficient data for the target schema, it must prefix its
response with "IMPOSSIBLE:|" followed by a plain-English explanation.

## Bug Fixes Applied

| # | Issue | Fix | Date |
|---|-------|-----|------|
| 1 | `LlmResponse.Output` deserialization failure — LM Studio returns `output` as an array of `{ type, content }` objects, but code expected a flat `string?` | Changed `Output` to `List<LlmOutputMessage>?`, extract text from `outputs[0].Content`. Added `JsonException` catch block. Retained `response` field fallback for backward compatibility. | 2026-07-03 |
| 2 | LLM produced wrong column headers (e.g., `Name` instead of `Project Name`, `GroupId` instead of `Project Group Id`) | Added to all 3 prompts: (a) CRITICAL header-exactness instruction, (b) column mapping table (common input → expected output), (c) concrete worked example | 2026-07-03 |

---

# Phase 2 — ETL Assistant Enhancements

## Features

| # | Feature | Classification | Est. Files Changed |
|---|---------|---------------|-------------------|
| F1 | Post-transform validation with status badge | Small Feature | 4 |
| F2 | Per-card reset button | Tiny Fix | 1 |
| F3 | Sample data loader | Tiny Fix | 1 |
| F4 | Copy-to-clipboard button | Tiny Fix | 2 |
| F5 | Save all to session | Small Feature | 3 |

**Total pipeline**: Two Small Features + three Tiny Fixes. Tiny Fixes are implemented directly (no branch/review/governance). Small Features get the full pipeline.

---

## F1 — Post-Transform Validation with Status Badge

### What
After the LLM returns output, validate that the tab-separated text matches the expected schema for the target format. Show a highlighted badge below the output textarea.

### Validation rules (for all 3 formats)
1. Output must not be empty
2. First line (header) must match the expected exact header for the format, split by tabs and trimmed
3. Every subsequent line must have the correct number of tab-separated columns
4. Blank lines are ignored

### Implementation

**New file**: `SpreadsheetUtility.Application/Validators/EtlOutputValidator.cs`

```csharp
public static class EtlOutputValidator
{
    private static readonly Dictionary<TargetFormat, string[]> ExpectedHeaders = new()
    {
        [TargetFormat.Projects] = ["ProjectID", "Project Name", "Project Group Id", "Team Id"],
        [TargetFormat.Tasks]    = ["ID", "Project Id", "ProjectName", "TaskName",
                                    "EstimatedEffortHours", "Dependencies", "Progress", "InternalID"],
        [TargetFormat.Team]     = ["Team ID", "Team Name", "Developer Id", "Developer Name",
                                    "Developer Vacation Date Intervals", "Daily Work Hours"],
    };

    public static ValidationResult Validate(string output, TargetFormat format) { ... }
}

public record ValidationResult(bool IsValid, string[] Errors);
```

**Validation logic**:
- Split output by `\n` or `\r\n`, filter blank lines
- First non-blank line: split by `\t`, compare to `ExpectedHeaders[format]` element-by-element (case-sensitive)
- All remaining non-blank lines: split by `\t`, verify length matches header count
- Return `ValidationResult` with specific errors (e.g. "Header mismatch: column 2 expected 'Project Name', got 'Name'")

**ViewModel changes** (`EtlAssistantViewModel.cs`):
```csharp
// Per-card validation state
public string? ProjectsValidationMessage { get; set; }
public bool ProjectsIsValid { get; set; }    // true = output validated OK
// Same for Tasks, Team

// Called after LLM returns
private void ValidateProjects()
{
    var result = EtlOutputValidator.Validate(ProjectsOutput, TargetFormat.Projects);
    ProjectsIsValid = result.IsValid;
    ProjectsValidationMessage = result.IsValid ? "✓ Validated — Schema OK"
                                                : $"✗ Validation Failed: {string.Join("; ", result.Errors)}";
}
```

**Razor changes** (`EtlAssistant.razor`):
- Below output textarea, add a validation badge:
```razor
@if (!string.IsNullOrEmpty(ViewModel.ProjectsOutput))
{
    <div class="@(ViewModel.ProjectsIsValid ? "validation-badge valid" : "validation-badge invalid")">
        @ViewModel.ProjectsValidationMessage
    </div>
}
```

**CSS**:
```css
.validation-badge { padding: 4px 10px; border-radius: 4px; font-size: 0.8rem; margin-top: 6px; }
.validation-badge.valid { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
.validation-badge.invalid { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
```

### Files changed
| File | Change |
|------|--------|
| `Application/Validators/EtlOutputValidator.cs` | **New** — static validator class |
| `UI.Web/ViewModels/EtlAssistantViewModel.cs` | Add validation properties + call validator after transform |
| `UI.Web/Components/Pages/EtlAssistant.razor` | Render validation badge |
| `SpreadsheetUtility.Test/.../EtlOutputValidatorTests.cs` | **New** — unit tests for validator |

---

## F2 — Per-Card Reset Button

### What
Add a secondary "Reset" button to each card that clears only that card's input, output, and validation state.

### Implementation

**ViewModel** — three new methods:
```csharp
public void ResetProjects()
{
    ProjectsInput = string.Empty;
    ProjectsOutput = string.Empty;
    ProjectsError = null;
    ProjectsIsLoading = false;
    ProjectsIsSuccess = false;
    ProjectsIsValid = false;
    ProjectsValidationMessage = null;
}
// Same for ResetTasks(), ResetTeam()
```

**Razor** — add reset button next to Transform button in each card:
```razor
@if (!string.IsNullOrEmpty(ViewModel.ProjectsInput) || !string.IsNullOrEmpty(ViewModel.ProjectsOutput))
{
    <button class="btn btn-sm btn-outline-danger ms-2" @onclick="ResetProjects">
        &#x1F5D1; Reset
    </button>
}
```

### Files changed
| File | Change |
|------|--------|
| `UI.Web/ViewModels/EtlAssistantViewModel.cs` | Add 3 reset methods |
| `UI.Web/Components/Pages/EtlAssistant.razor` | Add reset buttons |

---

## F3 — Sample Data Loader

### What
Add a "Load Sample" link/button per card that fills the input textarea with representative sample data so users can test the feature instantly.

### Sample data definitions

**Projects**:
```
ProjectIdentifier	Name	Group	Team
1	Project Alpha	1	1
2	Project Beta	1	2
3	Project Gamma	2	1
```

**Tasks**:
```
ID	Project	Name	Task	Effort	Dep	Progress
T1	1	Project Alpha	Design database	40		0
T2	1	Project Alpha	Build API		30	T1	50
T3	2	Project Beta	Create UI		20		10
```

**Team**:
```
TeamId	Name	DeveloperId	Developer	Vacation	Hours
1	Team Alpha	101	Alice		8
1	Team Alpha	102	Bob	2026-07-20|2026-07-25	8
2	Team Beta		201	Charlie		6
```

### Implementation

**ViewModel**:
```csharp
private const string SampleProjectsData = "ProjectIdentifier\tName\tGroup\tTeam\n1\tProject Alpha\t1\t1\n...";
private const string SampleTasksData = "...";
private const string SampleTeamData = "...";

public void LoadSampleProjects() => ProjectsInput = SampleProjectsData;
public void LoadSampleTasks() => TasksInput = SampleTasksData;
public void LoadSampleTeam() => TeamInput = SampleTeamData;
```

**Razor** — add subtle "Load Sample" link below the input textarea:
```razor
<div class="mt-1">
    <button class="btn btn-sm btn-link p-0" @onclick="LoadSampleProjects">
        &#x1F4CB; Load sample data
    </button>
</div>
```

### Files changed
| File | Change |
|------|--------|
| `UI.Web/ViewModels/EtlAssistantViewModel.cs` | Add sample data constants + load methods |
| `UI.Web/Components/Pages/EtlAssistant.razor` | Add load sample buttons |

---

## F4 — Copy-to-Clipboard Button

### What
Add a "Copy" button alongside the existing "Download" button in each card's output section, using JS interop to copy output text to clipboard.

### Implementation

**JS interop** — extend `file-download-etl.js`:
```javascript
window.copyToClipboard = function (text) {
    navigator.clipboard.writeText(text).catch(function (err) {
        console.error('Copy failed: ', err);
    });
};
```

**Razor** — add Copy button next to Download:
```razor
@if (ViewModel.ProjectsIsSuccess)
{
    <button class="btn btn-sm btn-outline-secondary mt-2" @onclick="() => CopyProjects()">
        &#x1F4CB; Copy
    </button>
    <button class="btn btn-sm btn-outline-secondary mt-2" @onclick="() => DownloadProjects()">
        &#x1F4E5; Download projects.txt
    </button>
}
```

**Page code**:
```csharp
private async Task CopyProjects()
{
    await JS.InvokeVoidAsync("copyToClipboard", ViewModel.ProjectsOutput);
}
```

### Files changed
| File | Change |
|------|--------|
| `UI.Web/wwwroot/js/file-download-etl.js` | Add `copyToClipboard()` function |
| `UI.Web/Components/Pages/EtlAssistant.razor` | Add copy buttons + handler methods |

---

## F5 — Save All to Session

### What
A "Save All to Session" button that persists all three validated outputs into the app's session storage infrastructure (`SessionService`), making the data available on the Gantt generator page. The button is disabled until all three cards have passed validation.

### Design Decision

The ETL Assistant needs a session to save into. Following the existing pattern in `GanttGeneratorFromPaste.razor`, the page will include a minimal session initialization section (email input + "Initialize Session" button) at the top. The "Save All to Session" button sits below the three cards.

### Flow

```
User enters email → "Initialize Session"
    ↓
SessionService.InitiateSession(email) returns sessionId
    ↓
Three ETL cards become active (or are already filled)
    ↓
User transforms each card → validation runs automatically
    ↓
All 3 validated → "Save All to Session" button enables (green)
    ↓
User clicks → SessionService.SaveProjectData(email, guid, output)
              SessionService.SaveTaskData(email, guid, output)
              SessionService.SaveTeamData(email, guid, output)
    ↓
Success message: "✓ Data saved! Go to Gantt Generator →"
```

### Implementation

**ViewModel additions**:
```csharp
// Session state
public string? Email { get; set; }
public Guid? SessionId { get; set; }
public bool IsSessionInitialized { get; set; }
public string? SessionMessage { get; set; }

// Computed
public bool CanSaveAll =>
    IsSessionInitialized &&
    ProjectsIsValid && TasksIsValid && TeamIsValid &&
    ProjectsIsSuccess && TasksIsSuccess && TeamIsSuccess;

// Methods
public async Task InitializeSessionAsync(SessionService sessionService)
{
    var sessionIdStr = sessionService.InitiateSession(Email!);
    SessionId = Guid.Parse(sessionIdStr);
    IsSessionInitialized = true;
    SessionMessage = "Session initialized.";
}

public async Task SaveAllToSessionAsync(SessionService sessionService)
{
    if (SessionId is null || Email is null) return;
    sessionService.SaveProjectData(Email, SessionId.Value, ProjectsOutput);
    sessionService.SaveTaskData(Email, SessionId.Value, TasksOutput);
    sessionService.SaveTeamData(Email, SessionId.Value, TeamOutput);
    SessionMessage = "✅ All data saved to session! Go to Gantt Generator to generate charts.";
}
```

Note: `SessionService` is already registered as scoped in `Program.cs`. The ViewModel will receive it via constructor injection (or via method parameter following the existing `ILlmTransformerService` pattern where the Razor page passes it).

**Razor additions** — session init section at top of page:
```razor
@* Session Initialization *@
<div class="card mb-4">
    <div class="card-body">
        <div class="row align-items-end">
            <div class="col-auto">
                <label class="form-label">Email</label>
                <InputText @bind-Value="ViewModel.Email" class="form-control"
                           placeholder="your@email.com" />
            </div>
            <div class="col-auto">
                <button class="btn btn-outline-primary"
                        disabled="@(string.IsNullOrWhiteSpace(ViewModel.Email) || ViewModel.IsSessionInitialized)"
                        @onclick="InitializeSession">
                    Initialize Session
                </button>
            </div>
        </div>
        @if (ViewModel.IsSessionInitialized)
        {
            <div class="mt-2 text-success">&#x2705; Session initialized</div>
        }
    </div>
</div>
```

Save All button below the three cards:
```razor
<div class="d-flex justify-content-end mt-4">
    <button class="btn btn-success btn-lg"
            disabled="@(!ViewModel.CanSaveAll)"
            @onclick="SaveAllToSession">
        &#x1F4BE; Save All to Session
    </button>
</div>
@if (ViewModel.SessionMessage is not null)
{
    <div class="alert alert-success mt-2">@ViewModel.SessionMessage</div>
}
```

### Files changed
| File | Change |
|------|--------|
| `UI.Web/ViewModels/EtlAssistantViewModel.cs` | Add session state props, CanSaveAll, init/save methods |
| `UI.Web/Components/Pages/EtlAssistant.razor` | Add session init section + Save All button |
| `UI.Web/Program.cs` | Potentially inject SessionService into ViewModel |

---

## Pipeline Execution Order

Since F1 (validation) is a dependency for F5 (save requires validation), and F2/F3/F4 are independent, the recommended order is:

1. **F1** — Post-transform validation (Small Feature: build, test, review, changelog)
2. **F2** — Per-card reset (Tiny Fix: build, test only)
3. **F3** — Sample data loader (Tiny Fix: build, test only)
4. **F4** — Copy button (Tiny Fix: build, test only)
5. **F5** — Save all to session (Small Feature: build, test, review, changelog)

## Estimated Test Impact

| Feature | New Tests | Total After |
|---------|-----------|-------------|
| F1 — Validator | ~8 (valid cases + invalid cases per format) | 89 |
| F2 — Reset | 0 (pure UI change) | 89 |
| F3 — Sample data | 0 (constants + assignments) | 89 |
| F4 — Copy | 0 (JS interop, no unit tests needed) | 89 |
| F5 — Save all | ~2 (ViewModel state transitions) | 91 |
