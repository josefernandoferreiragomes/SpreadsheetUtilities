using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [JsonProperty("customClass")]
        public string? CustomClass { get; set; } // New property for styling

        [JsonProperty("resource")]
        public string? Resource { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double EstimatedEffortHours { get; set; }
        public string? ProjectName { get; set; }
        public string? TaskName { get; set; }
        public string? ProjectID { get; set; }
        public string? ProjectDependency { get; set; }
        public string? AssignedDeveloper { get; set; }
    }
}
