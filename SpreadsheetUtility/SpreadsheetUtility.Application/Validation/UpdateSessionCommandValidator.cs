using FluentValidation;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Application.Validation;

public class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.NewValue)
            .NotEmpty().WithMessage("New value is required.");
    }
}
