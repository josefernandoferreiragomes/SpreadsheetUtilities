using MediatR;
using SpreadsheetUtility.Application.DTOs;

namespace SpreadsheetUtility.Application.UseCases.ParseExcelData;

public class ParseExcelDataCommandHandler : IRequestHandler<ParseExcelDataCommand, CalculateGanttChartAllocationInput>
{
    public Task<CalculateGanttChartAllocationInput> Handle(ParseExcelDataCommand request, CancellationToken cancellationToken)
    {
        var input = new CalculateGanttChartAllocationInput();

        if (!string.IsNullOrEmpty(request.ProjectsData))
        {
            input.ProjectDtos = ParseProjects(request.ProjectsData);
        }

        if (!string.IsNullOrEmpty(request.TasksData))
        {
            input.TaskDtos = ParseTasks(request.TasksData);
        }

        if (!string.IsNullOrEmpty(request.TeamData))
        {
            input.DeveloperDtos = ParseTeam(request.TeamData);
        }

        return Task.FromResult(input);
    }

    private static List<ProjectDto> ParseProjects(string data)
    {
        var projectDtoList = new List<ProjectDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length == 4)
            {
                projectDtoList.Add(new ProjectDto()
                {
                    ProjectID = columns[0].Trim(),
                    ProjectName = columns[1].Trim(),
                    ProjectGroup = columns[2].Trim(),
                    TeamId = columns[3].Trim()
                });
            }
        }
        return projectDtoList;
    }

    private static List<TaskDto> ParseTasks(string data)
    {
        var taskDtoList = new List<TaskDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length >= 8)
            {
                taskDtoList.Add(new TaskDto()
                {
                    Id = columns[0].Trim(),
                    ProjectID = columns[1].Trim(),
                    ProjectName = columns[2].Trim(),
                    TaskName = columns[3].Trim(),
                    EffortHours = double.TryParse(columns[4].Trim(), out var effort) ? effort : 0,
                    Dependencies = columns[5].Trim(),
                    Progress = columns[6].Trim(),
                    InternalID = columns[7].Trim(),
                    ActualStart = columns.Length > 8 ? columns[8].Trim() : null,
                    ActualEnd = columns.Length > 9 ? columns[9].Trim() : null
                });
            }
        }
        return taskDtoList;
    }

    private static List<DeveloperDto> ParseTeam(string data)
    {
        var developerDtoList = new List<DeveloperDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length == 6)
            {
                developerDtoList.Add(new DeveloperDto()
                {
                    TeamId = columns[0].Trim(),
                    Team = columns[1].Trim(),
                    DeveloperId = columns[2].Trim(),
                    Name = columns[3].Trim(),
                    VacationPeriods = columns[4].Trim(),
                    DailyWorkHours = int.TryParse(columns[5].Trim(), out var hours) ? hours : 0,
                });
            }
        }
        return developerDtoList;
    }
}
