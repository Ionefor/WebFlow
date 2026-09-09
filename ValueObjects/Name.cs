using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;
using WebFlow.Extensions;

namespace WebFlow.ValueObjects;

public class Name : ComparableValueObject
{
    private Name() {}

    private Name(string value)
    {
        Value = value;
    }
    
    public string Value { get; }
    
    public static Result<Name, Error> Create(string value)
    {
        if (!value.IsName())
        {
            return GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(Name)));
        }
        
        return new Name(value);
    }
    
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}