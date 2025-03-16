using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public class CalculateGanttChartAllocationInput        
    {
        public List<TaskDto> TaskDtos {get; set;} = new List<TaskDto>();
        public List<DeveloperDto> DeveloperDtos {get; set;} = new List<DeveloperDto>();
        public List<ProjectDto> ProjectDtos {get; set;} = new List<ProjectDto>();
        public bool PreSortTasks { get; set; } = false;
    }
}
