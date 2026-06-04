using SpreadsheetUtility.Application.DTOs;

namespace SpreadsheetUtility.Application.Services;

public class PasteParserService : IPasteParserService
{
    public List<ProjectDto> ParseProjects(string data)
    {
        var list = new List<ProjectDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length == 4)
            {
                list.Add(new ProjectDto
                {
                    ProjectID = columns[0].Trim(),
                    ProjectName = columns[1].Trim(),
                    ProjectGroup = columns[2].Trim(),
                    TeamId = columns[3].Trim()
                });
            }
        }
        return list;
    }

    public List<TaskDto> ParseTasks(string data)
    {
        var list = new List<TaskDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length >= 8)
            {
                list.Add(new TaskDto
                {
                    Id = columns[0].Trim(),
                    ProjectID = columns[1].Trim(),
                    ProjectName = columns[2].Trim(),
                    TaskName = columns[3].Trim(),
                    EffortHours = double.TryParse(columns[4].Trim(), out var effort) ? effort : 0,
                    Dependencies = columns[5].Trim(),
                    Progress = columns[6].Trim(),
                    InternalID = columns[7].Trim()
                });
            }
        }
        return list;
    }

    public List<TaskDto> ParseTasksWithActual(string data)
    {
        var list = new List<TaskDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length == 10)
            {
                list.Add(new TaskDto
                {
                    Id = columns[0].Trim(),
                    ProjectID = columns[1].Trim(),
                    ProjectName = columns[2].Trim(),
                    TaskName = columns[3].Trim(),
                    EffortHours = double.TryParse(columns[4].Trim(), out var effort) ? effort : 0,
                    Dependencies = columns[5].Trim(),
                    Progress = columns[6].Trim(),
                    InternalID = columns[7].Trim(),
                    ActualStart = columns[8].Trim(),
                    ActualEnd = columns[9].Trim()
                });
            }
        }
        return list;
    }

    public List<DeveloperDto> ParseTeam(string data)
    {
        var list = new List<DeveloperDto>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split('\t');
            if (columns.Length == 6)
            {
                list.Add(new DeveloperDto
                {
                    TeamId = columns[0].Trim(),
                    Team = columns[1].Trim(),
                    DeveloperId = columns[2].Trim(),
                    Name = columns[3].Trim(),
                    VacationPeriods = columns[4].Trim(),
                    DailyWorkHours = int.TryParse(columns[5].Trim(), out var hours) ? hours : 0
                });
            }
        }
        return list;
    }
}
