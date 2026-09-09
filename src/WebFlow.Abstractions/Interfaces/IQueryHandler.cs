using CSharpFunctionalExtensions;
using ErrorsFlow.Models;

namespace WebFlow.Abstractions.Interfaces;

/// <summary>
/// Обработчик запроса.
/// </summary>
/// <typeparam name="TQuery">Тип обрабатываемого запроса.</typeparam>
/// <typeparam name="TResponse">Тип успешного результата.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery
{
    /// <summary>Выполняет запрос.</summary>
    Task<Result<TResponse, ErrorList>> Handle(
        TQuery query,
        CancellationToken cancellationToken = default);
}
