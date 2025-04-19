using SpreadsheetUtility.Library;

namespace SpreadsheetUtility.Library.Processors.GanttChartProcessor.Builders
{

    public class CalculateGanttChartAllocationOutputBuilder
    {
        private readonly CalculateGanttChartAllocationOutput _output = new();

        public CalculateGanttChartAllocationOutputBuilder WithProjects(List<Project> projects)
        {
            _output.ProjectList = projects;
            return this;
        }

        public CalculateGanttChartAllocationOutputBuilder WithGanttTasks(List<GanttTask> tasks)
        {
            _output.GanttTasks = tasks;
            return this;
        }

        public CalculateGanttChartAllocationOutputBuilder WithGanttProjects(List<GanttTask> projects)
        {
            _output.GanttProjects = projects;
            return this;
        }

        public CalculateGanttChartAllocationOutputBuilder WithDeveloperAvailability(List<DeveloperAvailability> availability)
        {
            _output.DeveloperAvailability = availability;
            return this;
        }
        public CalculateGanttChartAllocationOutputBuilder WithHolidayList(List<Holiday> holidays)
        {
            _output.HolidayList = holidays;
            return this;
        }

        public CalculateGanttChartAllocationOutput Build()
        => _output;

    }
}