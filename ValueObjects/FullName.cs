using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;

namespace WebFlow.ValueObjects;

public class FullName : ComparableValueObject
{
    private FullName() {}
    private FullName(string firstName, string middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }
    public string FirstName { get; }
    public string MiddleName { get; }
    public string LastName { get; }

    public static Result<FullName, Error> Create(
        string firstName, string middleName, string lastName)
    {
        var firstNameResult = Name.Create(firstName);
        var middleNameResult = Name.Create(middleName);
        var lastNameResult = Name.Create(lastName);
        
        if (firstNameResult.IsFailure ||
            middleNameResult.IsFailure
            || lastNameResult.IsFailure)
        {
            return GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(FullName)));
        }
   
        return new FullName(firstName, middleName, lastName);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return FirstName;
        yield return MiddleName;
        yield return LastName;
    }
}