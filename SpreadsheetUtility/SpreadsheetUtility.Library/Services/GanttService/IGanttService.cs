using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.Services;

public interface IGanttService
{        
    List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);       
    CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input);
}
