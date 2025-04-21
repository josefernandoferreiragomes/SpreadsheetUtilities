using SpreadsheetUtility.Library.Providers;
using System.Globalization;

namespace SpreadsheetUtility.Library.Processors.GanttChartProcessor.Mappers
{
    public interface IGanttChartMapper
    {
        public List<GanttTask> MapGanttTasksFromTaskDtos(List<TaskDto> taskDtos);
        public List<Project> MapProjectsFromProjectDtos(List<ProjectDto> projectDtos);
        public List<Developer> MapDevelopersFromDeveloperDtos(List<DeveloperDto> developerDtos);
        public List<DeveloperAvailability> MapDeveloperAvailabilitiesFromDevelopers(List<Developer> developerList);
    }

    public class GanttChartMapper(IDateTimeProvider _dateTimeProvider) : IGanttChartMapper
    {
        public List<GanttTask> MapGanttTasksFromTaskDtos(List<TaskDto> taskDtos)
            => taskDtos.Select(dto => new GanttTask
            {
                Id = dto.Id ?? "",
                Name = $"{dto.ProjectName} : {dto.TaskName}",
                EstimatedEffortHours = dto.EstimatedEffortHours,
                Dependencies = dto.Dependencies ?? "",
                Progress = int.TryParse(dto.Progress, out var p) ? p : 0,
                ProjectID = dto.ProjectID ?? "",
                ProjectName = dto.ProjectName ?? "",
                TaskName = dto.TaskName ?? "",
                InternalID = dto.InternalID ?? "",
                ActualStart = dto.ActualStart ?? "",
                ActualEnd = dto.ActualEnd ?? "",
            }).ToList();

        public List<Project> MapProjectsFromProjectDtos(List<ProjectDto> projectDtos)
            => projectDtos.Select(dto => new Project
            {
                ProjectID = dto.ProjectID,
                ProjectName = dto.ProjectName,
                ProjectGroup = dto.ProjectGroup
            }).ToList();


        public List<Developer> MapDevelopersFromDeveloperDtos(List<DeveloperDto> developerDtos)
            => developerDtos.Select(dto => new Developer(_dateTimeProvider)
            {
                Name = $"{dto.Team} : {dto.Name}",
                DeveloperId = dto.DeveloperId,
                DailyWorkHours = dto.DailyWorkHours,
                VacationPeriods = dto.VacationPeriods.Split('|')
                    .Select(ParseDateRange)
                    .Where(d => d != null)
                    .ToList()
            }).ToList();

        public List<DeveloperAvailability> MapDeveloperAvailabilitiesFromDevelopers(List<Developer> developerList)
        {
            return developerList.Select(d => new DeveloperAvailability
            {
                Name = d.Name,
                DeveloperId = d.DeveloperId,
                DailyWorkHours = d.DailyWorkHours,
                VacationPeriods = d.VacationPeriods,
                Tasks = d.Tasks,
                AllocatedHours = d.AllocatedHours,
                SlackHours = d.SlackHours,
                TotalHours = d.TotalHours,
                VacationPeriodsSerialized = d.VacationPeriodsSerialized,
                NextAvailableDateForTasks = d.NextAvailableDateForTasks
            }).ToList();
        }

        private (DateTime Start, DateTime End)? ParseDateRange(string range)
        {
            var dates = range.Split(';');
            if (dates.Length == 2 &&
                DateTime.TryParseExact(dates[0].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) &&
                DateTime.TryParseExact(dates[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
            {
                return (start, end);
            }
            return null;
        }

    }
}
