using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ErrorsFlow.Models;
using Microsoft.AspNetCore.Mvc;
using WebFlow.Models;

namespace WebFlow.Controller;

public partial class ApplicationController
{
    protected async Task<ActionResult> HandleQuery<TRequest, TQuery, TResponse, TKResponse>(
        TRequest request,
        Func<TRequest, TQuery> createQuery,
        Func<TQuery, CancellationToken, Task<PageList<(TResponse, TKResponse)>>> handleQuery,
        CancellationToken cancellationToken)
    {
        var query = createQuery(request);
        
        var result = await handleQuery(query, cancellationToken);
        
        return Ok(result);
    }
    
    protected async Task<ActionResult> HandleQuery
        <TRequest, TQuery, TXResponse, TYResponse, TZResponse>
        (TRequest request,
        Func<TRequest, TQuery> createQuery,
        Func<TQuery, CancellationToken, Task<PageList<(TXResponse, TYResponse, TZResponse)>>> handleQuery,
        CancellationToken cancellationToken)
    {
        var query = createQuery(request);
        
        var result = await handleQuery(query, cancellationToken);
        
        return Ok(result);
    }
    
    protected async Task<ActionResult> HandleQuery<TRequest, TQuery, TResponse>(
        TRequest request,
        Func<TRequest, TQuery> createQuery,
        Func<TQuery, CancellationToken, Task<PageList<TResponse>>> handleQuery,
        CancellationToken cancellationToken)
    {
        var query = createQuery(request);
        
        var result = await handleQuery(query, cancellationToken);
        
        return Ok(result);
    }
    protected async Task<ActionResult<TResponse>> HandleQuery<TRequest, TQuery, TResponse>(
        TRequest request,
        Func<TRequest, TQuery> createQuery,
        Func<TQuery, CancellationToken, Task<Result<TResponse, ErrorList>>> handleQuery,
        Func<ErrorList, ObjectResult> createErrorResult,
        CancellationToken cancellationToken)
    {
        var query = createQuery(request);
        var result = await handleQuery(query, cancellationToken);
        
        if (result.IsFailure)
            return createErrorResult(result.Error);

        return Ok(result.Value);
    }
    
    protected async Task<ActionResult<TResponse>> HandleQuery<TQuery, TResponse>(
        Guid userId,
        Func<Guid, TQuery> createQuery,
        Func<TQuery, CancellationToken, Task<Result<TResponse, ErrorList>>> handleQuery,
        Func<ErrorList, ObjectResult> createErrorResult,
        CancellationToken cancellationToken)
    {
        var query = createQuery!(userId);
        var result = await handleQuery(query, cancellationToken);
        
        if (result.IsFailure)
            return createErrorResult(result.Error);
        
        return Ok(result.Value);
    }
    
    protected async Task<ActionResult<TResponse>> HandleQuery<TQuery, TResponse>(
        Guid firstId,
        Guid secondId,
        Func<Guid, Guid, TQuery> createQuery,
        Func<TQuery, CancellationToken, Task<Result<TResponse, ErrorList>>> handleQuery,
        Func<ErrorList, ObjectResult> createErrorResult,
        CancellationToken cancellationToken)
    {
        var query = createQuery(firstId, secondId);
        var result = await handleQuery(query, cancellationToken);
        
        if (result.IsFailure)
            return createErrorResult(result.Error);
        
        return Ok(result.Value);
    }
}