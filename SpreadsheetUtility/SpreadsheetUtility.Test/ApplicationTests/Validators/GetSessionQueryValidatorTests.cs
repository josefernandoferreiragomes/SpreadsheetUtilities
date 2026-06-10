using FluentValidation.TestHelper;
using SpreadsheetUtility.Application.UseCases.Session;
using SpreadsheetUtility.Application.Validation;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class GetSessionQueryValidatorTests
{
    private readonly GetSessionQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var query = new GetSessionQuery("", Guid.NewGuid());
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var query = new GetSessionQuery("bad-email", Guid.NewGuid());
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Query_Is_Valid()
    {
        var query = new GetSessionQuery("user@example.com", Guid.NewGuid());
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
