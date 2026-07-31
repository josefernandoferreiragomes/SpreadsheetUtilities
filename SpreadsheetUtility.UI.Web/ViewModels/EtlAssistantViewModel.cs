using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.Validators;
using SpreadsheetUtility.Infrastructure.Services;

namespace SpreadsheetUtility.UI.Web.ViewModels;

public class EtlAssistantViewModel
{
    // ── Projects ──────────────────────────────────────────────────
    public string ProjectsInput { get; set; } = string.Empty;
    public string ProjectsOutput { get; set; } = string.Empty;
    public bool ProjectsIsLoading { get; set; }
    public string? ProjectsError { get; set; }
    public bool ProjectsHasResult => !string.IsNullOrEmpty(ProjectsOutput);
    public bool ProjectsIsSuccess { get; set; }
    public bool ProjectsIsValid { get; set; }
    public string? ProjectsValidationMessage { get; set; }

    // ── Tasks ─────────────────────────────────────────────────────
    public string TasksInput { get; set; } = string.Empty;
    public string TasksOutput { get; set; } = string.Empty;
    public bool TasksIsLoading { get; set; }
    public string? TasksError { get; set; }
    public bool TasksHasResult => !string.IsNullOrEmpty(TasksOutput);
    public bool TasksIsSuccess { get; set; }
    public bool TasksIsValid { get; set; }
    public string? TasksValidationMessage { get; set; }

    // ── Team ──────────────────────────────────────────────────────
    public string TeamInput { get; set; } = string.Empty;
    public string TeamOutput { get; set; } = string.Empty;
    public bool TeamIsLoading { get; set; }
    public string? TeamError { get; set; }
    public bool TeamHasResult => !string.IsNullOrEmpty(TeamOutput);
    public bool TeamIsSuccess { get; set; }
    public bool TeamIsValid { get; set; }
    public string? TeamValidationMessage { get; set; }

    // ── Session (F5) ──────────────────────────────────────────────
    public string? Email { get; set; }
    public Guid? SessionId { get; set; }
    public bool IsSessionInitialized { get; set; }
    public string? SessionMessage { get; set; }

    public bool CanSaveAll =>
        IsSessionInitialized &&
        ProjectsIsValid && TasksIsValid && TeamIsValid &&
        ProjectsIsSuccess && TasksIsSuccess && TeamIsSuccess;

    // ── Sample data constants (F3) ────────────────────────────────
    private const string SampleProjectsData =
        "ProjectIdentifier\tName\tGroup\tTeam\n" +
        "1\tProject Alpha\t1\t1\n" +
        "2\tProject Beta\t1\t2\n" +
        "3\tProject Gamma\t2\t1";

    private const string SampleTasksData =
        "ID\tProject\tName\tTask\tEffort\tDep\tProgress\n" +
        "T1\t1\tProject Alpha\tDesign database\t40\t\t0\n" +
        "T2\t1\tProject Alpha\tBuild API\t30\tT1\t50\n" +
        "T3\t2\tProject Beta\tCreate UI\t20\t\t10";

    private const string SampleTeamData =
        "TeamId\tName\tDeveloperId\tDeveloper\tVacation\tHours\n" +
        "1\tTeam Alpha\t101\tAlice\t\t8\n" +
        "1\tTeam Alpha\t102\tBob\t2026-07-20|2026-07-25\t8\n" +
        "2\tTeam Beta\t201\tCharlie\t\t6";

    // ── Transform methods ─────────────────────────────────────────
    public async Task TransformProjectsAsync(ILlmTransformerService service, CancellationToken ct = default)
    {
        ProjectsIsLoading = true;
        ProjectsError = null;
        ProjectsOutput = string.Empty;
        ProjectsIsSuccess = false;
        ProjectsIsValid = false;
        ProjectsValidationMessage = null;

        try
        {
            var result = await service.TransformAsync(ProjectsInput, TargetFormat.Projects, ct);
            ProjectsIsSuccess = result.IsPossible;
            ProjectsOutput = result.IsPossible ? result.Output : string.Empty;
            if (!result.IsPossible)
            {
                ProjectsError = result.ErrorReason;
            }
        }
        catch (Exception ex)
        {
            ProjectsError = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            ProjectsIsLoading = false;
            ValidateProjects();
        }
    }

    public async Task TransformTasksAsync(ILlmTransformerService service, CancellationToken ct = default)
    {
        TasksIsLoading = true;
        TasksError = null;
        TasksOutput = string.Empty;
        TasksIsSuccess = false;
        TasksIsValid = false;
        TasksValidationMessage = null;

        try
        {
            var result = await service.TransformAsync(TasksInput, TargetFormat.Tasks, ct);
            TasksIsSuccess = result.IsPossible;
            TasksOutput = result.IsPossible ? result.Output : string.Empty;
            if (!result.IsPossible)
            {
                TasksError = result.ErrorReason;
            }
        }
        catch (Exception ex)
        {
            TasksError = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            TasksIsLoading = false;
            ValidateTasks();
        }
    }

