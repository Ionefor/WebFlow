using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;
using WebFlow.Extensions;

namespace WebFlow.ValueObjects;

public class DateOfBirth : ComparableValueObject
{
    private DateOfBirth() {}
    
    private DateOfBirth(DateOnly date)
    {
        Value = date;
    }

    public DateOnly Value { get; }
    
    public static Result<DateOfBirth, Error> Create(DateOnly date)
    {
        if (date.IsDateOfBirth())
        {
            return GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(DateOfBirth)));
        }
        
        return new DateOfBirth(date);
    }
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}