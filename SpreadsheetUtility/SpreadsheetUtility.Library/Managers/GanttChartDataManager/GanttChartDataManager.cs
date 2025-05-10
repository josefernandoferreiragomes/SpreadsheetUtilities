using SpreadsheetUtility.Library;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Processors;

namespace SpreadsheetUtility.Library.Services;

public class GanttChartDataManager : IGanttChartDataManager
{
    IGanttChartProcessor _ganttChartProcessor;
    public GanttChartDataManager(IGanttChartProcessor ganttChartProcessor)
    {
        _ganttChartProcessor = ganttChartProcessor;
    }
    public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
    {
        return _ganttChartProcessor.LoadTasksFromDtos(taskDtos);
    }        
    public CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input)
    {
        return _ganttChartProcessor.CalculateGanttChartAllocation(input);
    }
}
