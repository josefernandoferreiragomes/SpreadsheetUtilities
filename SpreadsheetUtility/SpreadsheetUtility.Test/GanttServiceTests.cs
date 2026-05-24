using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SpreadsheetUtility.Library.Calculators;
using SpreadsheetUtility.Library.ListGenerators;
using SpreadsheetUtility.Library.Mappers;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Processors;
using SpreadsheetUtility.Library.Providers;
using SpreadsheetUtility.Library.TaskAssigners;
using SpreadsheetUtility.Library.Services;
using SpreadsheetUtility.Test.Helpers;
using SpreadsheetUtility.Library.Grouppers;
using SpreadsheetUtility.Library.TaskSorters;
using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Test
{

    public class GanttServiceTests
    {
        private const string ParameterTypeInput = "Input";
        private const string ParameterTypeOutput = "Output";
        private readonly Mock<ILogger<List<GanttTask>>> _mockLogger;
        private readonly Mock<ILogger<List<Holiday>>> _mockHolidayProviderLogger;
        private readonly GanttChartDataManager _ganttService;
        private readonly Mock<IDateTimeProvider>? _mockDateTimeProvider;

        public GanttServiceTests()
        {
            // Create a service collection
            var services = new ServiceCollection();

            // Mock ILogger
            _mockLogger = new Mock<ILogger<List<GanttTask>>>();
            _mockHolidayProviderLogger = new Mock<ILogger<List<Holiday>>>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();

            // Register mocks and dependencies
            services.AddSingleton(_mockLogger.Object);
            services.AddSingleton(_mockHolidayProviderLogger.Object);            

            // Register IDateTimeProvider mock
            _mockDateTimeProvider.Setup(m => m.Today).Returns(new DateTime(2025, 03, 20));
            services.AddSingleton<IDateTimeProvider>(_mockDateTimeProvider.Object);

            // Register GanttChartMapper
            services.AddSingleton<IGanttChartMapper, GanttChartMapper>();

            // Register IHolidayProvider mock
            var holidayProviderMock = new Mock<IHolidayProvider>();
            var holidayInput = JsonTestHelper.ProcessMethodJson<List<Holiday>>("2025HolidaysPT", ParameterTypeInput);
            holidayProviderMock.Setup(h => h.LoadHolidaysFromConfigurationFile()).Returns(holidayInput);
            services.AddSingleton<IHolidayProvider>(holidayProviderMock.Object);

            //add GanttChartProcessor to middleware services:
            services.AddScoped<IGanttChartProcessor, GanttChartProcessor>();
            services.AddScoped<IGanttChartDataManager, GanttChartDataManager>();
            
            services.AddScoped<IGanttChartMapper, GanttChartMapper>();
            services.AddScoped<IDateCalculator, DateCalculator>();
            
            services.AddScoped<ITaskAssignmentStrategyFactory, TaskAssignmentStrategyFactory>();
            services.AddScoped<ITaskSortingStrategyFactory, TaskSortingStrategyFactory>();
            services.AddScoped<DefaultTaskAssignmentStrategy>();
            services.AddScoped<DefaultTaskSortingStrategy>();
            services.AddScoped<TaskSortingStrategyFactory>();
            services.AddScoped<TaskSortingStrategyEffortBased>();
            // Register the generalized ListGenerator implementations
            services.AddScoped<IListGenerator<GanttTask, Project>, GanttTaskProjectListGenerator>();
            services.AddScoped<IListGenerator<GanttTask, GanttTask>, GanttTaskListGenerator>();
            services.AddScoped<IListGenerator<Developer, List<GanttTask>>, DeveloperTaskListGenerator>();

            services.AddScoped<IDateCalculator, DateCalculator>();
            services.AddScoped<IDeveloperHoursCalculator, DeveloperHoursCalculator>();
            services.AddScoped<ICalculatorFacade, CalculatorFacade>();
            services.AddScoped<LoggingInvoker>();
            services.AddScoped<GroupProjectsByProjectGroupQuery>();

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            // Validate all scoped services (optional manual step)
            using (var scope = serviceProvider.CreateScope())
            {
                var servicesToValidate = new Type[]
                {
                    typeof(IGanttChartProcessor),
                    typeof(IGanttChartDataManager),
                    typeof(IDateTimeProvider),
                    typeof(IGanttChartMapper),
                    typeof(IDateCalculator),
                    typeof(IHolidayProvider),
                    typeof(ITaskAssignmentStrategyFactory),
                    typeof(ITaskSortingStrategyFactory),
                    typeof(DefaultTaskAssignmentStrategy),
                    typeof(DefaultTaskSortingStrategy),
                    typeof(TaskSortingStrategyEffortBased),
                    typeof(IListGenerator<GanttTask, Project>),
                    typeof(IListGenerator<GanttTask, GanttTask>),
                    typeof(IListGenerator<Developer, List<GanttTask>>),
                    typeof(IDeveloperHoursCalculator),
                    typeof(ICalculatorFacade),
                    typeof(LoggingInvoker),
                    typeof(GroupProjectsByProjectGroupQuery),

                };

                foreach (var serviceType in servicesToValidate)
                {
                    scope.ServiceProvider.GetRequiredService(serviceType); // Throws if not registered
                }
            }

            // Resolve GanttChartProcessor and GanttChartDataManager
            var ganttProcessor = serviceProvider.GetRequiredService<IGanttChartProcessor>();
            _ganttService = new GanttChartDataManager(ganttProcessor);
        }
        [Fact]
        public void CalculateGanttChartAllocationSort()
        {
            // With task dependencies, With sorting, One Project group, No fixed Team for project
            // Arrange
            var methodName = System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "";
            var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(methodName, ParameterTypeInput);
            var fixedDateTime = new DateTime(2025, 05, 11);
            _mockDateTimeProvider!.Setup(m => m.Today).Returns(fixedDateTime);
            Assert.NotNull(input);
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.GanttTasks.Count);
            Assert.Equal(4, result.DeveloperAvailability.Count);
            // Validate first task is assigned to a Team Alpha member
            Assert.Contains("Team Alpha", result.GanttTasks[0].AssignedDeveloper);
            // Validate all tasks have assignments
            Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
            // Validate developers are from the correct team
            Assert.True(result.GanttTasks.All(t => t.AssignedDeveloper.Contains("Team Alpha")), "All tasks should be assigned to Team Alpha");
        }
        [Fact]
        public void CalculateGanttChartAllocationProjectGroupFixedTeam()
        {
            // No task dependencies, no sorting, Two Project groups, fixed Team for project groups
            // Arrange
            var methodName = System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "";
            var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(methodName, ParameterTypeInput);
            var fixedDateTime = new DateTime(2025, 05, 11);
            _mockDateTimeProvider!.Setup(m => m.Today).Returns(fixedDateTime);
            Assert.NotNull(input);
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.GanttTasks.Count);
            Assert.Equal(4, result.DeveloperAvailability.Count);
            // All tasks should have assignments
            Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
        }
        [Fact]
        public void CalculateGanttChartAllocationNoDependenciesProjectGroup()
        {
            // No task dependencies, No sorting, Two Project groups, No fixed Team for project
            // Arrange
            var methodName = System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "";
            var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(methodName, ParameterTypeInput);
            var fixedDateTime = new DateTime(2025, 05, 11);
            _mockDateTimeProvider!.Setup(m => m.Today).Returns(fixedDateTime);
            Assert.NotNull(input);
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.GanttTasks.Count);
            Assert.Equal(4, result.DeveloperAvailability.Count);
            // All tasks should have assignments
            Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
        }
        [Fact]
        public void CalculateGanttChartAllocationNoDependencies()
        {
            // No task dependencies, No sorting, One Project group, No fixed Team for project
            // Arrange
            var methodName = System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "";
            var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>(methodName, ParameterTypeInput);
            var fixedDateTime = new DateTime(2025, 05, 11);
            _mockDateTimeProvider!.Setup(m => m.Today).Returns(fixedDateTime);
            Assert.NotNull(input);
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.GanttTasks.Count);
            Assert.Equal(4, result.DeveloperAvailability.Count);
            // All tasks should have assignments
            Assert.True(result.GanttTasks.All(t => !string.IsNullOrEmpty(t.AssignedDeveloperId)), "All tasks should have developer assignments");
            // Validate work distribution across team members
            var assignmentCounts = result.GanttTasks
                .GroupBy(t => t.AssignedDeveloperId)
                .ToDictionary(g => g.Key, g => g.Count());
            Assert.True(assignmentCounts.Values.All(count => count >= 2), "Work should be distributed across team members");
        }
        [Fact]
        public void CalculateGanttChartAllocationFromDtos_ShouldReturnCorrectAllocation_WhenValidDataProvided()
        {
            // Arrange
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
            _mockDateTimeProvider!.Setup(m => m.Today).Returns(fixedDateTime);

            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
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
        public void CalculateGanttChartAllocationFromDtos_ShouldHandleEmptyTaskList()
        {
            // Arrange
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
            _mockDateTimeProvider!.Setup(m => m.Today).Returns(fixedDateTime);

            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.GanttTasks);
            Assert.Single(result.DeveloperAvailability);
        }

        [Fact]
        public void CalculateGanttChartAllocationFromDtos_ShouldHandleEmptyDeveloperList()
        {
            // Arrange
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


            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.GanttTasks);
            Assert.Empty(result.DeveloperAvailability);
        }
    }

}