    public async Task TransformTeamAsync(ILlmTransformerService service, CancellationToken ct = default)
    {
        TeamIsLoading = true;
        TeamError = null;
        TeamOutput = string.Empty;
        TeamIsSuccess = false;
        TeamIsValid = false;
        TeamValidationMessage = null;

        try
        {
            var result = await service.TransformAsync(TeamInput, TargetFormat.Team, ct);
            TeamIsSuccess = result.IsPossible;
            TeamOutput = result.IsPossible ? result.Output : string.Empty;
            if (!result.IsPossible)
            {
                TeamError = result.ErrorReason;
            }
        }
        catch (Exception ex)
        {
            TeamError = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            TeamIsLoading = false;
            ValidateTeam();
        }
    }

    // ── Validation (F1) ───────────────────────────────────────────
    private void ValidateProjects()
    {
        if (string.IsNullOrEmpty(ProjectsOutput) || !ProjectsIsSuccess)
        {
            ProjectsIsValid = false;
            ProjectsValidationMessage = null;
            return;
        }

        var result = EtlOutputValidator.Validate(ProjectsOutput, TargetFormat.Projects);
        ProjectsIsValid = result.IsValid;
        ProjectsValidationMessage = result.IsValid
            ? "✓ Validated — Schema OK"
            : $"✗ Validation Failed: {string.Join("; ", result.Errors)}";
    }

    private void ValidateTasks()
    {
        if (string.IsNullOrEmpty(TasksOutput) || !TasksIsSuccess)
        {
            TasksIsValid = false;
            TasksValidationMessage = null;
            return;
        }

        var result = EtlOutputValidator.Validate(TasksOutput, TargetFormat.Tasks);
        TasksIsValid = result.IsValid;
        TasksValidationMessage = result.IsValid
            ? "✓ Validated — Schema OK"
            : $"✗ Validation Failed: {string.Join("; ", result.Errors)}";
    }

    private void ValidateTeam()
    {
        if (string.IsNullOrEmpty(TeamOutput) || !TeamIsSuccess)
        {
            TeamIsValid = false;
            TeamValidationMessage = null;
            return;
        }

        var result = EtlOutputValidator.Validate(TeamOutput, TargetFormat.Team);
        TeamIsValid = result.IsValid;
        TeamValidationMessage = result.IsValid
            ? "✓ Validated — Schema OK"
            : $"✗ Validation Failed: {string.Join("; ", result.Errors)}";
    }

    // ── Reset per card (F2) ───────────────────────────────────────
    public void ResetProjects()
    {
        ProjectsInput = string.Empty;
        ProjectsOutput = string.Empty;
        ProjectsIsLoading = false;
        ProjectsError = null;
        ProjectsIsSuccess = false;
        ProjectsIsValid = false;
        ProjectsValidationMessage = null;
    }

    public void ResetTasks()
    {
        TasksInput = string.Empty;
        TasksOutput = string.Empty;
        TasksIsLoading = false;
        TasksError = null;
        TasksIsSuccess = false;
        TasksIsValid = false;
        TasksValidationMessage = null;
    }

    public void ResetTeam()
    {
        TeamInput = string.Empty;
        TeamOutput = string.Empty;
        TeamIsLoading = false;
        TeamError = null;
        TeamIsSuccess = false;
        TeamIsValid = false;
        TeamValidationMessage = null;
    }

    // ── Sample data loaders (F3) ──────────────────────────────────
    public void LoadSampleProjects() => ProjectsInput = SampleProjectsData;
    public void LoadSampleTasks() => TasksInput = SampleTasksData;
    public void LoadSampleTeam() => TeamInput = SampleTeamData;

    // ── Session (F5) ──────────────────────────────────────────────
    public async Task InitializeSessionAsync(SessionService sessionService)
    {
        if (string.IsNullOrWhiteSpace(Email))
            return;

        var sessionIdStr = sessionService.InitiateSession(Email);
        SessionId = Guid.Parse(sessionIdStr);
        IsSessionInitialized = true;
        SessionMessage = "Session initialized.";
    }

    public void SaveAllToSession(SessionService sessionService)
    {
        if (SessionId is null || string.IsNullOrWhiteSpace(Email))
            return;

        sessionService.SaveProjectData(Email, SessionId.Value, ProjectsOutput);
        sessionService.SaveTaskData(Email, SessionId.Value, TasksOutput);
        sessionService.SaveTeamData(Email, SessionId.Value, TeamOutput);
        SessionMessage = "✅ All data saved to session! Go to Gantt Generator to generate charts.";
    }

    // ── Full reset ────────────────────────────────────────────────
    public void Reset()
    {
        ResetProjects();
        ResetTasks();
        ResetTeam();

        Email = null;
        SessionId = null;
        IsSessionInitialized = false;
        SessionMessage = null;
    }
}
