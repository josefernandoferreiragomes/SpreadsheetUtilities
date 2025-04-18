using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SpreadsheetUtility.Library;
using SpreadsheetUtility.Library.Mappers;
using SpreadsheetUtility.Library.Providers;
using SpreadsheetUtility.Services;
using SpreadsheetUtility.Test.Helpers;

namespace SpreadsheetUtility.Test
{

    public class GanttServiceTests
    {
        private readonly GanttService _ganttService;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly Mock<ILogger> _mockLogger;

        public GanttServiceTests()
        {
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var mapper = new GanttChartMapper(_mockDateTimeProvider.Object);
            var ganttProcessor = new GanttChartProcessor(_mockLogger.Object, _mockDateTimeProvider?.Object!, mapper);
            _ganttService = new GanttService(ganttProcessor);
        }
        
        [Fact]
        public void CalculateGanttChartAllocation()
        {
            // Arrange
            var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>("CalculateGanttChartAllocation","Input");
            var fixedDateTime = new DateTime(2025, 03, 20);
            _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);
            Assert.NotNull(input);
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            var expected = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationOutput>("CalculateGanttChartAllocation","Output");
            Assert.Equal(expected.GanttTasks.Count, result.GanttTasks.Count);
            Assert.Equal(expected.DeveloperAvailability.Count, result.DeveloperAvailability.Count);
            Assert.Equal(expected.GanttTasks[0].Name, result.GanttTasks[0].Name);
            Assert.Equal(expected.DeveloperAvailability[0].Name, result.DeveloperAvailability[0].Name);
            Assert.Equal(expected.DeveloperAvailability[0].DailyWorkHours, result.DeveloperAvailability[0].DailyWorkHours);
            Assert.Equal(JsonConvert.SerializeObject(expected.GanttTasks, Formatting.Indented), JsonConvert.SerializeObject(result.GanttTasks, Formatting.Indented));
        }

        [Fact]
        public void CalculateGanttChartAllocationNoDependencies()
        {
            // Arrange
            var input = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationInput>("CalculateGanttChartAllocationNoDependencies", "Input");
            var fixedDateTime = new DateTime(2025, 03, 20);
            _mockDateTimeProvider.Setup(m => m.Today).Returns(fixedDateTime);
            Assert.NotNull(input);
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            var expected = JsonTestHelper.ProcessMethodJson<CalculateGanttChartAllocationOutput>("CalculateGanttChartAllocationNoDependencies", "Output");
            Assert.Equal(expected.GanttTasks.Count, result.GanttTasks.Count);
            Assert.Equal(expected.DeveloperAvailability.Count, result.DeveloperAvailability.Count);
            Assert.Equal(expected.GanttTasks[0].Name, result.GanttTasks[0].Name);
            Assert.Equal(expected.DeveloperAvailability[0].Name, result.DeveloperAvailability[0].Name);
            Assert.Equal(expected.DeveloperAvailability[0].DailyWorkHours, result.DeveloperAvailability[0].DailyWorkHours);
            Assert.Equal(JsonConvert.SerializeObject(expected.GanttTasks, Formatting.Indented), JsonConvert.SerializeObject(result.GanttTasks, Formatting.Indented));
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
                    EstimatedEffortHours = 10,
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

            var developerDtoList = new List<DeveloperDto>
            {
                new DeveloperDto
                {
                    Team = "Team 1",
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
            // Act
            var result = _ganttService.CalculateGanttChartAllocation(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.GanttTasks);
            Assert.Single(result.DeveloperAvailability);

            var ganttTask = result.GanttTasks.First();
            Assert.Equal("1", ganttTask.Id);
            Assert.Equal("Project 1 : Task 1 (Team 1 : Dev 1)", ganttTask.Name);
            Assert.Equal(10, ganttTask.EstimatedEffortHours);

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
                    Team = "Team 1",
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
                    EstimatedEffortHours = 10,
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
