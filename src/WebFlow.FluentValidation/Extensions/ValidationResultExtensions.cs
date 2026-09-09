using ErrorsFlow;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using FluentValidation.Results;

namespace WebFlow.FluentValidation.Extensions;

/// <summary>
/// Преобразования результатов FluentValidation в ошибки ErrorsFlow.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Преобразует неуспешный результат в непустой список ошибок валидации.
    /// </summary>
    /// <param name="validationResult">Результат FluentValidation с ошибками.</param>
    /// <returns>Список ошибок ErrorsFlow с полем, кодом и сообщением валидатора.</returns>
    /// <exception cref="ArgumentException">Возникает, если валидация прошла успешно.</exception>
    public static ErrorList ToErrorList(this ValidationResult validationResult)
    {
        ArgumentNullException.ThrowIfNull(validationResult);

        if (validationResult.IsValid)
        {
            throw new ArgumentException(
                "Only a failed validation result can be converted to an error list.",
                nameof(validationResult));
        }

        return new ErrorList(validationResult.Errors.Select(error =>
            ErrorFactory.Create(
                string.IsNullOrWhiteSpace(error.ErrorCode)
                    ? GeneralErrorCodes.ValueIsInvalid
                    : error.ErrorCode,
                error.ErrorMessage,
                ErrorType.Validation,
                error.PropertyName)));
    }
}
