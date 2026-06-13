using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.UI.Web.ViewModels;

public class GanttGeneratorViewModel
{
    // UI State
    public bool IsSessionInitialized { get; set; }
    public string Email { get; set; } = string.Empty;
    public Guid? SessionIdentifierGuid { get; set; }

    // Configuration
    public DateTime ProjectStartDate { get; set; } = DateTime.Today;
    public ChartModeType ChartMode { get; set; } = ChartModeType.Week;
    public bool PreSortTasks { get; set; }
    public bool SetTeamsToProjectGroups { get; set; }

    // Input Data
    public string ExcelProjectData { get; set; } = string.Empty;
    public string ExcelTaskData { get; set; } = string.Empty;
    public string ExcelTeamData { get; set; } = string.Empty;
    public string SessionContent { get; set; } = string.Empty;

    // Model Lists
    public List<ProjectDto> ProjectDtoList { get; set; } = new();
    public List<TaskDto> TaskDtoList { get; set; } = new();
    public List<DeveloperDto> DeveloperDtoList { get; set; } = new();

    // Output Queryable for Grids
    public IQueryable<ProjectDto>? ProjectDtoListOutput { get; set; }
    public IQueryable<TaskDto>? TaskDtoListOutput { get; set; }
    public IQueryable<DeveloperDto>? TeamDtoListOutput { get; set; }
    public IQueryable<Holiday>? ProjectHolidayList { get; set; }
    public IQueryable<Project>? ProjectListOutput { get; set; }
    public IQueryable<GanttTask>? GanttTaskListOutput { get; set; }
    public IQueryable<GanttTask>? GanttProjectListOutput { get; set; }
    public IQueryable<GanttTask>? DeveloperGanttTaskListOutput { get; set; }
    public IQueryable<DeveloperAvailability>? DeveloperListOutput { get; set; }

    public void Reset()
    {
        IsSessionInitialized = false;
        Email = string.Empty;
        SessionIdentifierGuid = null;
        ExcelProjectData = string.Empty;
        ExcelTaskData = string.Empty;
        ExcelTeamData = string.Empty;
        SessionContent = string.Empty;
        ProjectDtoList = new();
        TaskDtoList = new();
        DeveloperDtoList = new();
        ProjectDtoListOutput = null;
        TaskDtoListOutput = null;
        TeamDtoListOutput = null;
        ProjectHolidayList = null;
        ProjectListOutput = null;
        GanttTaskListOutput = null;
        GanttProjectListOutput = null;
        DeveloperGanttTaskListOutput = null;
        DeveloperListOutput = null;
    }

    public enum ChartModeType
    {
        Week,
        Day
    }
}
