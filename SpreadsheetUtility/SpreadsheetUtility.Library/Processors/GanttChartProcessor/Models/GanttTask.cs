using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SpreadsheetUtility.Library.Models;

public class GanttTask
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("name")]    
    public required string Name { get; set; }

    [JsonProperty("start")]
    public string? Start { get; set; } // Format: "YYYY-MM-DD"

    [JsonProperty("end")]
    public string? End { get; set; }   // Format: "YYYY-MM-DD"

    [JsonProperty("progress")]
    public int Progress { get; set; } = 0;

    [JsonProperty("dependencies")]
    public string Dependencies { get; set; } = "";

    [JsonProperty("custom_class")]
    public string? CustomClass { get; set; } // New property for styling

    [JsonProperty("resource")]
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
    /// <summary>
    /// To be defined... The task is updated by the user
    /// </summary>
    internal bool DependencyUpdated { get; set; } = false;
    public string? TaskEndWeekDescription { get; set; }

    public string? ActualStart { get; set; }
    public string? ActualEnd { get; set; }

    /// <summary>
    /// To be defined... The actual progress of the task, represented as a consumed time, in hour
    /// </summary>
    public int ActualProgress { get; set; }   // 0-100%

    public int? IntervalDays { get; set; }
    public int? WorkDays { get; set; }
    public int? VacationDays { get; set; }
    public int? NonWorkingDays { get; set; }
    public int? DailyWorkHours { get; set; }
}

