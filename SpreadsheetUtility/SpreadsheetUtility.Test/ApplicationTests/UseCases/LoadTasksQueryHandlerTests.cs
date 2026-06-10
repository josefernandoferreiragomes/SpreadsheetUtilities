using Moq;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.Mappers;
using SpreadsheetUtility.Application.UseCases.LoadTasks;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class LoadTasksQueryHandlerTests
{
    private readonly Mock<IGanttChartMapper> _mapperMock;
    private readonly LoadTasksQueryHandler _handler;

    public LoadTasksQueryHandlerTests()
    {
        _mapperMock = new Mock<IGanttChartMapper>();
        _handler = new LoadTasksQueryHandler(_mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_MapGanttTasksFromTaskDtos()
    {
        var taskDtos = new List<TaskDto>
        {
            new() { Id = "1", ProjectID = "P1", TaskName = "Task 1", EffortHours = 10 }
        };
        var expectedTasks = new List<GanttTask>
        {
            new() { Id = "1", TaskName = "Task 1", EffortHours = 10 }
        };

        _mapperMock.Setup(m => m.MapGanttTasksFromTaskDtos(taskDtos)).Returns(expectedTasks);

        var result = await _handler.Handle(new LoadTasksQuery(taskDtos), CancellationToken.None);

        Assert.Equal(expectedTasks, result);
        _mapperMock.Verify(m => m.MapGanttTasksFromTaskDtos(taskDtos), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Tasks()
    {
        var taskDtos = new List<TaskDto>();
        _mapperMock.Setup(m => m.MapGanttTasksFromTaskDtos(taskDtos)).Returns(new List<GanttTask>());

        var result = await _handler.Handle(new LoadTasksQuery(taskDtos), CancellationToken.None);

        Assert.Empty(result);
    }
}
