using FluentValidation.TestHelper;
using SpreadsheetUtility.Application.UseCases.Session;
using SpreadsheetUtility.Application.Validation;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class UpdateSessionCommandValidatorTests
{
    private readonly UpdateSessionCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new UpdateSessionCommand("", Guid.NewGuid(), "new value");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_NewValue_Is_Empty()
    {
        var command = new UpdateSessionCommand("user@example.com", Guid.NewGuid(), "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NewValue);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new UpdateSessionCommand("user@example.com", Guid.NewGuid(), "new value");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
