using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public interface IGanttService
    {        
        List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);       
        GanttChartAllocation CalculateGanttChartAllocationFromDtos(GanttChartAllocationInput input);
    }
}
