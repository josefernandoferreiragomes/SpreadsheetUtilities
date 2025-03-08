using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public interface IGanttChartProcessor
    {
        string ProcessExcelDataTasksFromFile(string taskFilePath, string teamFilePath);
        string ProcessExcelDataProjectsFromFile(string taskFilePath, string teamFilePath);
        List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);
        List<DeveloperAvailability> LoadDeveloperAvailabilityFromDtos(List<DeveloperDto> taskDtos);
        List<GanttTask> AssignProjectsFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos);
        GanttChartAllocation CalculateGanttChartAllocationFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos);
    }
}
