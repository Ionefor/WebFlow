using ErrorsFlow;
using ErrorsFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFlow.AspNetCore.Extensions;
using WebFlow.AspNetCore.Models;

namespace WebFlow.AspNetCore.Tests;

public sealed class ErrorResponseExtensionsTests
{
    [Theory]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Failure, StatusCodes.Status500InternalServerError)]
    [InlineData(ErrorType.InternalServer, StatusCodes.Status500InternalServerError)]
    public void ToActionResult_WhenSingleErrorType_ReturnsExpectedStatusCode(
        ErrorType type,
        int expectedStatusCode)
    {
        var error = ErrorFactory.Create("test.error", "Test error.", type);

        var result = error.ToActionResult();

        Assert.Equal(expectedStatusCode, result.StatusCode);
        var envelope = Assert.IsType<Envelope<object?>>(result.Value);
        Assert.Same(error, Assert.Single(envelope.Errors!));
    }

    [Fact]
    public void ToActionResult_WhenErrorListHasDifferentTypes_ReturnsInternalServerError()
    {
        var errors = new ErrorList([
            ErrorFactory.Create("validation.error", "Validation error.", ErrorType.Validation),
            ErrorFactory.Create("conflict.error", "Conflict error.", ErrorType.Conflict)
        ]);

        var result = errors.ToActionResult();

        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
    }
}
