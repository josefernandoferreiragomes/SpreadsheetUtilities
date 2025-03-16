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
        public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
        {
            return _ganttChartProcessor.LoadTasksFromDtos(taskDtos);
        }        
        public GanttChartAllocation CalculateGanttChartAllocationFromDtos(GanttChartAllocationInput input)
        {
            return _ganttChartProcessor.CalculateGanttChartAllocationFromDtos(input);
        }
    }
}
