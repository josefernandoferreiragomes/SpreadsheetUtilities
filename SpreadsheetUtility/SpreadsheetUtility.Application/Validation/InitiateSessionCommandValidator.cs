using FluentValidation;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Application.Validation;

public class InitiateSessionCommandValidator : AbstractValidator<InitiateSessionCommand>
{
    public InitiateSessionCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
