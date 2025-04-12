using ClosedXML.Excel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SpreadsheetUtility.Library.Providers;

namespace SpreadsheetUtility.Library
{     
    public class ProjectGroup
    {
        public string ProjectGroupID { get; set; } = "";
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    public class GanttChartProcessor: IGanttChartProcessor
    {
        private List<Project> _projectInputList;
        private List<Project> _projectOutputList;
        private List<GanttTask> _ganttTaskList;
        private List<GanttTask> _ganttProjectList;
        private List<Developer> _developerList;
        private List<Holiday> _holidayList;
        private List<Holiday> _projectHolidayList;
        private int _currentMaximumTaskID = 0;
        private readonly IDateTimeProvider _dateTimeProvider;
        private DateTime _projectStartDate;
        public GanttChartProcessor(IDateTimeProvider dateTimeProvider)
        {
            _projectInputList = new List<Project>();
            _projectOutputList = new List<Project>();
            _ganttTaskList = new List<GanttTask>();
            _ganttProjectList = new List<GanttTask>();
            _developerList = new List<Developer>();
            _projectHolidayList = new List<Holiday>();
            _dateTimeProvider = dateTimeProvider;
            _projectStartDate = _dateTimeProvider.Today;
            _holidayList = ProcessHolidays();
        }        

        #region dto processing
        public CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input)
        {
            _projectInputList = LoadProjectsFromDtos(input.ProjectDtos);            
            _ganttTaskList = LoadTasksFromDtos(input.TaskDtos);
            _developerList = LoadTeamDataFromDtos(input.DeveloperDtos);
            _projectStartDate = input.ProjectStartDate;

            //group projects by ProjectGroup
            var projectGroupList = _projectInputList.GroupBy(p => p.ProjectGroup)
                .Select(g => new ProjectGroup
                {
                    ProjectGroupID = g.Key ?? "",
                    Projects = g.ToList()
                }).ToList();

            foreach (var projectGroup in projectGroupList)
            {
                AssignTasks(projectGroup, input.PreSortTasks);
            }
            //obtain ProjectList from aggregated tasks
            GenerateProjectListFromTasks();

            CalculateDeveloperHours();

            var developerAvailability = MapDeveloperAvailability();
            
            return new CalculateGanttChartAllocationOutput
            {
                ProjectList = _projectOutputList,
                GanttTasks = _ganttTaskList,
                GanttProjects = _ganttProjectList,
                DeveloperAvailability = developerAvailability,
                HolidayList = _projectHolidayList                
            };
        }
        public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
        {
            return taskDtos.Select(dto => new GanttTask
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
        }

        private void GenerateProjectListFromTasks()
        {
            _projectOutputList = _ganttTaskList.GroupBy(t => t.ProjectName)
                .Select(g => new Project
                {
                    ProjectID = g.First().ProjectID,
                    ProjectName = g.Key,                                                    
                    StartDate = g.Min(t => t.StartDate),
                    EndDate = g.Max(t => t.EndDate),
                    TotalEstimatedEffortHours = g.Sum(t => t.EstimatedEffortHours),
                    ProjectGroup = _projectInputList.Find(p => p.ProjectName == g.Key)?.ProjectGroup
                }).ToList();

            double totalEstimatedEffortHours = _ganttTaskList.Sum(t => t.EstimatedEffortHours);            
            _ganttProjectList = _ganttTaskList.GroupBy(t => t.ProjectName)
                .Select(g => new GanttTask
                {
                    Id = g.First().ProjectID ?? "",
                    Name = $"{g.Key} ({g.Sum(t => t.EstimatedEffortHours)} hours)",
                    Dependencies = g.Select(t => t.ProjectDependency).Where(d => !string.IsNullOrEmpty(d)).FirstOrDefault() ?? string.Empty,
                    EstimatedEffortHours = g.Sum(t => t.EstimatedEffortHours),
                    Progress = (int)(g.Sum(t => (t.Progress * (t.EstimatedEffortHours / totalEstimatedEffortHours)))), // Summing the progress of each related task
                    StartDate = g.Min(t => t.StartDate),
                    EndDate = g.Max(t => t.EndDate),
                    Start = g.Min(t => t.StartDate).ToString("yyyy-MM-dd"),
                    End = g.Max(t => t.EndDate).ToString("yyyy-MM-dd"),
                    ProjectName = g.Key,
                    ProjectID = g.First().ProjectID ?? "",
                }).ToList();

            Console.WriteLine(JsonConvert.SerializeObject(_projectInputList,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

            Console.WriteLine(JsonConvert.SerializeObject(_ganttProjectList,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );

        }

        private List<Project> LoadProjectsFromDtos(List<ProjectDto> projectDtos)
        {
            return projectDtos.Select(dto => new Project
            {
                ProjectID = dto.ProjectID,
                ProjectName = dto.ProjectName,                
                ProjectGroup = dto.ProjectGroup
            }).ToList();
        }

               
        private List<Developer> LoadTeamDataFromDtos(List<DeveloperDto> developerDtos)
        {
            return developerDtos.Select(dto => new Developer(_dateTimeProvider)
            {
                Name = $"{dto.Team} : {dto.Name}",
                DailyWorkHours = dto.DailyWorkHours,
                VacationPeriods = dto.VacationPeriods.Split('|')
                        .Select(ParseDateRange)
                        .Where(d => d != null)
                        .ToList()
            }).ToList();
        }        

        private List<DeveloperAvailability> MapDeveloperAvailability()
        {
            return _developerList.Select(d => new DeveloperAvailability
            {
                Name = d.Name,
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
        #endregion

        #region task processing            
        
        private void CalculateDeveloperHours()
        {
            if(_ganttTaskList.Count == 0 || _developerList.Count == 0) return;
            DateTime minDate = _ganttTaskList.Min(t => t.StartDate);
            DateTime maxDate = _ganttTaskList.Max(t => t.EndDate);

            //calculate the sum of hours of non allocation for each developer
            foreach (var developer in _developerList)
            {
                if(developer != null)
                {
                    var calculatedIntervalDays = CalculateIntervalDays(minDate, maxDate, developer?.VacationPeriods);
                    developer.AllocatedHours = developer.Tasks?.Sum(t => t.EstimatedEffortHours) ?? 0;
                    developer.TotalHours = calculatedIntervalDays * developer.DailyWorkHours;
                    var slackHours = developer.TotalHours - developer.AllocatedHours;
                    developer.SlackHours = slackHours >= 0 ? slackHours : 0;
                }
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

        private void AssignTasks(ProjectGroup projectGroupList, bool preSortTasks = false)
        {
            //filter all tasks that belong to the projects who are assigned to the projectGroupID
            //var projectTasks = _ganttTaskList.Select(t => t.ProjectID).Intersect(projectGroupList.Projects.Select(p => p.ProjectID)).ToList();

            ////filter all tasks whose ID is in the projectTasks list
            //projectTasks = _ganttTaskList.Where(t => projectTasks.Contains(t.ProjectID)).Select(t => t.Id).ToList();

            var projectTasks = _ganttTaskList.Where(t => projectGroupList.Projects.Select(p=>p.ProjectID).Contains(t.ProjectID)).ToList();

            DateTime startDate = _projectStartDate;
            while (IsWeekendOrHoliday(startDate))
            {
                startDate = startDate.AddDays(1);
            }

            if (preSortTasks)
            {
                projectTasks = PreSortTasks(projectTasks);
            }

            foreach (var task in projectTasks)
            {
                var assignedDeveloper = _developerList.OrderBy(d => d.NextAvailableDate(startDate)).FirstOrDefault();
                if (assignedDeveloper == null) continue;

                DateTime taskStart = assignedDeveloper.NextAvailableDate(startDate);
                var taskStartFromDependency = projectTasks.Find(t => t.Id == task.Dependencies)?.EndDate ?? taskStart;

                taskStart = taskStart > taskStartFromDependency ? taskStart : taskStartFromDependency;

                double requiredDays = Math.Ceiling(task.EstimatedEffortHours / assignedDeveloper.DailyWorkHours);
                DateTime taskEnd = CalculateEndDate(taskStart, requiredDays, assignedDeveloper.VacationPeriods);

                task.Start = taskStart.ToString("yyyy-MM-dd");
                task.End = taskEnd.ToString("yyyy-MM-dd");
                //TaskEndWeek is equal to the first day of the week of the task end date
                task.TaskEndWeek = $"Week of {taskEnd.AddDays(-(int)(taskEnd.DayOfWeek - 1)).ToString("yyyy-MM-dd")}";

                task.StartDate = taskStart;
                task.EndDate = taskEnd;
                task.AssignedDeveloper = assignedDeveloper.Name;
                task.Name = $"{task.Name} ({assignedDeveloper.Name})";                
                assignedDeveloper.Tasks.Add(task);
                assignedDeveloper.SetNextAvailableDate(taskEnd.AddDays(1));                
            }
        }

        private List<GanttTask> PreSortTasks(List<GanttTask> projectTasks)
        {
            List<GanttTask> sortedTasks = projectTasks;
            // Change Dependencies from empty string to "0" for sorting
            foreach (var task in projectTasks)
            {
                if (string.IsNullOrEmpty(task.Dependencies))
                {
                    task.Dependencies = "0";
                }
            }

            // Sort tasks by dependencies and estimated effort hours
            sortedTasks = projectTasks.OrderBy(t => t.Dependencies).ThenByDescending(t => t.EstimatedEffortHours).ToList();

            // Create a mapping of original task IDs to new task IDs
            var idMapping = new Dictionary<string, string>();
            for (int i = _currentMaximumTaskID; i < sortedTasks.Count; i++)
            {
                var originalTaskId = sortedTasks[i].Id;
                var newTaskId = (i + 1).ToString();
                idMapping[originalTaskId] = newTaskId;
                sortedTasks[i].Id = newTaskId;

                if (sortedTasks[i].Dependencies == "0")
                {
                    sortedTasks[i].Dependencies = "";
                }
                _currentMaximumTaskID = int.Parse(newTaskId);
            }

            // Update the dependencies to the new sorted order
            foreach (var task in sortedTasks)
            {
                if (!string.IsNullOrEmpty(task.Dependencies) && idMapping.ContainsKey(task.Dependencies))
                {
                    task.Dependencies = idMapping[task.Dependencies];
                }
            }

            projectTasks = sortedTasks;
            return projectTasks;
        }

        private DateTime CalculateEndDate(DateTime start, double workDays, List<(DateTime Start, DateTime End)?>? vacations)
        {
            DateTime end = start;
            while (workDays > 0)
            {
                if (!IsVacationDay(end, vacations) && !IsWeekendOrHoliday(end)) workDays--;
                end = end.AddDays(1);
            }
            return end.AddDays(-1);
        }

        private int CalculateIntervalDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?>? vacations)
        {
            int days = 0;
            var startDate = start;
            while (startDate < end)
            {
                if (!IsVacationDay(startDate, vacations) && !IsWeekendOrHoliday(startDate))
                {
                   days++;
                }
                startDate = startDate.AddDays(1);
            }
            return days;
        }

        private bool IsVacationDay(DateTime date, List<(DateTime Start, DateTime End)?>? vacations)
        {
            return vacations?.Any(v => v.HasValue && date >= v.Value.Start && date <= v.Value.End) ?? false;
        }

        private bool IsWeekendOrHoliday(DateTime date)
        {
            return IsWeekend(date) || IsHoliday(date);
        }
        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
        private bool IsHoliday(DateTime date)
        {
            bool isHoliday = false;
            isHoliday = _holidayList.Select(h=>h.Date).Contains(date);
            if(isHoliday)
            {
                if(!_projectHolidayList.Select(h => h.Date).Contains(date))
                    _projectHolidayList.Add(_holidayList.Find(h => h.Date == date) ?? new Holiday());                
            }
            return isHoliday;
        }

        private List<Holiday> ProcessHolidays()
        {
            var holidayList = new List<Holiday>();
            var filePath = Path.Combine(AppContext.BaseDirectory, "Holidays", "2025HolidaysPT.json");

            try
            {
                var jsonString = File.ReadAllText(filePath);
                holidayList = JsonConvert.DeserializeObject<List<Holiday>>(jsonString) ?? new List<Holiday>();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, JSON parsing errors)
                Console.WriteLine($"An error occurred processing holidays: {ex.Message}");
            }

            return holidayList;
        }

        #endregion
    }

}