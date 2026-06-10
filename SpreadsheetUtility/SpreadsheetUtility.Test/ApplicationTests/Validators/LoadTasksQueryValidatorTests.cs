using FluentValidation.TestHelper;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.UseCases.LoadTasks;
using SpreadsheetUtility.Application.Validation;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class LoadTasksQueryValidatorTests
{
    private readonly LoadTasksQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_TaskDtos_Is_Null()
    {
        var query = new LoadTasksQuery(null!);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.TaskDtos);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TaskDtos_Is_Provided()
    {
        var query = new LoadTasksQuery(new List<TaskDto>());
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
