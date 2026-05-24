using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Library.Services;

public interface IGanttChartDataManager
{        
    List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);       
    CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input);
}
