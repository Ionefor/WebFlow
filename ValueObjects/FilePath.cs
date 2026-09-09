using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ErrorsFlow.Errors;
using ErrorsFlow.Models;
using ErrorsFlow.Parameters;

namespace WebFlow.ValueObjects;

public class FilePath : ComparableValueObject
{
    private FilePath() {}

    private FilePath(string path)
    {
        Value = path;
    }
    public string Value { get; }
    
    public static FilePath Create()
    {
        var path = Guid.NewGuid();
        var extension = System.IO.Path.GetExtension(path.ToString());
        
        var fullPath = path + extension;

        return new FilePath(fullPath);
    }
    
    public static Result<FilePath, Error> Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return GeneralErrors.ValueIsInvalid(
                new ErrorParameters.ValueIsInvalid(nameof(FilePath)));
        }

        var extension = System.IO.Path.GetExtension(path);

        if (!path.Contains(extension))
        {
            path += extension;
        }
        
        return new FilePath(path);
    }
    
    public static FilePath ToFilePath(string filePath) =>
        new(filePath);
    
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}