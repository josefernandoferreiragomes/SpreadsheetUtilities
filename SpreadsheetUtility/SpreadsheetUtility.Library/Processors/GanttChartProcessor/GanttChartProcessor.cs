using Microsoft.Extensions.Logging;
using SpreadsheetUtility.Library.Builders;
using SpreadsheetUtility.Library.Calculators;
using SpreadsheetUtility.Library.Grouppers;
using SpreadsheetUtility.Library.ListGenerators;
using SpreadsheetUtility.Library.Mappers;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Providers;
using SpreadsheetUtility.Library.TaskAssigners;

namespace SpreadsheetUtility.Library.Processors;

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
    private readonly ITaskAssignmentStrategyFactory _taskAssignmentStrategyFactory;
    private readonly ITaskSortingStrategyFactory _taskSortingStrategyFactory;
    private readonly IListGenerator<GanttTask, Project> _projectListGenerator;
    private readonly IListGenerator<GanttTask, GanttTask> _ganttTaskListGenerator;
    private readonly IListGenerator<Developer, GanttTask> _developerListGenerator;
    private readonly ICalculatorFacade _calculatorFacade;
    private readonly LoggingInvoker _loggingInvoker;
    private readonly GroupProjectsByProjectGroupQuery _groupProjectsQuery;

    public GanttChartProcessor(
        ILogger<List<GanttTask>> logger,
        IDateTimeProvider dateTimeProvider,
        IGanttChartMapper ganttChartMapper,
        IDateCalculator dateCalculator,
        ITaskAssignmentStrategyFactory taskAssignmentStrategyFactory,
        ITaskSortingStrategyFactory taskSortingStrategyFactory,
        IListGenerator<GanttTask, Project> projectListGenerator,
        IListGenerator<GanttTask, GanttTask> ganttTaskListGenerator,
        IListGenerator<Developer, GanttTask> developerListGenerator,
        ICalculatorFacade calculatorFacade,
        LoggingInvoker loggingInvoker,
        GroupProjectsByProjectGroupQuery groupProjectsQuery
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
        _taskAssignmentStrategyFactory = taskAssignmentStrategyFactory;
        _taskSortingStrategyFactory = taskSortingStrategyFactory;
        _projectListGenerator = projectListGenerator;
        _ganttTaskListGenerator = ganttTaskListGenerator;
        _developerListGenerator = developerListGenerator;
        _calculatorFacade = calculatorFacade;
        _loggingInvoker = loggingInvoker;
        _groupProjectsQuery = groupProjectsQuery;
    }

    #region Processing Tasks
    public List<GanttTask> LoadTasksFromDtos(List<TaskDto> taskDtos)
    {
        return _ganttChartMapper.MapGanttTasksFromTaskDtos(taskDtos);
    }

    public CalculateGanttChartAllocationOutput CalculateGanttChartAllocation(CalculateGanttChartAllocationInput input)
    {
        _dateCalculator.AddObserver(this);
        _projectInputList = _ganttChartMapper.MapProjectsFromProjectDtos(input.ProjectDtos);
        _ganttTaskList = _ganttChartMapper.MapGanttTasksFromTaskDtos(input.TaskDtos);
        _developerList = _ganttChartMapper.MapDevelopersFromDeveloperDtos(input.DeveloperDtos);
        _projectStartDate = input.ProjectStartDate;
        //group projects by ProjectGroup
        List<ProjectGroup> projectGroupList = GroupProjectsByProjectGroup();          

        foreach (var projectGroup in projectGroupList)
        {
            //filter all tasks that belong to the projects who are assigned to the projectGroupID                
            var projectTasks = _ganttTaskList.Where(t => projectGroup.Projects.Select(p => p.ProjectID).Contains(t.ProjectID)).ToList();
            //filter the developers from the project group team
            var developerList = new List<Developer>();
            if (input.SetTeamsToProjectGroups)
            {
                developerList = _developerList.Where(d => projectGroup.Projects.Select(p => p.TeamId).Contains(d.TeamId)).ToList();
            }
            else
            {
                developerList = _developerList;
            }
            AssignTasks(projectGroup, developerList, projectTasks, _projectStartDate, input.PreSortTasks);
        }
        //obtain ProjectList from aggregated tasks
        var projectOutputList = GenerateProjectListFromTasks();            
        var ganttProjectList = GenerateGanttProjectListFromTasks();
        CalculateDeveloperHours();
        var developerGanttTasks = GenerateGanttTaskListFromDevelopers();
        var developerAvailability = _ganttChartMapper.MapDeveloperAvailabilitiesFromDevelopers(_developerList);

        PerformLogging(ganttProjectList);

        var output = new CalculateGanttChartAllocationOutputBuilder()
           .WithProjects(projectOutputList)
           .WithGanttTasks(_ganttTaskList)
           .WithGanttProjects(ganttProjectList)
           .WithDeveloperTasks(developerGanttTasks)
           .WithDeveloperAvailability(developerAvailability)
           .WithHolidayList(_projectHolidayList)
           .Build();

        return output;
    }
    #endregion

    #region Observable
    public void OnCompleted()
        => throw new NotImplementedException();

    public void OnError(Exception error)
        => throw error;

    public void OnNext(Holiday holiday)
    {
        //iff the holiday is not already in the list, add it
        if (_projectHolidayList.Any(h => h.Date == holiday.Date && h.HolidayDescription == holiday.HolidayDescription))
            return;
        //add the holiday to the list
        _projectHolidayList.Add(holiday);
    }

    #endregion

    #region Private Methods
    private List<ProjectGroup> GroupProjectsByProjectGroup()
        => _groupProjectsQuery.Execute(_projectInputList);       

    private void AssignTasks(ProjectGroup projectGroupList, List<Developer> developerList, List<GanttTask> projectTasks, DateTime projectStartDate, bool preSortTasks = false)
    {                       
        var taskAssignmentStrategy = _taskAssignmentStrategyFactory.GetStrategy(TaskAssignmentStrategyType.Default);
        var taskSortingStrategy = _taskSortingStrategyFactory.GetStrategy(preSortTasks);     
        
        var taskSortingStrategyResult = taskSortingStrategy.SortTasks(projectTasks, _currentMaximumTaskID);

        _currentMaximumTaskID = taskSortingStrategyResult.newMaximumTaskID;
        projectTasks = taskSortingStrategyResult.ganttTaskList;            

        taskAssignmentStrategy.AssignTasks(projectGroupList, developerList, projectTasks, _projectStartDate);
    }

    private List<Project> GenerateProjectListFromTasks()            
        => _projectListGenerator.GenerateList(
            _ganttTaskList,
            new ListGeneratorInput()
            {
                projectInputList = _projectInputList,
                projectStartDate = _projectStartDate,
            }
        );   

    private List<GanttTask> GenerateGanttProjectListFromTasks()
        => _ganttTaskListGenerator.GenerateList(
            _ganttTaskList,
            new ListGeneratorInput()
            {
                projectInputList = _projectInputList,
                projectStartDate = _projectStartDate,
            }
        );    

    private List<GanttTask> GenerateGanttTaskListFromDevelopers()    
        => _developerListGenerator.GenerateList(
            _developerList, 
            new ListGeneratorInput()
            {
                projectInputList = _projectInputList,
                projectStartDate = _projectStartDate,
            }
        );    

    private void PerformLogging(List<GanttTask> ganttProjectList)
    {
        // Create and add logging commands
        _loggingInvoker.AddCommand(new ProjectInputLogCommand(_logger, _projectInputList));
        _loggingInvoker.AddCommand(new GanttTaskLogCommand(_logger, ganttProjectList));

        // Execute all logging commands
        _loggingInvoker.ExecuteCommands();
    }
   
    private void CalculateDeveloperHours()
        => _calculatorFacade.DeveloperHoursCalculator.CalculateDeveloperHours(_ganttTaskList, _developerList, _calculatorFacade.DateCalculator);         

    #endregion        
    
}
