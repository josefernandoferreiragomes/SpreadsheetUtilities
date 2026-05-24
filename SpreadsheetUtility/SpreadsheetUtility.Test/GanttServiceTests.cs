using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SpreadsheetUtility.Application;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.CalculateGanttChartAllocation;
using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Test.Helpers;

namespace SpreadsheetUtility.Test;

public class GanttServiceTests
{
    private const string ParameterTypeInput = "Input";
    private const string ParameterTypeOutput = "Output";
    private readonly Mock<ILogger<List<GanttTask>>> _mockLogger;
    private readonly ServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly Mock<ILogger<List<Holiday>>> _mockHolidayProviderLogger;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;

    public GanttServiceTests()
    {
        var services = new ServiceCollection();

        _mockLogger = new Mock<ILogger<List<GanttTask>>>();
        _mockHolidayProviderLogger = new Mock<ILogger<List<Holiday>>>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        services.AddSingleton(_mockLogger.Object);
        services.AddSingleton(_mockHolidayProviderLogger.Object);
        services.AddLogging();

        // Register Application layer (MediatR, handlers, mappers, calculators, strategies)
        services.AddApplication();

        // Mock IDateTimeProvider
        _mockDateTimeProvider.Setup(m => m.Today).Returns(new DateTime(2025, 03, 20));
        services.AddSingleton<IDateTimeProvider>(_mockDateTimeProvider.Object);

        // Mock IHolidayProvider
        var holidayProviderMock = new Mock<IHolidayProvider>();
        var holidayInput = JsonTestHelper.ProcessMethodJson<List<Holiday>>("2025HolidaysPT", ParameterTypeInput);
        holidayProviderMock.Setup(h => h.LoadHolidaysFromConfigurationFile()).Returns(holidayInput);
        services.AddSingleton<IHolidayProvider>(holidayProviderMock.Object);

        _serviceProvider = services.BuildServiceProvider();
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task CalculateGanttChartAllocationSort()
    {
        var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(nameof(CalculateGanttChartAllocationSort), ParameterTypeInput);
        var fixedDateTime = new DateTime(2025, 05, 11);
        _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);
        Assert.NotNull(input);

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Equal(12, result.GanttTasks.Count);
        Assert.Equal(4, result.DeveloperAvailability.Count);
        Assert.Contains("Team Alpha", result.GanttTasks[0].AssignedDeveloper);
        Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
        Assert.True(result.GanttTasks.All(t => t.AssignedDeveloper.Contains("Team Alpha")), "All tasks should be assigned to Team Alpha");
    }

