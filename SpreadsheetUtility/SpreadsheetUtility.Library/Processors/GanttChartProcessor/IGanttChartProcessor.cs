using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public interface IGanttChartProcessor
    {       
        List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);        
        CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input);
    }
}
