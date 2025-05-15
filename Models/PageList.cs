using System.Collections.Generic;

namespace WebFlow.Models;

public class PageList<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    
    public int TotalCount { get; init; }
    
    public int PageSize { get; init; }
    
    public int Page { get; init; }
    
    public bool HasPreviousPage => Page > 1;
    
    public bool HasNextPage => Page * PageSize < TotalCount;
}