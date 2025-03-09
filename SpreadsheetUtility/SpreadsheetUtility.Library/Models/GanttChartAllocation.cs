using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public class GanttChartAllocation
    {
        public List<Project> ProjectList { get; set; } = new List<Project>();
        public List<GanttTask> GanttTasks { get; set; } = new List<GanttTask>();
        public List<DeveloperAvailability> DeveloperAvailability { get; set; } = new List<DeveloperAvailability>();
    }
}
