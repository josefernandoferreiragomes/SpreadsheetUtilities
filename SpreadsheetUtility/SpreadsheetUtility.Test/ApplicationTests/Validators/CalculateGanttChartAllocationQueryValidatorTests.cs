using FluentValidation.TestHelper;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.UseCases.CalculateGanttChartAllocation;
using SpreadsheetUtility.Application.Validation;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class CalculateGanttChartAllocationQueryValidatorTests
{
    private readonly CalculateGanttChartAllocationQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Input_Is_Null()
    {
        var query = new CalculateGanttChartAllocationQuery(null!);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Input);
    }

    [Fact]
    public void Should_Have_Error_When_TaskDtos_Is_Null()
    {
        var query = new CalculateGanttChartAllocationQuery(new CalculateGanttChartAllocationInput
        {
            TaskDtos = null!,
            DeveloperDtos = new List<DeveloperDto>(),
            ProjectDtos = new List<ProjectDto>()
        });
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor("Input.TaskDtos");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Input_Is_Valid()
    {
        var query = new CalculateGanttChartAllocationQuery(new CalculateGanttChartAllocationInput
        {
            TaskDtos = new List<TaskDto>(),
            DeveloperDtos = new List<DeveloperDto>(),
            ProjectDtos = new List<ProjectDto>()
        });
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
