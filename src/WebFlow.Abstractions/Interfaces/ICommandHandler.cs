using CSharpFunctionalExtensions;
using ErrorsFlow.Models;

namespace WebFlow.Abstractions.Interfaces;

/// <summary>
/// Обработчик команды с полезным результатом.
/// </summary>
/// <typeparam name="TCommand">Тип обрабатываемой команды.</typeparam>
/// <typeparam name="TResponse">Тип успешного результата.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand
{
    /// <summary>Выполняет команду.</summary>
    Task<Result<TResponse, ErrorList>> Handle(
        TCommand command,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Обработчик команды без полезного результата.
/// </summary>
/// <typeparam name="TCommand">Тип обрабатываемой команды.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>Выполняет команду.</summary>
    Task<UnitResult<ErrorList>> Handle(
        TCommand command,
        CancellationToken cancellationToken = default);
}
