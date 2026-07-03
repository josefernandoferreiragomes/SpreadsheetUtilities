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

        var responseBody = await response.Content.ReadFromJsonAsync<LlmResponse>(_jsonOptions, cancellationToken);
        var llmOutput = responseBody?.Output ?? responseBody?.Response ?? string.Empty;

        if (string.IsNullOrWhiteSpace(llmOutput))
        {
            return new LlmTransformationResult(false, string.Empty, "The LLM returned an empty response.");
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
            + "extract project information and output it in the following tab-separated format "
            + "with a header row:\n\n"
            + "ProjectID\tProject Name\tProject Group Id\tTeam Id\n\n"
            + "Rules:\n"
            + "1. Always output the header row first.\n"
            + "2. Each subsequent row must have exactly 4 tab-separated columns.\n"
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
            + "extract task information and output it in the following tab-separated format "
            + "with a header row:\n\n"
            + "ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID\n\n"
            + "Rules:\n"
            + "1. Always output the header row first.\n"
            + "2. Each subsequent row must have exactly 8 tab-separated columns.\n"
            + "3. Omit any data that is not task-related (e.g., project metadata, team info).\n"
            + "4. Preserve the order of tasks as they appear in the input.\n"
            + "5. Do not add any explanation, commentary, or formatting - output only the tab-separated table.\n"
            + "6. If the input data does not contain enough columns or meaningful task data to produce a proper table, "
            + "start your response with 'IMPOSSIBLE:|' followed by a clear explanation of why the ETL cannot be performed.";
    }

    private static string GetTeamPrompt()
    {
        return "You are a data transformation assistant. "
            + "Given arbitrary tabular data (CSV, TSV, markdown table, or other delimited format), "
            + "extract team/developer information and output it in the following tab-separated format "
            + "with a header row:\n\n"
            + "Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours\n\n"
            + "Rules:\n"
            + "1. Always output the header row first.\n"
            + "2. Each subsequent row must have exactly 6 tab-separated columns.\n"
            + "3. Omit any data that is not team/developer-related (e.g., tasks, projects).\n"
            + "4. Preserve the order of developers as they appear in the input.\n"
            + "5. Do not add any explanation, commentary, or formatting - output only the tab-separated table.\n"
            + "6. If the input data does not contain enough columns or meaningful team data to produce a proper table, "
            + "start your response with 'IMPOSSIBLE:|' followed by a clear explanation of why the ETL cannot be performed.";
    }

    private class LlmResponse
    {
        [JsonPropertyName("output")]
        public string? Output { get; set; }

        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }
}
