using ClosedXML.Excel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;

namespace SpreadsheetUtility.Library
{ 


    public interface IGanttChartService
    {
        string ProcessExcelDataTasks(string taskFilePath, string teamFilePath);
        string ProcessExcelDataProjects(string taskFilePath, string teamFilePath);
        public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos);

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

        public string ProcessExcelDataTasks(string taskFilePath, string teamFilePath)
        {
            ProcessDataTasks(taskFilePath, teamFilePath);

            Console.WriteLine(JsonConvert.SerializeObject(_ganttTasks,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            return JsonConvert.SerializeObject(_ganttTasks);
        }

        private void ProcessDataTasks(string taskFilePath, string teamFilePath)
        {
            _ganttTasks = LoadTasks(taskFilePath);
            _developerAvailability = LoadDevelopers(teamFilePath);
            AssignTasks();
        }

        public string ProcessExcelDataProjects(string taskFilePath, string teamFilePath)
        {
            ProcessDataTasks(taskFilePath, teamFilePath);

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

        private List<TaskDto> LoadTaskDtos(string filePath)
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

        private List<GanttTask> LoadTasks(string filePath)
        {
            var taskDtos = LoadTaskDtos(filePath);
            List<GanttTask> tasks = LoadTasksFromDtos(taskDtos);

            return tasks;
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

        private List<DeveloperAvailability> LoadDevelopers(string filePath)
        {
            var developers = new List<DeveloperAvailability>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RangeUsed().RowsUsed().Skip(1)) // Skip header
                {
                    var name = row.Cell(2).GetString();
                    var dailyHours = row.Cell(4).GetDouble();
                    var vacationDates = row.Cell(3).GetString().Split(';')
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

        private (DateTime Start, DateTime End)? ParseDateRange(string range)
        {
            var dates = range.Split('-');
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
            foreach (var task in _ganttTasks)
            {
                var assignedDeveloper = _developerAvailability.OrderBy(d => d.NextAvailableDate(startDate)).FirstOrDefault();
                if (assignedDeveloper == null) continue;
                DateTime taskStart = assignedDeveloper.NextAvailableDate(startDate);
                DateTime dependencyEndDate;
                taskStart = DateTime.TryParse(_ganttTasks.Find(t=> t.Dependencies == task.Id)?.End ?? "", out dependencyEndDate) ? dependencyEndDate : taskStart;
                double requiredDays = Math.Ceiling(task.EstimatedEffortHours / assignedDeveloper.DailyWorkHours);
                DateTime taskEnd = CalculateEndDate(taskStart, requiredDays, assignedDeveloper.VacationPeriods);
                task.Start = taskStart.ToString("yyyy-MM-dd");
                task.End = taskEnd.ToString("yyyy-MM-dd");

                task.Name = $"{task.Name} ({assignedDeveloper.Name})";                

                assignedDeveloper.SetNextAvailableDate(taskEnd.AddDays(1));
            }

        }

        private void AssignProjects(List<GanttTask> tasks, List<DeveloperAvailability> developers)
        {
            DateTime startDate = DateTime.Today;
            foreach (var task in tasks)
            {
                var assignedDeveloper = developers.OrderBy(d => d.NextAvailableDate(startDate)).FirstOrDefault();
                if (assignedDeveloper == null) continue;

                DateTime taskStart = assignedDeveloper.NextAvailableDate(startDate);
                double requiredDays = Math.Ceiling(task.EstimatedEffortHours / assignedDeveloper.DailyWorkHours);
                DateTime taskEnd = CalculateEndDate(taskStart, requiredDays, assignedDeveloper.VacationPeriods);
                task.Start = taskStart.ToString("yyyy-MM-dd");
                task.End = taskEnd.ToString("yyyy-MM-dd");

                task.Name = task.Name;                

                assignedDeveloper.SetNextAvailableDate(taskEnd.AddDays(1));
            }

        }

        private DateTime CalculateEndDate(DateTime start, double workDays, List<(DateTime Start, DateTime End)?> vacations)
        {
            DateTime end = start;
            while (workDays > 0)
            {
                if (!IsVacationDay(end, vacations)) workDays--;
                end = end.AddDays(1);
            }
            return end.AddDays(-1);
        }

        private bool IsVacationDay(DateTime date, List<(DateTime Start, DateTime End)?> vacations)
        {
            return vacations.Any(v => v.HasValue && date >= v.Value.Start && date <= v.Value.End);
        }
    }

    public class GanttTask
    {      
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; } // Format: "YYYY-MM-DD"

        [JsonProperty("end")]
        public string End { get; set; }   // Format: "YYYY-MM-DD"

        [JsonProperty("progress")]
        public int Progress { get; set; } = 0;

        [JsonProperty("dependencies")]
        public string Dependencies { get; set; } = "";
        
        [JsonProperty("customClass")]
        public string CustomClass { get; set; } // New property for styling

        [JsonProperty("resource")]
        public string Resource { get; set; }
        
        public double EstimatedEffortHours { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDependency { get; set; }
    }

    public class DeveloperAvailability
    {
        public string Name { get; set; }
        public double DailyWorkHours { get; set; }
        public List<(DateTime Start, DateTime End)?> VacationPeriods { get; set; }
        private DateTime nextAvailableDate = DateTime.Today;

        public DateTime NextAvailableDate(DateTime fromDate)
        {
            DateTime date = fromDate > nextAvailableDate ? fromDate : nextAvailableDate;
            while (IsOnVacation(date)) date = date.AddDays(1);
            return date;
        }

        private bool IsOnVacation(DateTime date)
        {
            return VacationPeriods.Any(v => v.HasValue && date >= v.Value.Start && date <= v.Value.End);
        }

        public void SetNextAvailableDate(DateTime date)
        {
            nextAvailableDate = date;
        }
    }


}