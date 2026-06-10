using FluentValidation;
using SpreadsheetUtility.Application.UseCases.LoadTasks;

namespace SpreadsheetUtility.Application.Validation;

public class LoadTasksQueryValidator : AbstractValidator<LoadTasksQuery>
{
    public LoadTasksQueryValidator()
    {
        RuleFor(x => x.TaskDtos)
            .NotNull().WithMessage("TaskDtos must not be null.");
    }
}
