using ClosedXML.Excel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpreadsheetUtility.Library
{ 


    public interface IGanttChartService
    {
        string ProcessExcelDataTasksFromFile(string taskFilePath, string teamFilePath);
        string ProcessExcelDataProjectsFromFile(string taskFilePath, string teamFilePath);
        List<GanttTask> ProcessDataTasksFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos);
        List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);
        List<DeveloperAvailability> LoadTeamDataFromDtos(List<DeveloperDto> taskDtos);
        List<GanttTask> AssignProjectsFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos);
        GanttChartAllocation CalculateGanttChartAllocationFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos);
    }

    public class GanttChartAllocation
    {
        public List<GanttTask> GanttTasks;
        public List<DeveloperAvailability> DeveloperAvailability;
    }

    public class GanttChartService: IGanttChartService
    {
        private List<GanttTask> _ganttTasks;
        private List<GanttTask> _ganttProjects;

        private List<DeveloperAvailability> _developerAvailability;

        public GanttChartService()
        {
            _ganttTasks = new List<GanttTask>();
            _ganttProjects = new List<GanttTask>();
            _developerAvailability = new List<DeveloperAvailability>();
        }        

        #region dto processing
        public GanttChartAllocation CalculateGanttChartAllocationFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos)
        {
            _ganttTasks = LoadTasksFromDtos(taskDtos);
            _developerAvailability = LoadTeamDataFromDtos(developerDtos);
            AssignTasks();
            CalculateDeveloperSlack();
            return new GanttChartAllocation
            {
                GanttTasks = _ganttTasks,
                DeveloperAvailability = _developerAvailability
            };
        }

        public List<GanttTask> ProcessDataTasksFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos)
        {
            _ganttTasks = LoadTasksFromDtos(taskDtos);
            _developerAvailability = LoadTeamDataFromDtos(developerDtos);
            AssignTasks();
            CalculateDeveloperSlack();
            Console.WriteLine(JsonConvert.SerializeObject(_ganttTasks,
               new JsonSerializerSettings
               {
                   ContractResolver = new CamelCasePropertyNamesContractResolver(),
                   Formatting = Formatting.Indented
               })
            );

            return _ganttTasks;
        }
        
        public List<GanttTask> AssignProjectsFromDtos(List<TaskDto> taskDtos, List<DeveloperDto> developerDtos)
        {
            ProcessDataTasksFromDtos(taskDtos, developerDtos);

            //ID	ProjectName	Dependencies
            double totalEstimatedEffortHours = _ganttTasks.Sum(t => t.EstimatedEffortHours);

            _ganttProjects = _ganttTasks.GroupBy(t => t.ProjectName)
                .Select(g => new GanttTask
                {
                    Id = g.First().ProjectID,
                    Name = $"{g.Key} ({g.Sum(t => t.EstimatedEffortHours)} hours)",
                    EstimatedEffortHours = g.Sum(t => t.EstimatedEffortHours),
                    Dependencies = g.Select(t => t.ProjectDependency).Where(d => !string.IsNullOrEmpty(d)).FirstOrDefault() ?? string.Empty,
                    Progress = (int)(g.Sum(t => (t.Progress * (t.EstimatedEffortHours / totalEstimatedEffortHours)))), // Summing the progress of each related task
                    Start = g.Min(t => t.Start)?.ToString() ?? string.Empty,
                    End = g.Max(t => t.End)?.ToString() ?? string.Empty,
                    StartDate = DateTime.TryParse(g.Min(t => t.Start),CultureInfo.CurrentCulture,out var startDate) ? startDate : DateTime.MinValue,
                    EndDate = DateTime.TryParse(g.Max(t => t.End), CultureInfo.CurrentCulture, out var endDate) ? endDate : DateTime.MinValue
                }).ToList();



            Console.WriteLine(JsonConvert.SerializeObject(_ganttProjects,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            return _ganttProjects;
        }

        public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
        {
            return taskDtos.Select(dto => new GanttTask
            {
                Id = dto.Id,
                Name = $"{dto.ProjectName} : {dto.TaskName}",
                EstimatedEffortHours = dto.EstimatedEffortHours,
                Dependencies = dto.Dependencies,
                Progress = int.TryParse(dto.Progress, out var p) ? p : 0,
                ProjectName = dto.ProjectName,
                TaskName = dto.TaskName
            }).ToList();
        }
        public List<DeveloperAvailability> LoadTeamDataFromDtos(List<DeveloperDto> taskDtos)
        {
            return taskDtos.Select(dto => new DeveloperAvailability
            {
                Name = $"{dto.Team} : {dto.Name}",
                DailyWorkHours = dto.DailyWorkHours,
                VacationPeriods = dto.VacationPeriods.Split('|')
                        .Select(ParseDateRange)
                        .Where(d => d != null)
                        .ToList()
            }).ToList();
        }

        #endregion

        #region file processing
        public string ProcessExcelDataTasksFromFile(string taskFilePath, string teamFilePath)
        {
            ProcessDataTasksFromFile(taskFilePath, teamFilePath);

            Console.WriteLine(JsonConvert.SerializeObject(_ganttTasks,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            return JsonConvert.SerializeObject(_ganttTasks);
        }
        
        public string ProcessExcelDataProjectsFromFile(string taskFilePath, string teamFilePath)
        {
            ProcessDataTasksFromFile(taskFilePath, teamFilePath);

            //ID	ProjectName	Dependencies
            double totalEstimatedEffortHours = _ganttTasks.Sum(t => t.EstimatedEffortHours);

            _ganttProjects = _ganttTasks.GroupBy(t => t.ProjectName)
                .Select(g => new GanttTask
                {
                    Id = g.First().ProjectID,
                    Name = $"{g.Key} ({g.Sum(t => t.EstimatedEffortHours)} hours)",
                    EstimatedEffortHours = g.Sum(t => t.EstimatedEffortHours),
                    Dependencies = g.Select(t => t.ProjectDependency).Where(d => !string.IsNullOrEmpty(d)).FirstOrDefault() ?? string.Empty,
                    Progress = (int)(g.Sum(t => (t.Progress * (t.EstimatedEffortHours / totalEstimatedEffortHours)))), // Summing the progress of each related task
                    Start = g.Min(t => t.Start)?.ToString() ?? string.Empty,
                    End = g.Max(t => t.End)?.ToString() ?? string.Empty,
                    StartDate = DateTime.TryParse(g.Min(t => t.Start), CultureInfo.CurrentCulture, out var startDate) ? startDate : DateTime.MinValue,
                    EndDate = DateTime.TryParse(g.Max(t => t.End), CultureInfo.CurrentCulture, out var endDate) ? endDate : DateTime.MinValue
                }).ToList();

            

            Console.WriteLine(JsonConvert.SerializeObject(_ganttProjects,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            return JsonConvert.SerializeObject(_ganttProjects);
        }

        private void ProcessDataTasksFromFile(string taskFilePath, string teamFilePath)
        {
            _ganttTasks = LoadTasksFromFile(taskFilePath);
            _developerAvailability = LoadDevelopersFromFile(teamFilePath);
            AssignTasks();
        }

        private List<TaskDto> LoadTaskDtosFromFile(string filePath)
        {
            //A ID	B ProjectID	C ProjectName	D TaskName	E EstimatedEffortHours	F TaskDependencies	G ProjectDependency	H Progress

            var taskDtos = new List<TaskDto>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RangeUsed().RowsUsed().Skip(1)) // Skip header
                {
                    taskDtos.Add(new TaskDto
                    {
                        Id = row.Cell("A").GetString(),
                        ProjectID = row.Cell("B").GetString(),
                        ProjectName = row.Cell("C").GetString(),
                        TaskName = row.Cell("D").GetString(),
                        EstimatedEffortHours = row.Cell("E").GetDouble(),
                        Dependencies = row.Cell("F").GetString(),
                        ProjectDependency = row.Cell("G").GetString(),
                        Progress = row.Cell("H").GetString()
                    });
                }
            }
            return taskDtos;
        }

        private List<GanttTask> LoadTasksFromFile(string filePath)
        {
            var taskDtos = LoadTaskDtosFromFile(filePath);
            List<GanttTask> tasks = LoadTasksFromDtos(taskDtos);

            return tasks;
        }

        private List<DeveloperAvailability> LoadDevelopersFromFile(string filePath)
        {
            var developers = new List<DeveloperAvailability>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RangeUsed().RowsUsed().Skip(1)) // Skip header
                {
                    var name = row.Cell(2).GetString();
                    var dailyHours = row.Cell(4).GetDouble();
                    var vacationDates = row.Cell(3).GetString().Split('|')
                        .Select(ParseDateRange)
                        .Where(d => d != null)
                        .ToList();

                    developers.Add(new DeveloperAvailability
                    {
                        Name = name,
                        DailyWorkHours = dailyHours,
                        VacationPeriods = vacationDates
                    });
                }
            }
            return developers;
        }
        #endregion

        #region auxiliary methods

        private void CalculateDeveloperSlack()
        {
            DateTime minDate = _ganttTasks.Min(t => t.StartDate);
            DateTime maxDate = _ganttTasks.Max(t => t.EndDate);
            
            //calculate the sum of hours of non allocation for each developer
            foreach (var developer in _developerAvailability)
            {
                var calculatedIntervalDays = CalculateIntervalDays(minDate, maxDate, developer.VacationPeriods);
                developer.AllocatedHours = developer.Tasks.Sum(t => t.EstimatedEffortHours);
                var slackHours = (calculatedIntervalDays * developer.DailyWorkHours) - developer.AllocatedHours;
                developer.SlackHours = slackHours >= 0 ? slackHours : 0;
            }

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

        private void AssignTasks()
        {

            DateTime startDate = DateTime.Today;
            while (IsWeekend(startDate))
            {
                startDate = startDate.AddDays(1);
            }
            foreach (var task in _ganttTasks)
            {
                var assignedDeveloper = _developerAvailability.OrderBy(d => d.NextAvailableDate(startDate)).FirstOrDefault();
                if (assignedDeveloper == null) continue;
                DateTime taskStart = assignedDeveloper.NextAvailableDate(startDate);
                DateTime dependencyEndDate;
                taskStart = DateTime.TryParse(_ganttTasks.Find(t=> t.Id == task.Dependencies)?.End ?? "", out dependencyEndDate) ? dependencyEndDate : taskStart;
                double requiredDays = Math.Ceiling(task.EstimatedEffortHours / assignedDeveloper.DailyWorkHours);
                DateTime taskEnd = CalculateEndDate(taskStart, requiredDays, assignedDeveloper.VacationPeriods);
                task.Start = taskStart.ToString("yyyy-MM-dd");
                task.End = taskEnd.ToString("yyyy-MM-dd");
                task.StartDate = taskStart;
                task.EndDate = taskEnd;
                task.AssignedDeveloper = assignedDeveloper.Name;
                task.Name = $"{task.Name} ({assignedDeveloper.Name})";
                task.CustomClass = task.Name.Contains("task") ? "gantt-task-blue" : "gantt-task-green";
                assignedDeveloper.Tasks.Add(task);
                assignedDeveloper.SetNextAvailableDate(taskEnd.AddDays(1));
            }

        }
        
        private DateTime CalculateEndDate(DateTime start, double workDays, List<(DateTime Start, DateTime End)?> vacations)
        {
            DateTime end = start;
            while (workDays > 0)
            {
                if (!IsVacationDay(end, vacations) && !IsWeekend(end)) workDays--;
                end = end.AddDays(1);
            }
            return end.AddDays(-1);
        }

        private int CalculateIntervalDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?> vacations)
        {
            int days = 0;
            var startDate = start;
            while (startDate < end)
            {
                if (!IsVacationDay(startDate, vacations) && !IsWeekend(startDate))
                {
                   days++;
                }
                startDate = startDate.AddDays(1);
            }
            return days;
        }

        private bool IsVacationDay(DateTime date, List<(DateTime Start, DateTime End)?> vacations)
        {
            return vacations.Any(v => v.HasValue && date >= v.Value.Start && date <= v.Value.End);
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        #endregion
    }

}