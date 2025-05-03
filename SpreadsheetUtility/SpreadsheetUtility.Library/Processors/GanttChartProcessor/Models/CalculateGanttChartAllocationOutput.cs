namespace SpreadsheetUtility.Library.Models;
public class CalculateGanttChartAllocationOutput
{
    public List<Project> ProjectList { get; set; } = new List<Project>();
    public List<GanttTask> GanttTasks { get; set; } = new List<GanttTask>();
    public List<GanttTask> GanttProjects { get; set; } = new List<GanttTask>();
    public List<GanttTask> DeveloperGanttTaskList { get; set; } = new List<GanttTask>();
    public List<DeveloperAvailability> DeveloperAvailability { get; set; } = new List<DeveloperAvailability>();
    public List<Holiday> HolidayList { get; set; } = new List<Holiday>();
}

