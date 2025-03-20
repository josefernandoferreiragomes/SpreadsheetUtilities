using SpreadsheetUtility.Library;

namespace SpreadsheetUtility.UI.Web.Helpers
{
    public static class GanttMapperHelper
    {
        public static List<ProjectDto> ConvertToGanttProjects(string excelProjectData)
        {
            var projectDtoList = new List<ProjectDto>();
            var lines = excelProjectData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            projectDtoList = new List<ProjectDto>();

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var columns = line.Split('\t');
                if (columns.Length == 3)
                {
                    projectDtoList.Add(new ProjectDto()
                    {
                        ProjectID = columns[0].Trim(),
                        ProjectName = columns[1].Trim(),
                        ProjectGroup = columns[2].Trim()
                    });
                }
            }
            return projectDtoList;
        }

        public static List<TaskDto> ConvertToGanttTasksWithActual(string excelTaskData)
        {
            var taskDtoList = new List<TaskDto>();
            var lines = excelTaskData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            taskDtoList = new List<TaskDto>();

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var columns = line.Split('\t');
                if (columns.Length == 10)
                {
                    taskDtoList.Add(new TaskDto()
                    {
                        Id = columns[0].Trim(),
                        ProjectID = columns[1].Trim(),
                        ProjectName = columns[2].Trim(),
                        TaskName = columns[3].Trim(),
                        EstimatedEffortHours = double.TryParse(columns[4].Trim(), out var effort) ? effort : 0,
                        Dependencies = columns[5].Trim(),
                        Progress = columns[6].Trim(),
                        InternalID = columns[7].Trim(),
                        ActualStart = columns[8].Trim(),
                        ActualEnd = columns[9].Trim()
                    });
                }
            }
            return taskDtoList;
        }

        public static List<TaskDto> ConvertToGanttTasks(string excelTaskData)
        {
            var taskDtoList = new List<TaskDto>();
            var lines = excelTaskData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            taskDtoList = new List<TaskDto>();

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var columns = line.Split('\t');
                if (columns.Length == 8)
                {
                    taskDtoList.Add(new TaskDto()
                    {
                        Id = columns[0].Trim(),
                        ProjectID = columns[1].Trim(),
                        ProjectName = columns[2].Trim(),
                        TaskName = columns[3].Trim(),
                        EstimatedEffortHours = double.TryParse(columns[4].Trim(), out var effort) ? effort : 0,
                        Dependencies = columns[5].Trim(),
                        Progress = columns[6].Trim(),
                        InternalID = columns[7].Trim()

                    });
                }
            }
            return taskDtoList;
        }

        public static List<DeveloperDto> ConvertToTeamData(string excelTeamData)
        {
            var developerDtoList = new List<DeveloperDto>();
            var lines = excelTeamData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            developerDtoList = new List<DeveloperDto>();

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var columns = line.Split('\t');
                if (columns.Length == 4)
                {
                    developerDtoList.Add(new DeveloperDto()
                    {
                        Team = columns[0].Trim(),
                        Name = columns[1].Trim(),
                        VacationPeriods = columns[2].Trim(),
                        DailyWorkHours = int.TryParse(columns[3].Trim(), out var hours) ? hours : 0,

                    });
                }
            }
            return developerDtoList;
        }

    }
}
