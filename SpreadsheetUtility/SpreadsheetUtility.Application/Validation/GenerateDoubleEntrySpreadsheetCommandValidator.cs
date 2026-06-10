using FluentValidation;
using SpreadsheetUtility.Application.UseCases.GenerateDoubleEntrySpreadsheet;

namespace SpreadsheetUtility.Application.Validation;

public class GenerateDoubleEntrySpreadsheetCommandValidator : AbstractValidator<GenerateDoubleEntrySpreadsheetCommand>
{
    public GenerateDoubleEntrySpreadsheetCommandValidator()
    {
        RuleFor(x => x.InputFilePath)
            .NotEmpty().WithMessage("Input file path is required.");

        RuleFor(x => x.KeyColumnId)
            .NotEmpty().WithMessage("Key column ID is required.")
            .Must(BeValidColumnId).WithMessage("Key column ID must be a positive integer.");

        RuleFor(x => x.ValuesColumnId)
            .NotEmpty().WithMessage("Values column ID is required.")
            .Must(BeValidColumnId).WithMessage("Values column ID must be a positive integer.");

        RuleFor(x => x.OutputFilePath)
            .NotEmpty().WithMessage("Output file path is required.");
    }

    private static bool BeValidColumnId(string columnId)
    {
        return int.TryParse(columnId, out var id) && id > 0;
    }
}
