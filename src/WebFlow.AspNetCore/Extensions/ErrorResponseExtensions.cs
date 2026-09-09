using ErrorsFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFlow.AspNetCore.Models;

namespace WebFlow.AspNetCore.Extensions;

/// <summary>
/// Преобразования ошибок ErrorsFlow в стандартизированные HTTP-ответы.
/// </summary>
public static class ErrorResponseExtensions
{
    /// <summary>Преобразует одну ошибку в HTTP-ответ.</summary>
    public static ObjectResult ToActionResult(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return error.ToErrorList().ToActionResult();
    }

    /// <summary>Преобразует список ошибок в HTTP-ответ.</summary>
    public static ObjectResult ToActionResult(this ErrorList errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorTypes = errors.Select(error => error.Type).Distinct().ToArray();
        var statusCode = errorTypes.Length == 1
            ? GetStatusCode(errorTypes[0])
            : StatusCodes.Status500InternalServerError;

        return new ObjectResult(Envelope<object?>.Failure(errors))
        {
            StatusCode = statusCode
        };
    }

    private static int GetStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Failure or ErrorType.InternalServer => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}
