namespace SpreadsheetUtility.Application.DTOs;

public class CalculateGanttChartAllocationInput
{
    public List<TaskDto> TaskDtos { get; set; } = new List<TaskDto>();
    public List<DeveloperDto> DeveloperDtos { get; set; } = new List<DeveloperDto>();
    public List<ProjectDto> ProjectDtos { get; set; } = new List<ProjectDto>();
    public bool PreSortTasks { get; set; }
    public DateTime ProjectStartDate { get; set; }
    public bool SetTeamsToProjectGroups { get; set; }
}
