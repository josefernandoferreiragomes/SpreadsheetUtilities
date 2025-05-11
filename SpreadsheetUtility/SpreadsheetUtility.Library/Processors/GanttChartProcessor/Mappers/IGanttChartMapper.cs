using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Domain;

namespace SpreadsheetUtility.Library.Mappers;
public interface IGanttChartMapper
{
    public List<GanttTask> MapGanttTasksFromTaskDtos(List<TaskDto> taskDtos);
    public List<Project> MapProjectsFromProjectDtos(List<ProjectDto> projectDtos);
    public List<Developer> MapDevelopersFromDeveloperDtos(List<DeveloperDto> developerDtos);
    public List<DeveloperAvailability> MapDeveloperAvailabilitiesFromDevelopers(List<Developer> developerList);
}