using System.Data;

namespace WebFlow.Abstractions.Interfaces;

/// <summary>
/// Контракт сохранения изменений и управления транзакциями.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Начинает транзакцию с уровнем изоляции, заданным поставщиком базы данных по умолчанию.
    /// </summary>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Начинает транзакцию с указанным уровнем изоляции.
    /// </summary>
    /// <param name="isolationLevel">Уровень изоляции, поддерживаемый используемой базой данных.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<ITransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет накопленные изменения.
    /// Если ранее была открыта явная транзакция, вызов не фиксирует её: для этого нужен
    /// <see cref="ITransaction.CommitAsync(CancellationToken)"/>.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
