using SpreadsheetUtility.Application.DTOs;

namespace SpreadsheetUtility.Application.Services;

public interface IPasteParserService
{
    List<ProjectDto> ParseProjects(string data);
    List<TaskDto> ParseTasks(string data);
    List<TaskDto> ParseTasksWithActual(string data);
    List<DeveloperDto> ParseTeam(string data);
}
