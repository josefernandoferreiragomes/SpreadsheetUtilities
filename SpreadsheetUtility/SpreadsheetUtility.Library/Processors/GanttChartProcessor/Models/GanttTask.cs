using Newtonsoft.Json;

namespace SpreadsheetUtility.Library
{
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

        public double EstimatedEffortHours { get; set; }
        public string? ProjectName { get; set; }
        public string? TaskName { get; set; }
        public string? ProjectID { get; set; }
        public string? ProjectDependency { get; set; }
        public string? AssignedDeveloper { get; set; }
        public string? AssignedDeveloperId { get; set; }
        internal bool DependencyUpdated { get; set; } = false;
        public string? TaskEndWeek { get; set; }

        public string? ActualStart { get; set; }
        public string? ActualEnd { get; set; }
        public int ActualProgress { get; set; }   // 0-100%
    }
}
