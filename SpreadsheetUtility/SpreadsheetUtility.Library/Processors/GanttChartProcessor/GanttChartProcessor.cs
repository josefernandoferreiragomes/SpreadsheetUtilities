using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpreadsheetUtility.Library.Processors.GanttChartProcessor.Builders;
using SpreadsheetUtility.Library.Processors.GanttChartProcessor.Calculators;
using SpreadsheetUtility.Library.Processors.GanttChartProcessor.Mappers;
using SpreadsheetUtility.Library.Providers;
using System;

namespace SpreadsheetUtility.Library
{
    public class ProjectGroup
    {
        public string ProjectGroupID { get; set; } = "";
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    public class GanttChartProcessor : IGanttChartProcessor, IObserver<Holiday>
    {
        private readonly ILogger<List<GanttTask>> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGanttChartMapper _ganttChartMapper;
        private readonly IDateCalculator _dateCalculator;        
        private List<Project> _projectInputList;        
        private List<GanttTask> _ganttTaskList;        
        private List<Developer> _developerList;       
        private List<Holiday> _projectHolidayList;
        private int _currentMaximumTaskID = 0;
        private DateTime _projectStartDate;

        public GanttChartProcessor(
            ILogger<List<GanttTask>> logger,
            IDateTimeProvider dateTimeProvider,
            IGanttChartMapper ganttChartMapper,
            IDateCalculator dateCalculator
        )
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _ganttChartMapper = ganttChartMapper;
            _dateCalculator = dateCalculator;           
            _projectInputList = new List<Project>();            
            _ganttTaskList = new List<GanttTask>();            
            _developerList = new List<Developer>();
            _projectHolidayList = new List<Holiday>();
            _projectStartDate = _dateTimeProvider.Today;            
        }

        public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
        {
            return _ganttChartMapper.MapGanttTasksFromTaskDtos(taskDtos);
        }

        #region Processing
        public CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input)
        {
            _dateCalculator.AddObserver(this);
            _projectInputList = _ganttChartMapper.MapProjectsFromProjectDtos(input.ProjectDtos);
            _ganttTaskList = _ganttChartMapper.MapGanttTasksFromTaskDtos(input.TaskDtos);
            _developerList = _ganttChartMapper.MapDevelopersFromDeveloperDtos(input.DeveloperDtos);
            _projectStartDate = input.ProjectStartDate;
            //group projects by ProjectGroup
            List<ProjectGroup> projectGroupList = GroupProjectsByProjectGroup();

            //full pre sort tasks sort
            //if (input.PreSortTasks)
            //{
            //    _ganttTaskList = PreSortTasks_Experimental(_ganttTaskList);
            //}

            foreach (var projectGroup in projectGroupList)
            {
                //filter all tasks that belong to the projects who are assigned to the projectGroupID                
                var projectTasks = _ganttTaskList.Where(t => projectGroup.Projects.Select(p => p.ProjectID).Contains(t.ProjectID)).ToList();
                AssignTasks(projectGroup, _developerList, projectTasks, input.PreSortTasks);
            }
            //obtain ProjectList from aggregated tasks
            var projectOutputList = GenerateProjectListFromTasks();            
            var ganttProjectList = GenerateGanttProjectListFromTasks();
            CalculateDeveloperHours();

            var developerAvailability = _ganttChartMapper.MapDeveloperAvailabilitiesFromDevelopers(_developerList);

            PerformLogging(ganttProjectList);

            var output = new CalculateGanttChartAllocationOutputBuilder()
               .WithProjects(projectOutputList)
               .WithGanttTasks(_ganttTaskList)
               .WithGanttProjects(ganttProjectList)
               .WithDeveloperAvailability(developerAvailability)
               .WithHolidayList(_projectHolidayList)
               .Build();

            return output;
        }

        private List<ProjectGroup> GroupProjectsByProjectGroup()
            => _projectInputList.GroupBy(p => p.ProjectGroup)
                .Select(g => new ProjectGroup
                {
                    ProjectGroupID = g.Key ?? "",
                    Projects = g.ToList()
                }).ToList();

