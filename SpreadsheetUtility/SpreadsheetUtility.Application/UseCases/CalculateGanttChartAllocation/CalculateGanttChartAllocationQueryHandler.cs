using MediatR;
using Microsoft.Extensions.Logging;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.Mappers;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.Services;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.UseCases.CalculateGanttChartAllocation;

public class CalculateGanttChartAllocationQueryHandler : IRequestHandler<CalculateGanttChartAllocationQuery, CalculateGanttChartAllocationOutput>, IObserver<Holiday>
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
    private readonly IListGenerator<Developer, List<GanttTask>> _developerTaskListGenerator;
    private readonly ICalculatorFacade _calculatorFacade;
    private readonly GroupProjectsByProjectGroupQuery _groupProjectsQuery;

    public CalculateGanttChartAllocationQueryHandler(
        ILogger<List<GanttTask>> logger,
        IDateTimeProvider dateTimeProvider,
        IGanttChartMapper ganttChartMapper,
        IDateCalculator dateCalculator,
        ITaskAssignmentStrategyFactory taskAssignmentStrategyFactory,
        ITaskSortingStrategyFactory taskSortingStrategyFactory,
        IListGenerator<GanttTask, Project> projectListGenerator,
        IListGenerator<GanttTask, GanttTask> ganttTaskListGenerator,
        IListGenerator<Developer, List<GanttTask>> developerTaskListGenerator,
        ICalculatorFacade calculatorFacade,
        GroupProjectsByProjectGroupQuery groupProjectsQuery)
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
        _developerTaskListGenerator = developerTaskListGenerator;
        _calculatorFacade = calculatorFacade;
        _groupProjectsQuery = groupProjectsQuery;
    }

    public Task<CalculateGanttChartAllocationOutput> Handle(CalculateGanttChartAllocationQuery request, CancellationToken cancellationToken)
    {
        var input = request.Input;
        _dateCalculator.AddObserver(this);

        _projectInputList = _ganttChartMapper.MapProjectsFromProjectDtos(input.ProjectDtos);
        _ganttTaskList = _ganttChartMapper.MapGanttTasksFromTaskDtos(input.TaskDtos);
        _developerList = _ganttChartMapper.MapDevelopersFromDeveloperDtos(input.DeveloperDtos);
        _projectStartDate = input.ProjectStartDate;

        List<ProjectGroup> projectGroupList = GroupProjectsByProjectGroup();

        foreach (var projectGroup in projectGroupList)
        {
            var projectTasks = _ganttTaskList.Where(t => projectGroup.Projects.Select(p => p.ProjectID).Contains(t.ProjectID)).ToList();
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

        var projectOutputList = GenerateProjectListFromTasks();
        var ganttProjectList = GenerateGanttProjectListFromTasks();
        CalculateDeveloperHours();
        var developerGanttTasks = GenerateGanttTaskListFromDevelopers();
        var developerAvailability = _ganttChartMapper.MapDeveloperAvailabilitiesFromDevelopers(_developerList);

        var output = new CalculateGanttChartAllocationOutputBuilder()
           .WithProjects(projectOutputList)
           .WithGanttTasks(_ganttTaskList)
           .WithGanttProjects(ganttProjectList)
           .WithDeveloperTasks(developerGanttTasks)
           .WithDeveloperAvailability(developerAvailability)
           .WithHolidayList(_projectHolidayList)
           .Build();

        _dateCalculator.RemoveObserver(this);

        return Task.FromResult(output);
    }

    public void OnCompleted() { }

    public void OnError(Exception error) => throw error;

    public void OnNext(Holiday holiday)
    {
        if (_projectHolidayList.Any(h => h.Date == holiday.Date && h.HolidayDescription == holiday.HolidayDescription))
            return;
        _projectHolidayList.Add(holiday);
    }

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
        => _developerTaskListGenerator.GenerateList(
            _developerList,
            new ListGeneratorInput()
            {
                projectInputList = _projectInputList,
                projectStartDate = _projectStartDate,
            }
        ).SelectMany(x => x).ToList();

    private void CalculateDeveloperHours()
        => _calculatorFacade.DeveloperHoursCalculator.CalculateDeveloperHours(_ganttTaskList, _developerList, _calculatorFacade.DateCalculator);
}
