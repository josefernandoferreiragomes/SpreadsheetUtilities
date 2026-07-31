using System.Text.Json.Serialization;

namespace SpreadsheetUtility.Infrastructure.Models;

public record CombinedSessionData
{
    [JsonPropertyName("projectData")]
    public string? ProjectData { get; init; }

    [JsonPropertyName("taskData")]
    public string? TaskData { get; init; }

    [JsonPropertyName("teamData")]
    public string? TeamData { get; init; }
}
