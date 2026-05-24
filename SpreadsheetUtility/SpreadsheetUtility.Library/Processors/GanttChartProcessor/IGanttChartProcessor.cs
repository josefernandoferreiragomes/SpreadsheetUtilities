using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Library.Processors;

public interface IGanttChartProcessor
{       
    List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);        
    CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input);
}
