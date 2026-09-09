using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;
using WebFlow.Extensions;

namespace WebFlow.ValueObjects;

public class Email : ComparableValueObject
{
    private Email() {}
    private Email(string email)
    {
        Value = email;
    }

    public string Value { get; }
    
    public static Result<Email, Error> Create(string email)
    {
        if (!email.IsEmail())
        {
            return GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(Email)));
        }
        
        return new Email(email);
    }
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}