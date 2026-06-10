using FluentValidation;
using SpreadsheetUtility.Application.UseCases.ParseExcelData;

namespace SpreadsheetUtility.Application.Validation;

public class ParseExcelDataCommandValidator : AbstractValidator<ParseExcelDataCommand>
{
    public ParseExcelDataCommandValidator()
    {
        RuleFor(x => x.ProjectsData)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.TasksData) && string.IsNullOrEmpty(x.TeamData))
            .WithMessage("At least one of ProjectsData, TasksData, or TeamData must be provided.");

        RuleFor(x => x.TasksData)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.ProjectsData) && string.IsNullOrEmpty(x.TeamData))
            .WithMessage("At least one of ProjectsData, TasksData, or TeamData must be provided.");

        RuleFor(x => x.TeamData)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.ProjectsData) && string.IsNullOrEmpty(x.TasksData))
            .WithMessage("At least one of ProjectsData, TasksData, or TeamData must be provided.");
    }
}
