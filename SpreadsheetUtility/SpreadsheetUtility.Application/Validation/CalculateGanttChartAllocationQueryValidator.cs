using FluentValidation;
using SpreadsheetUtility.Application.UseCases.CalculateGanttChartAllocation;

namespace SpreadsheetUtility.Application.Validation;

public class CalculateGanttChartAllocationQueryValidator : AbstractValidator<CalculateGanttChartAllocationQuery>
{
    public CalculateGanttChartAllocationQueryValidator()
    {
        RuleFor(x => x.Input)
            .NotNull().WithMessage("Input must not be null.");

        When(x => x.Input != null, () =>
        {
            RuleFor(x => x.Input.TaskDtos)
                .NotNull().WithMessage("TaskDtos must not be null.");

            RuleFor(x => x.Input.DeveloperDtos)
                .NotNull().WithMessage("DeveloperDtos must not be null.");

            RuleFor(x => x.Input.ProjectDtos)
                .NotNull().WithMessage("ProjectDtos must not be null.");
        });
    }
}
