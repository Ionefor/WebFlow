using FluentValidation.Results;
using WebFlow.FluentValidation.Extensions;

namespace WebFlow.FluentValidation.Tests;

public sealed class ValidationResultExtensionsTests
{
    [Fact]
    public void ToErrorList_WhenValidationFails_PreservesValidatorDetails()
    {
        var validationResult = new ValidationResult([
            new ValidationFailure("Email", "Email has an invalid format.")
            {
                ErrorCode = "users.email.invalid"
            }
        ]);

        var errors = validationResult.ToErrorList();

        var error = Assert.Single(errors);
        Assert.Equal("users.email.invalid", error.Code);
        Assert.Equal("Email has an invalid format.", error.Message);
        Assert.Equal("Email", error.Target);
        Assert.Equal(ErrorsFlow.Models.ErrorType.Validation, error.Type);
    }

    [Fact]
    public void ToErrorList_WhenValidationSucceeds_Throws()
    {
        var validationResult = new ValidationResult();

        Assert.Throws<ArgumentException>(() => validationResult.ToErrorList());
    }
}