        private void AssignTasks(ProjectGroup projectGroupList, List<Developer> developerList, List<GanttTask> projectTasks, bool preSortTasks = false)
        {                        

            DateTime startDate = _projectStartDate;
            startDate = _dateCalculator.GetNextWorkingDay(startDate);

            //pre sort tasks by project group
            if (preSortTasks)
            {
                projectTasks = PreSortTasks_Experimental(projectTasks);
            }

            foreach (var task in projectTasks)
            {
                var assignedDeveloper = developerList.OrderBy(d => d.NextAvailableDate(startDate)).FirstOrDefault();
                if (assignedDeveloper == null) continue;

                DateTime taskStart = assignedDeveloper.NextAvailableDate(startDate);
                var taskStartFromDependency = projectTasks.Find(t => t.Id == task.Dependencies)?.EndDate ?? DateTime.MinValue;

                taskStart = taskStart > taskStartFromDependency ? taskStart : _dateCalculator.GetNextWorkingDay(taskStartFromDependency.AddDays(1));

                double requiredDays = Math.Ceiling(task.EstimatedEffortHours / assignedDeveloper.DailyWorkHours);
                DateTime taskEnd = _dateCalculator.CalculateEndDate(taskStart, requiredDays, assignedDeveloper.VacationPeriods);

                task.Start = taskStart.ToString("yyyy-MM-dd");
                task.End = taskEnd.ToString("yyyy-MM-dd");
                //TaskEndWeek is equal to the first day of the week of the task end date
                task.TaskEndWeek = $"Week of {taskEnd.AddDays(-(int)(taskEnd.DayOfWeek - 1)).ToString("yyyy-MM-dd")}";

                task.StartDate = taskStart;
                task.EndDate = taskEnd;
                task.AssignedDeveloper = assignedDeveloper.Name;
                task.Name = $"{task.Name} ({assignedDeveloper.Name})";
                assignedDeveloper.Tasks.Add(task);
                assignedDeveloper.SetNextAvailableDate(_dateCalculator.GetNextWorkingDay(taskEnd.AddDays(1)));
            }
        }

        private List<Project> GenerateProjectListFromTasks()        
            => _ganttTaskList.GroupBy(t => t.ProjectName)
                .Select(g => new Project
                {
                    ProjectID = g.First().ProjectID,
                    ProjectName = g.Key,
                    StartDate = g.Min(t => t.StartDate),
                    EndDate = g.Max(t => t.EndDate),
                    TotalEstimatedEffortHours = g.Sum(t => t.EstimatedEffortHours),
                    ProjectGroup = _projectInputList.Find(p => p.ProjectName == g.Key)?.ProjectGroup
                }).ToList();

        private List<GanttTask> GenerateGanttProjectListFromTasks()
        { 
            double totalEstimatedEffortHours = _ganttTaskList.Sum(t => t.EstimatedEffortHours);

            var ganttProjectList = _ganttTaskList.GroupBy(t => t.ProjectName)
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
            return ganttProjectList;
        }
        private void PerformLogging(List<GanttTask> ganttProjectList)
        {
            _logger.LogDebug(JsonConvert.SerializeObject(_projectInputList,
               new JsonSerializerSettings
               {
                   ContractResolver = new CamelCasePropertyNamesContractResolver(),
                   Formatting = Formatting.Indented
               })
           );

            _logger.LogDebug(JsonConvert.SerializeObject(ganttProjectList,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                })
            );
        }
        private void CalculateDeveloperHours()
        {
            if (_ganttTaskList.Count == 0 || _developerList.Count == 0) return;
            DateTime minDate = _ganttTaskList.Min(t => t.StartDate);
            DateTime maxDate = _ganttTaskList.Max(t => t.EndDate);

            //calculate the sum of hours of non allocation for each developer
            foreach (var developer in _developerList)
            {
                if (developer != null)
                {
                    var calculatedIntervalDays = _dateCalculator.CalculateIntervalDays(minDate, maxDate, developer?.VacationPeriods);
                    developer!.AllocatedHours = developer.Tasks?.Sum(t => t.EstimatedEffortHours) ?? 0;
                    developer.TotalHours = calculatedIntervalDays * developer.DailyWorkHours;
                    var slackHours = developer.TotalHours - developer.AllocatedHours;
                    developer.SlackHours = slackHours >= 0 ? slackHours : 0;
                }
            }
        }

        #endregion




        private List<GanttTask> PreSortTasks_Experimental(List<GanttTask> projectTasks)
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

        #region Observable
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(Holiday holiday)
        {
            //iff the holiday is not already in the list, add it
            if (_projectHolidayList.Any(h => h.Date == holiday.Date && h.HolidayDescription == holiday.HolidayDescription))
                return;
            //add the holiday to the list
            _projectHolidayList.Add(holiday);
        }

        #endregion
    }
    
}