namespace WebFlow.Abstractions.Interfaces;

/// <summary>
/// Абстракция транзакции, не зависящая от конкретной ORM или провайдера БД.
/// </summary>
/// <remarks>
/// Реализация должна откатить транзакцию при освобождении через <c>DisposeAsync</c>,
/// если она не была завершена через <see cref="CommitAsync(CancellationToken)"/>
/// или <see cref="RollbackAsync(CancellationToken)"/>.
/// </remarks>
public interface ITransaction : IAsyncDisposable
{
    /// <summary>Фиксирует транзакцию.</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>Откатывает транзакцию.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
