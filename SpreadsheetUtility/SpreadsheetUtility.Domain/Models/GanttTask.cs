using System.Text.Json.Serialization;

namespace SpreadsheetUtility.Domain.Models;

public class GanttTask
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string TaskName { get; set; }

    [JsonPropertyName("start")]
    public string? Start { get; set; }

    [JsonPropertyName("end")]
    public string? End { get; set; }

    [JsonPropertyName("progress")]
    public int Progress { get; set; } = 0;

    [JsonPropertyName("dependencies")]
    public string Dependencies { get; set; } = "";

    [JsonPropertyName("custom_class")]
    public string? CustomClass { get; set; }

    [JsonPropertyName("resource")]
    public string? Resource { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? InternalID { get; set; }

    public double EffortHours { get; set; }
    public string? ProjectName { get; set; }
    public string? TaskExtendedDescription { get; set; }
    public string? ProjectID { get; set; }
    public string? ProjectDependency { get; set; }
    public string? AssignedDeveloper { get; set; }
    public string? AssignedDeveloperId { get; set; }
    internal bool DependencyUpdated { get; set; }
    public string? TaskEndWeekDescriptionDescription { get; set; }

    public string? ActualStart { get; set; }
    public string? ActualEnd { get; set; }

    public int ActualProgress { get; set; }

    public int? IntervalDays { get; set; }
    public int? WorkDays { get; set; }
    public int? VacationDays { get; set; }
    public int? NonWorkingDays { get; set; }
    public int? DailyWorkHours { get; set; }
}
