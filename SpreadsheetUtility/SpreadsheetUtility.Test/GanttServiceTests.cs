using SpreadsheetUtility.Library;
using SpreadsheetUtility.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Test
{
   
    public class GanttServiceTests
    {
        private readonly GanttService _ganttService;

        public GanttServiceTests()
        {
            var ganttProcessor = new GanttChartProcessor();
            _ganttService = new GanttService(ganttProcessor);
        }

        [Fact]
        public void CalculateGanttChartAllocationFromDtos_ShouldReturnCorrectAllocation_WhenValidDataProvided()
        {
            // Arrange
            var taskDtos = new List<TaskDto>
            {
                new TaskDto
                {
                    Id = "1",
                    ProjectID = "P1",
                    ProjectName = "Project 1",
                    TaskName = "Task 1",
                    EstimatedEffortHours = 10,
                    Dependencies = "",
                    ProjectDependency = "",
                    Progress = "0"
                }
            };

            var developerDtos = new List<DeveloperDto>
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

            // Act
            var result = _ganttService.CalculateGanttChartAllocationFromDtos(taskDtos, developerDtos, preSortTasks);

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
            var taskDtos = new List<TaskDto>();
            var developerDtos = new List<DeveloperDto>
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

            // Act
            var result = _ganttService.CalculateGanttChartAllocationFromDtos(taskDtos, developerDtos, preSortTasks);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.GanttTasks);
            Assert.Single(result.DeveloperAvailability);
        }

        [Fact]
        public void CalculateGanttChartAllocationFromDtos_ShouldHandleEmptyDeveloperList()
        {
            // Arrange
            var taskDtos = new List<TaskDto>
            {
                new TaskDto
                {
                    Id = "1",
                    ProjectID = "P1",
                    ProjectName = "Project 1",
                    TaskName = "Task 1",
                    EstimatedEffortHours = 10,
                    Dependencies = "",
                    ProjectDependency = "",
                    Progress = "0"
                }
            };

            var developerDtos = new List<DeveloperDto>();

            bool preSortTasks = true;

            // Act
            var result = _ganttService.CalculateGanttChartAllocationFromDtos(taskDtos, developerDtos, preSortTasks);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.GanttTasks);
            Assert.Empty(result.DeveloperAvailability);
        }
    }

}
