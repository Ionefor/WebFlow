using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebFlow.Models;

namespace WebFlow.Extensions;

public static class QueriesExtensions
{
    public static async Task<PageList<T>> ToPagedList<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await source.
            CountAsync(cancellationToken);
        
        var items = await source.
            Skip((page - 1) * pageSize).
            Take(pageSize).
            ToListAsync(cancellationToken);

        return new PageList<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public static PageList<T> ToPagedList<T>(
        this IEnumerable<T> source,
        int page,
        int pageSize)
    {
        var totalCount = source.Count();
        
        var items = source.
            Skip((page - 1) * pageSize).
            Take(pageSize).ToList();

        return new PageList<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public static async Task<PageList<(T1, T2)>> ToPagedList<T1, T2>(
        this (IQueryable<T1>, IQueryable<T2>) source,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var firstCount = await source.
            Item1.
            CountAsync(cancellationToken);
        
        var secondCount = await source.
            Item2.
            CountAsync(cancellationToken);
        
        var totalCount = firstCount + secondCount;
        
        var firstItems = await source.Item1.
            Skip((page - 1) * pageSize).
            Take(pageSize).
            ToListAsync(cancellationToken);
        
        var secondItems = await source.Item2.
            Skip((page - 1) * pageSize).
            Take(pageSize).
            ToListAsync(cancellationToken);
        
        var combinedItems = firstItems.
            Zip(secondItems, (item1, item2) => (item1, item2)).
            ToList();
        
        return new PageList<(T1, T2)>
        {
            Items = combinedItems,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    public static PageList<(T1, T2, T3)> ToPagedList<T1, T2, T3>(
        this (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) source,
        int page,
        int pageSize)
    {
        var firstCount = source.
            Item1.
            Count();
        
        var secondCount = source.
            Item2.
            Count();
        
        var thirdCount = source.
            Item3.
            Count();
        
        var totalCount = firstCount + secondCount + thirdCount;
        
        var firstItems = source.Item1
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var secondItems = source.Item2
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var thirdItems = source.Item3
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var combinedItems = firstItems
            .Zip(secondItems, (item1, item2) => (item1, item2))
            .Zip(thirdItems, (tuple, item3) => (tuple.item1, tuple.item2, item3))
            .ToList();

        return new PageList<(T1, T2, T3)>
        {
            Items = combinedItems,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
    
    public static IQueryable<TSource> SortIf<TSource, TKey>(
        this IQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, TKey>> selector)
    {
        if (!condition)
            return source;
        
        if (source is IOrderedQueryable<TSource> ordered)
            return ordered.ThenBy(selector);

        return source.OrderBy(selector);
    }
}