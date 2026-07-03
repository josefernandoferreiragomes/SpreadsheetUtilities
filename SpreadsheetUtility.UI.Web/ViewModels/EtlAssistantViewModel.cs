using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.UI.Web.ViewModels;

public class EtlAssistantViewModel
{
    public string ProjectsInput { get; set; } = string.Empty;
    public string ProjectsOutput { get; set; } = string.Empty;
    public bool ProjectsIsLoading { get; set; }
    public string? ProjectsError { get; set; }
    public bool ProjectsHasResult => !string.IsNullOrEmpty(ProjectsOutput);
    public bool ProjectsIsSuccess { get; set; }

    public string TasksInput { get; set; } = string.Empty;
    public string TasksOutput { get; set; } = string.Empty;
    public bool TasksIsLoading { get; set; }
    public string? TasksError { get; set; }
    public bool TasksHasResult => !string.IsNullOrEmpty(TasksOutput);
    public bool TasksIsSuccess { get; set; }

    public string TeamInput { get; set; } = string.Empty;
    public string TeamOutput { get; set; } = string.Empty;
    public bool TeamIsLoading { get; set; }
    public string? TeamError { get; set; }
    public bool TeamHasResult => !string.IsNullOrEmpty(TeamOutput);
    public bool TeamIsSuccess { get; set; }

    public async Task TransformProjectsAsync(ILlmTransformerService service, CancellationToken ct = default)
    {
        ProjectsIsLoading = true;
        ProjectsError = null;
        ProjectsOutput = string.Empty;
        ProjectsIsSuccess = false;

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
        }
    }

    public async Task TransformTasksAsync(ILlmTransformerService service, CancellationToken ct = default)
    {
        TasksIsLoading = true;
        TasksError = null;
        TasksOutput = string.Empty;
        TasksIsSuccess = false;

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
        }
    }

    public async Task TransformTeamAsync(ILlmTransformerService service, CancellationToken ct = default)
    {
        TeamIsLoading = true;
        TeamError = null;
        TeamOutput = string.Empty;
        TeamIsSuccess = false;

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
        }
    }

    public void Reset()
    {
        ProjectsInput = string.Empty;
        ProjectsOutput = string.Empty;
        ProjectsIsLoading = false;
        ProjectsError = null;
        ProjectsIsSuccess = false;

        TasksInput = string.Empty;
        TasksOutput = string.Empty;
        TasksIsLoading = false;
        TasksError = null;
        TasksIsSuccess = false;

        TeamInput = string.Empty;
        TeamOutput = string.Empty;
        TeamIsLoading = false;
        TeamError = null;
        TeamIsSuccess = false;
    }
}