    [Fact]
    public async Task CalculateGanttChartAllocationProjectGroupFixedTeam()
    {
        var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(nameof(CalculateGanttChartAllocationProjectGroupFixedTeam), ParameterTypeInput);
        var fixedDateTime = new DateTime(2025, 05, 11);
        _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);
        Assert.NotNull(input);

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Equal(12, result.GanttTasks.Count);
        Assert.Equal(4, result.DeveloperAvailability.Count);
        Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
    }

    [Fact]
    public async Task CalculateGanttChartAllocationNoDependenciesProjectGroup()
    {
        var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(nameof(CalculateGanttChartAllocationNoDependenciesProjectGroup), ParameterTypeInput);
        var fixedDateTime = new DateTime(2025, 05, 11);
        _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);
        Assert.NotNull(input);

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Equal(12, result.GanttTasks.Count);
        Assert.Equal(4, result.DeveloperAvailability.Count);
        Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
    }

    [Fact]
    public async Task CalculateGanttChartAllocationNoDependencies()
    {
        var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(nameof(CalculateGanttChartAllocationNoDependencies), ParameterTypeInput);
        var fixedDateTime = new DateTime(2025, 05, 11);
        _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);
        Assert.NotNull(input);

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Equal(12, result.GanttTasks.Count);
        Assert.Equal(4, result.DeveloperAvailability.Count);
        Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
        var assignmentCounts = result.GanttTasks
            .GroupBy(t => t.AssignedDeveloperId)
            .ToDictionary(g => g.Key, g => g.Count());
        Assert.True(assignmentCounts.Values.All(count => count >= 2), "Work should be distributed across team members");
    }

    [Fact]
    public async Task CalculateGanttChartAllocationFromDtos_ShouldReturnCorrectAllocation_WhenValidDataProvided()
    {
        var taskDtoList = new List<TaskDto>
        {
            new TaskDto
            {
                Id = "1",
                ProjectID = "P1",
                ProjectName = "Project 1",
                TaskName = "Task 1",
                EffortHours = 10,
                Dependencies = "",
                Progress = "0"
            }
        };

        var projectDtoList = new List<ProjectDto>
        {
            new ProjectDto
            {
                ProjectID = "P1",
                ProjectName = "Project 1",
                ProjectGroup = "1",
                TeamId = "1",
            }
        };

        var developerDtoList = new List<DeveloperDto>
        {
            new DeveloperDto
            {
                TeamId = "1",
                Team = "Team 1",
                DeveloperId = "1",
                Name = "Dev 1",
                VacationPeriods = "",
                DailyWorkHours = 8
            }
        };

        bool preSortTasks = true;

        var input = new CalculateGanttChartAllocationInput
        {
            TaskDtos = taskDtoList,
            DeveloperDtos = developerDtoList,
            ProjectDtos = projectDtoList,
            PreSortTasks = preSortTasks
        };

        var fixedDateTime = new DateTime(2025, 03, 20);
        _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Single(result.GanttTasks);
        Assert.Single(result.DeveloperAvailability);

        var ganttTask = result.GanttTasks.First();
        Assert.Equal("1", ganttTask.Id);
        Assert.Equal("Project 1 : Task 1 (Team 1 : Dev 1)", ganttTask.TaskName);
        Assert.Equal(10, ganttTask.EffortHours);

        var developerAvailability = result.DeveloperAvailability.First();
        Assert.Equal("Team 1 : Dev 1", developerAvailability.Name);
        Assert.Equal(8, developerAvailability.DailyWorkHours);
    }

    [Fact]
    public async Task CalculateGanttChartAllocationFromDtos_ShouldHandleEmptyTaskList()
    {
        var taskDtoList = new List<TaskDto>();

        var projectDtoList = new List<ProjectDto>
        {
            new ProjectDto
            {
                ProjectID = "P1",
                ProjectName = "Project 1",
            }
        };

        var developerDtoList = new List<DeveloperDto>
        {
            new DeveloperDto
            {
                TeamId = "1",
                Team = "Team 1",
                DeveloperId = "1",
                Name = "Dev 1",
                VacationPeriods = "",
                DailyWorkHours = 8
            }
        };

        bool preSortTasks = true;

        var input = new CalculateGanttChartAllocationInput
        {
            TaskDtos = taskDtoList,
            DeveloperDtos = developerDtoList,
            ProjectDtos = projectDtoList,
            PreSortTasks = preSortTasks
        };

        var fixedDateTime = new DateTime(2025, 03, 20);
        _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Empty(result.GanttTasks);
        Assert.Single(result.DeveloperAvailability);
    }

    [Fact]
    public async Task CalculateGanttChartAllocationFromDtos_ShouldHandleEmptyDeveloperList()
    {
        var taskDtoList = new List<TaskDto>
        {
            new TaskDto
            {
                Id = "1",
                ProjectID = "P1",
                ProjectName = "Project 1",
                TaskName = "Task 1",
                EffortHours = 10,
                Dependencies = "",
                Progress = "0"
            }
        };

        var projectDtoList = new List<ProjectDto>
        {
            new ProjectDto
            {
                ProjectID = "P1",
                ProjectName = "Project 1",
            }
        };

        var developerDtoList = new List<DeveloperDto>();

        bool preSortTasks = true;

        var input = new CalculateGanttChartAllocationInput
        {
            TaskDtos = taskDtoList,
            DeveloperDtos = developerDtoList,
            ProjectDtos = projectDtoList,
            PreSortTasks = preSortTasks
        };

        var result = await _mediator.Send(new CalculateGanttChartAllocationQuery(input));

        Assert.NotNull(result);
        Assert.Single(result.GanttTasks);
        Assert.Empty(result.DeveloperAvailability);
    }
}
