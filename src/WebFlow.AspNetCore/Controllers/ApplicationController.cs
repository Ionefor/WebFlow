using ErrorsFlow.Models;
using Microsoft.AspNetCore.Mvc;
using WebFlow.AspNetCore.Extensions;
using WebFlow.AspNetCore.Models;

namespace WebFlow.AspNetCore.Controllers;

/// <summary>
/// Базовый контроллер с единым форматом ответов WebFlow.
/// </summary>
[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : ControllerBase
{
    /// <summary>Возвращает успешный ответ в формате <see cref="Envelope{T}"/>.</summary>
    protected OkObjectResult OkEnvelope<T>(T response) => Ok(Envelope<T>.Success(response));

    /// <summary>
    /// Возвращает ответ <c>201 Created</c> в формате <see cref="Envelope{T}"/> без заголовка <c>Location</c>.
    /// </summary>
    protected CreatedResult CreatedEnvelope<T>(T response) =>
        Created((string?)null, Envelope<T>.Success(response));

    /// <summary>
    /// Возвращает ответ <c>201 Created</c> в формате <see cref="Envelope{T}"/>.
    /// </summary>
    protected CreatedAtActionResult CreatedAtActionEnvelope<T>(
        string actionName,
        object? routeValues,
        T response) =>
        CreatedAtAction(actionName, routeValues, Envelope<T>.Success(response));

    /// <summary>
    /// Возвращает ответ <c>202 Accepted</c> в формате <see cref="Envelope{T}"/>.
    /// </summary>
    protected AcceptedResult AcceptedEnvelope<T>(T response) => Accepted(Envelope<T>.Success(response));

    /// <summary>Преобразует ошибки домена или приложения в HTTP-ответ.</summary>
    protected ObjectResult Error(ErrorList errors) => errors.ToActionResult();
}
