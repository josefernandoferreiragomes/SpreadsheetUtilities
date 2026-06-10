using FluentValidation.TestHelper;
using SpreadsheetUtility.Application.UseCases.GenerateDoubleEntrySpreadsheet;
using SpreadsheetUtility.Application.Validation;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class GenerateDoubleEntrySpreadsheetCommandValidatorTests
{
    private readonly GenerateDoubleEntrySpreadsheetCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_InputFilePath_Is_Empty()
    {
        var command = new GenerateDoubleEntrySpreadsheetCommand
        {
            InputFilePath = "",
            KeyColumnId = "1",
            ValuesColumnId = "2",
            OutputFilePath = "output.xlsx"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.InputFilePath);
    }

    [Fact]
    public void Should_Have_Error_When_KeyColumnId_Is_Not_A_Positive_Integer()
    {
        var command = new GenerateDoubleEntrySpreadsheetCommand
        {
            InputFilePath = "input.xlsx",
            KeyColumnId = "abc",
            ValuesColumnId = "2",
            OutputFilePath = "output.xlsx"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.KeyColumnId);
    }

    [Fact]
    public void Should_Have_Error_When_ValuesColumnId_Is_Not_A_Positive_Integer()
    {
        var command = new GenerateDoubleEntrySpreadsheetCommand
        {
            InputFilePath = "input.xlsx",
            KeyColumnId = "1",
            ValuesColumnId = "0",
            OutputFilePath = "output.xlsx"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValuesColumnId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new GenerateDoubleEntrySpreadsheetCommand
        {
            InputFilePath = "input.xlsx",
            KeyColumnId = "1",
            ValuesColumnId = "2",
            OutputFilePath = "output.xlsx"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
