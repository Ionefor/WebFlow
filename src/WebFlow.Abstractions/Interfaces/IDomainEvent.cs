namespace WebFlow.Abstractions.Interfaces;

/// <summary>
/// Внутреннее событие предметной области, возникшее в пределах одного модуля.
/// </summary>
/// <remarks>
/// Не является контрактом RabbitMQ и не предназначено для потребления другими модулями.
/// </remarks>
public interface IDomainEvent
{
    /// <summary>
    /// Уникальный идентификатор экземпляра события.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Момент возникновения события в UTC.
    /// </summary>
    DateTimeOffset OccurredAt { get; }
}
