using SpreadsheetUtility.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Services
{
    
    public class GanttService : IGanttService
    {
        IGanttChartProcessor _ganttChartProcessor;
        public GanttService(IGanttChartProcessor ganttChartProcessor)
        {
            _ganttChartProcessor = ganttChartProcessor;
        }
        public string ProcessExcelDataTasksFromFile(string taskFilePath, string teamFilePath)
        {
            return _ganttChartProcessor.ProcessExcelDataTasksFromFile(taskFilePath, teamFilePath);
        }
        public string ProcessExcelDataProjectsFromFile(string taskFilePath, string teamFilePath)
        {
            return _ganttChartProcessor.ProcessExcelDataProjectsFromFile(taskFilePath, teamFilePath);
        }
        public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
        {
            return _ganttChartProcessor.LoadTasksFromDtos(taskDtos);
        }
        public List<DeveloperAvailability> LoadDeveloperAvailabilityFromDtos(List<DeveloperDto> developerDtos)
        {
            return _ganttChartProcessor.LoadDeveloperAvailabilityFromDtos(developerDtos);
        }
        public List<GanttTask> AssignProjectsFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos, bool preSortTasks = false)
        {
            return _ganttChartProcessor.AssignProjectsFromDtos(taskDtos, developerDtos, preSortTasks);
        }
        public GanttChartAllocation CalculateGanttChartAllocationFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos, bool preSortTasks = true)
        {
            return _ganttChartProcessor.CalculateGanttChartAllocationFromDtos(taskDtos, developerDtos, preSortTasks);
        }
    }
}
