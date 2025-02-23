using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Globalization;

namespace SpreadsheetUtility.Library
{
    public interface IGanttChartService
    {
        string ProcessExcelData(string taskFilePath, string teamFilePath);
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
    }



    public class GanttChartService: IGanttChartService
    {
        public string ProcessExcelData(string taskFilePath, string teamFilePath)
        {
            //var tasks = LoadTasks(taskFilePath);
            //var developers = LoadDevelopers(teamFilePath);
            //AssignTasksToDevelopers(tasks, developers);
            var tasks = new List<GanttTask>
            {
                new GanttTask { Id = "1", Name = "Design UI", Start = "2025-02-25", End = "2025-03-01", Progress = 50 },
                new GanttTask { Id = "2", Name = "Backend API", Start = "2025-03-02", End = "2025-03-10", Progress = 30, Dependencies = "1" }
            };

            Console.WriteLine(JsonConvert.SerializeObject(tasks, Formatting.Indented));
            return JsonConvert.SerializeObject(tasks);
        }

        private List<TaskAssignment> LoadTasks(string filePath)
        {
            var tasks = new List<TaskAssignment>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    tasks.Add(new TaskAssignment
                    {
                        Id = row.Cell(1).GetValue<int>(),
                        Name = row.Cell(2).GetValue<string>(),
                        EstimatedHours = row.Cell(3).GetValue<int>(),
                        Start = DateTime.Now,  // Placeholder, will be adjusted
                        Progress = 0
                    });
                }
            }
            return tasks;
        }

        private List<DeveloperAvailability> LoadDevelopers(string filePath)
        {
            var developers = new List<DeveloperAvailability>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    developers.Add(new DeveloperAvailability
                    {
                        Name = row.Cell(2).GetValue<string>(),
                        DailyWorkHours = row.Cell(4).GetValue<int>(),
                        Vacations = ParseVacationDates(row.Cell(3).GetValue<string>())
                    });
                }
            }
            return developers;
        }

        private void AssignTasksToDevelopers(List<TaskAssignment> tasks, List<DeveloperAvailability> developers)
        {
            DateTime currentDate = DateTime.Now;
            foreach (var task in tasks)
            {
                var developer = developers.OrderBy(d => d.NextAvailableDate(currentDate)).First();
                task.Start = developer.NextAvailableDate(currentDate);
                task.End = task.Start.AddHours(task.EstimatedHours / developer.DailyWorkHours * 8);
                developer.BlockDates(task.Start, task.End);
            }
        }

        private List<(DateTime Start, DateTime End)> ParseVacationDates(string vacationData)
        {
            var vacations = new List<(DateTime Start, DateTime End)>();
            if (!string.IsNullOrWhiteSpace(vacationData))
            {
                var ranges = vacationData.Split(';');
                foreach (var range in ranges)
                {
                    var dates = range.Split('-');
                    if (dates.Length == 2 &&
                        DateTime.TryParseExact(dates[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start) &&
                        DateTime.TryParseExact(dates[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end))
                    {
                        vacations.Add((start, end));
                    }
                }
            }
            return vacations;
        }
    }

    public class TaskAssignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Progress { get; set; }
        public string Dependencies { get; set; }
        internal int EstimatedHours { get; set; }
    }

    public class DeveloperAvailability
    {
        public string Name { get; set; }
        public int DailyWorkHours { get; set; }
        public List<(DateTime Start, DateTime End)> Vacations { get; set; } = new List<(DateTime, DateTime)>();

        public DateTime NextAvailableDate(DateTime fromDate)
        {
            while (Vacations.Any(v => fromDate >= v.Start && fromDate <= v.End))
            {
                fromDate = fromDate.AddDays(1);
            }
            return fromDate;
        }

        public void BlockDates(DateTime start, DateTime end)
        {
            Vacations.Add((start, end));
        }
    }
}