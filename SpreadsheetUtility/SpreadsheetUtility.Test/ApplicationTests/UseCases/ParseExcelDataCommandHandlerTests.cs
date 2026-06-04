using SpreadsheetUtility.Application.UseCases.ParseExcelData;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class ParseExcelDataCommandHandlerTests
{
    private readonly ParseExcelDataCommandHandler _handler = new();

    [Fact]
    public async Task Handle_Should_Parse_ProjectsData()
    {
        var command = new ParseExcelDataCommand
        {
            ProjectsData = "ProjectID\tProjectName\tGroup\tTeamId\nP1\tProject1\tG1\tT1\nP2\tProject2\tG2\tT2"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.ProjectDtos.Count);
        Assert.Equal("P1", result.ProjectDtos[0].ProjectID);
        Assert.Equal("Project1", result.ProjectDtos[0].ProjectName);
    }

    [Fact]
    public async Task Handle_Should_Parse_TasksData()
    {
        var command = new ParseExcelDataCommand
        {
            TasksData = "Id\tProjectID\tProjectName\tTaskName\tEffortHours\tDependencies\tProgress\tInternalID\n1\tP1\tProj1\tTask1\t10\t\t0\tINT1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.TaskDtos);
        Assert.Equal("1", result.TaskDtos[0].Id);
        Assert.Equal("Task1", result.TaskDtos[0].TaskName);
        Assert.Equal(10, result.TaskDtos[0].EffortHours);
    }

    [Fact]
    public async Task Handle_Should_Parse_TeamData()
    {
        var command = new ParseExcelDataCommand
        {
            TeamData = "TeamId\tTeam\tDeveloperId\tName\tVacationPeriods\tDailyWorkHours\nT1\tAlpha\tD1\tDev1\t\t8"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.DeveloperDtos);
        Assert.Equal("D1", result.DeveloperDtos[0].DeveloperId);
        Assert.Equal("Dev1", result.DeveloperDtos[0].Name);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_When_No_Data()
    {
        var command = new ParseExcelDataCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.ProjectDtos);
        Assert.Empty(result.TaskDtos);
        Assert.Empty(result.DeveloperDtos);
    }
}
