using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;
using WebFlow.Extensions;

namespace WebFlow.ValueObjects;

public class PhoneNumber : ComparableValueObject
{
    private PhoneNumber() {}

    private PhoneNumber(string phoneNumber)
    {
        Value = phoneNumber;
    }
    public string Value { get; }
    
    public static Result<PhoneNumber, Error> Create(string phoneNumber)
    {
        if (!phoneNumber.IsValidPhoneNumber())
        {
            return GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(PhoneNumber)));
        }
        
        return new PhoneNumber(phoneNumber);
    }
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}