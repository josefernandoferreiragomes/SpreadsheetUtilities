using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Infrastructure.Options;

namespace SpreadsheetUtility.Infrastructure.Services;

public class LlmTransformerService : ILlmTransformerService
{
    private readonly HttpClient _httpClient;
    private readonly LlmOptions _options;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    public LlmTransformerService(HttpClient httpClient, IOptions<LlmOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<LlmTransformationResult> TransformAsync(
        string inputData,
        TargetFormat targetFormat,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(inputData))
        {
            return new LlmTransformationResult(false, string.Empty, "Input data is empty. Please paste some data to transform.");
        }

        var systemPrompt = GetSystemPrompt(targetFormat);

        var requestBody = new
        {
            model = _options.Model,
            system_prompt = systemPrompt,
            input = inputData
        };

        HttpResponseMessage response;
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_options.TimeoutSeconds));
            response = await _httpClient.PostAsJsonAsync("/api/v1/chat", requestBody, _jsonOptions, cts.Token);
            response.EnsureSuccessStatusCode();
        }
        catch (TaskCanceledException)
        {
            return new LlmTransformationResult(false, string.Empty, $"The request timed out after {_options.TimeoutSeconds} seconds. The local LLM may be overloaded or not running.");
        }
        catch (HttpRequestException ex)
        {
            return new LlmTransformationResult(false, string.Empty, $"Failed to reach the LLM at {_options.BaseUrl}. Is the server running? Details: {ex.Message}");
        }

        string llmOutput;
        try
        {
            var responseBody = await response.Content.ReadFromJsonAsync<LlmResponse>(_jsonOptions, cancellationToken);

            if (responseBody?.Output is { Count: > 0 } outputs && !string.IsNullOrWhiteSpace(outputs[0].Content))
            {
                llmOutput = outputs[0].Content;
            }
            else if (!string.IsNullOrWhiteSpace(responseBody?.Response))
            {
                llmOutput = responseBody.Response;
            }
            else
            {
                return new LlmTransformationResult(false, string.Empty, "The LLM returned an empty response.");
            }
        }
        catch (JsonException ex)
        {
            return new LlmTransformationResult(false, string.Empty, $"Failed to parse the LLM response. The server may have returned an unexpected format. Details: {ex.Message}");
        }

        // Check if the LLM flagged it as impossible
        var trimmed = llmOutput.Trim();
        if (trimmed.StartsWith("IMPOSSIBLE:|", StringComparison.OrdinalIgnoreCase))
        {
            var reason = trimmed["IMPOSSIBLE:|".Length..].Trim();
            return new LlmTransformationResult(false, string.Empty, reason);
        }

        return new LlmTransformationResult(true, llmOutput, null);
    }

    private static string GetSystemPrompt(TargetFormat format)
    {
        return format switch
        {
            TargetFormat.Projects => GetProjectsPrompt(),
            TargetFormat.Tasks => GetTasksPrompt(),
            TargetFormat.Team => GetTeamPrompt(),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
    }

    private static string GetProjectsPrompt()
    {
        return "You are a data transformation assistant. "
            + "Given arbitrary tabular data (CSV, TSV, markdown table, or other delimited format), "
            + "extract project information and output it EXACTLY in the following tab-separated "
            + "format with a header row:\n\n"
            + "ProjectID\tProject Name\tProject Group Id\tTeam Id\n\n"
            + "CRITICAL: You MUST output the header row EXACTLY as shown above, character for character. "
            + "NEVER rename columns based on the input headers. "
            + "For example, regardless of whether the input columns are called Name, ProjectName, "
            + "or Project Name, the output header MUST be \"Project Name\".\n\n"
            + "Column mapping guide (common input column → output header):\n"
            + "- ProjectIdentifier, ProjectID, ID, Id, Project Id → ProjectID\n"
            + "- Name, ProjectName, Project Name, Project → Project Name\n"
            + "- Group, GroupId, GroupID, ProjectGroupId, Project Group → Project Group Id\n"
            + "- Team, TeamId, TeamID, Team Id → Team Id\n\n"
            + "Example:\n"
            + "Input:\n"
            + "ProjectIdentifier\tName\tGroup\tTeam\n"
            + "1\tProj A\t1\t1\n\n"
            + "Output:\n"
            + "ProjectID\tProject Name\tProject Group Id\tTeam Id\n"
            + "1\tProj A\t1\t1\n\n"
            + "Rules:\n"
            + "1. Always output the exact header row first.\n"
            + "2. Each subsequent row must have exactly 4 tab-separated columns, in the order specified by the header.\n"
            + "3. Omit any data that is not project-related (e.g., tasks, team members).\n"
            + "4. Preserve the order of projects as they appear in the input.\n"
            + "5. Do not add any explanation, commentary, or formatting - output only the tab-separated table.\n"
            + "6. If the input data does not contain enough columns or meaningful project data to produce a proper table, "
            + "start your response with 'IMPOSSIBLE:|' followed by a clear explanation of why the ETL cannot be performed.";
    }

    private static string GetTasksPrompt()
    {
        return "You are a data transformation assistant. "
            + "Given arbitrary tabular data (CSV, TSV, markdown table, or other delimited format), "
            + "extract task information and output it EXACTLY in the following tab-separated "
            + "format with a header row:\n\n"
            + "ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID\n\n"
            + "CRITICAL: You MUST output the header row EXACTLY as shown above, character for character. "
            + "NEVER rename columns based on the input headers. "
            + "For example, regardless of whether the input calls it Task, TaskName, or Name, "
            + "the output header MUST be \"TaskName\".\n\n"
            + "Column mapping guide (common input column → output header):\n"
            + "- Id, ID, TaskID, TaskId → ID\n"
            + "- ProjectId, ProjectID, Project, ProjectIdentifier → Project Id\n"
            + "- ProjectName, Project, Name → ProjectName\n"
            + "- TaskName, Task, Name, Description → TaskName\n"
            + "- Hours, Effort, EstimatedEffort, EstimatedEffortHours → EstimatedEffortHours\n"
            + "- Deps, Dependencies, DependsOn, Predecessors → Dependencies\n"
            + "- Progress, Pct, Percent, Completion → Progress\n"
            + "- InternalId, InternalID, IntID, RefID → InternalID\n\n"
            + "Example:\n"
            + "Input:\n"
            + "Id\tProjectName\tTask\tHours\tDep\n"
            + "1\tProj A\tDesign UI\t100\t\n\n"
            + "Output:\n"
            + "ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID\n"
            + "1\tProj A\tDesign UI\t100\t\t\t0\t\n\n"
            + "Rules:\n"
            + "1. Always output the exact header row first.\n"
            + "2. Each subsequent row must have exactly 8 tab-separated columns, in the order specified by the header.\n"
            + "3. Use empty cells (empty tab positions) for missing optional data like Dependencies or InternalID.\n"
            + "4. Omit any data that is not task-related (e.g., project metadata, team info).\n"
            + "5. Preserve the order of tasks as they appear in the input.\n"
            + "6. Do not add any explanation, commentary, or formatting - output only the tab-separated table.\n"
            + "7. If the input data does not contain enough columns or meaningful task data to produce a proper table, "
            + "start your response with 'IMPOSSIBLE:|' followed by a clear explanation of why the ETL cannot be performed.";
    }

    private static string GetTeamPrompt()
    {
        return "You are a data transformation assistant. "
            + "Given arbitrary tabular data (CSV, TSV, markdown table, or other delimited format), "
            + "extract team/developer information and output it EXACTLY in the following tab-separated "
            + "format with a header row:\n\n"
            + "Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours\n\n"
            + "CRITICAL: You MUST output the header row EXACTLY as shown above, character for character. "
            + "NEVER rename columns based on the input headers.\n\n"
            + "Column mapping guide (common input column → output header):\n"
            + "- TeamId, TeamID, Team Id, TeamIdentifier → Team ID\n"
            + "- TeamName, Name, Team → Team Name\n"
            + "- DevId, DeveloperId, DeveloperID, DevID, Id → Developer Id\n"
            + "- DevName, DeveloperName, Name, Developer → Developer Name\n"
            + "- Vacation, VacationDays, VacationDates, Holiday → Developer Vacation Date Intervals\n"
            + "- Hours, WorkHours, DailyHours, DailyWorkHours → Daily Work Hours\n\n"
            + "Example:\n"
            + "Input:\n"
            + "TeamId\tTeam\tDevId\tDevName\tVacation\tHours\n"
            + "1\tAlpha\t1\tAlice\t2026-08-10|2026-08-15\t6\n\n"
            + "Output:\n"
            + "Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours\n"
            + "1\tAlpha\t1\tAlice\t2026-08-10|2026-08-15\t6\n\n"
            + "Rules:\n"
            + "1. Always output the exact header row first.\n"
            + "2. Each subsequent row must have exactly 6 tab-separated columns, in the order specified by the header.\n"
            + "3. Omit any data that is not team/developer-related (e.g., tasks, projects).\n"
            + "4. Preserve the order of developers as they appear in the input.\n"
            + "5. Do not add any explanation, commentary, or formatting - output only the tab-separated table.\n"
            + "6. If the input data does not contain enough columns or meaningful team data to produce a proper table, "
            + "start your response with 'IMPOSSIBLE:|' followed by a clear explanation of why the ETL cannot be performed.";
    }

    private class LlmOutputMessage
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private class LlmResponse
    {
        [JsonPropertyName("output")]
        public List<LlmOutputMessage>? Output { get; set; }

        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }
}
