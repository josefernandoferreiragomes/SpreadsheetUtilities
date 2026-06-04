using FluentValidation.TestHelper;
using SpreadsheetUtility.Application.UseCases.ParseExcelData;
using SpreadsheetUtility.Application.Validation;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class ParseExcelDataCommandValidatorTests
{
    private readonly ParseExcelDataCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_All_Data_Fields_Are_Empty()
    {
        var command = new ParseExcelDataCommand
        {
            ProjectsData = null,
            TasksData = null,
            TeamData = null
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProjectsData_Is_Provided()
    {
        var command = new ParseExcelDataCommand
        {
            ProjectsData = "ProjectID\tProjectName\nP1\tProj1",
            TasksData = null,
            TeamData = null
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_TasksData_Is_Provided()
    {
        var command = new ParseExcelDataCommand
        {
            ProjectsData = null,
            TasksData = "Id\tProjectID\tTaskName\n1\tP1\tTask1",
            TeamData = null
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_TeamData_Is_Provided()
    {
        var command = new ParseExcelDataCommand
        {
            ProjectsData = null,
            TasksData = null,
            TeamData = "TeamId\tTeam\tDeveloperId\n1\tAlpha\tD1"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
