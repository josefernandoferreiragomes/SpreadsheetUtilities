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
    }

    public class GanttChartService: IGanttChartService
    {
        public string ProcessExcelDataTasks(string taskFilePath, string teamFilePath)
        {
            var tasks = LoadTasks(taskFilePath);
            var developers = LoadDevelopers(teamFilePath);
            AssignTasks(tasks, developers);          

            Console.WriteLine(JsonConvert.SerializeObject(tasks,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            return JsonConvert.SerializeObject(tasks);
        }

        public string ProcessExcelDataProjects(string taskFilePath, string teamFilePath)
        {
            var tasks = LoadTasks(taskFilePath);
            var developers = LoadDevelopers(teamFilePath);

            var projects = tasks.GroupBy(t => t.ProjectName)
                .Select(g => new GanttTask
                {
                    Id = g.First().Id,
                    Name = g.Key,
                    EstimatedEffortHours = g.Sum(t => t.EstimatedEffortHours),
                    Dependencies = string.Join(",", g.Select(t => t.Dependencies).Where(d => !string.IsNullOrEmpty(d))),
                    Progress = (int)g.Average(t => t.Progress)
                }).ToList();

            AssignProjects(projects, developers);

            Console.WriteLine(JsonConvert.SerializeObject(projects,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            return JsonConvert.SerializeObject(projects);
        }       

        private List<TaskDto> LoadTaskDtos(string filePath)
        {
            var taskDtos = new List<TaskDto>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RangeUsed().RowsUsed().Skip(1)) // Skip header
                {
                    taskDtos.Add(new TaskDto
                    {
                        Id = row.Cell("A").GetString(),
                        ProjectName = row.Cell("B").GetString(),
                        TaskName = row.Cell("C").GetString(),
                        EstimatedEffortHours = row.Cell("D").GetDouble(),
                        Dependencies = row.Cell("E").GetString(),
                        Progress = row.Cell("F").GetString()
                    });
                }
            }
            return taskDtos;
        }

        private List<GanttTask> LoadTasks(string filePath)
        {
            var taskDtos = LoadTaskDtos(filePath);
            var tasks = taskDtos.Select(dto => new GanttTask
            {
                Id = dto.Id,
                Name = $"{dto.ProjectName} : {dto.TaskName}",
                EstimatedEffortHours = dto.EstimatedEffortHours,
                Dependencies = dto.Dependencies,
                Progress = int.TryParse(dto.Progress, out var p) ? p : 0,
                ProjectName = dto.ProjectName,
                TaskName = dto.TaskName
            }).ToList();

            return tasks;
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

        private void AssignTasks(List<GanttTask> tasks, List<DeveloperAvailability> developers)
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
        
        internal double EstimatedEffortHours { get; set; }
        internal string ProjectName { get; set; }
        internal string TaskName { get; set; }
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