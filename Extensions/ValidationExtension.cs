using System;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualBasic;

namespace WebFlow.Extensions;

public static class ValidationExtension
{
    public static ErrorList ToErrorList(this ValidationResult validationResult)
    {
        var validationErrors = validationResult.Errors;

        var errors = from validationError in validationErrors
            let errorMessage = validationError.ErrorMessage
            let error = Error.Deserialize(errorMessage)
            select GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(validationError.PropertyName)));

        return errors.ToList();
    }
    
    public static IRuleBuilderOptionsConditions<T, TElement>
        MustBeValueObject<T, TElement, TValueObject>(
            this IRuleBuilder<T, TElement> ruleBuilder,
            Func<TElement, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value);

            if (result.IsSuccess)
                return;

            context.AddFailure(result.Error.Serialize());
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
    {
        return rule.WithMessage(error.Serialize());
    }
    
    public static IRuleBuilderOptionsConditions<T, TProperty>
        MustBeEnum<T, TProperty, TEnum>(
            this IRuleBuilder<T, TProperty> ruleBuilder) where TEnum : Enum
    {
        return ruleBuilder.Custom((value, context) =>
        {
            if (!Enum.TryParse(typeof(TEnum), value!.ToString(), out var result))
            {
                context.AddFailure(GeneralErrors.ValueIsInvalid().Serialize());
            }
        });
    }
    
    public static bool IsEmail(this string email)
    {
        return Regex.IsMatch(email, Constants.PatternEmail);
    }
    
    public static bool IsValidPhoneNumber(this string phoneNumber)
    {
       
        return Regex.IsMatch(phoneNumber, Constants.PatternPhoneNumber);
    }
    
    public static bool IsOnlyLetters(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false; 

        return input.All(char.IsLetter);
    }
}