using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Mappers;

public interface IGanttChartMapper
{
    List<GanttTask> MapGanttTasksFromTaskDtos(List<TaskDto> taskDtos);
    List<Project> MapProjectsFromProjectDtos(List<ProjectDto> projectDtos);
    List<Developer> MapDevelopersFromDeveloperDtos(List<DeveloperDto> developerDtos);
    List<DeveloperAvailability> MapDeveloperAvailabilitiesFromDevelopers(List<Developer> developerList);
}
