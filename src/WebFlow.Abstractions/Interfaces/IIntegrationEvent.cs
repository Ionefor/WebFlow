namespace WebFlow.Abstractions.Interfaces;

/// <summary>
/// Публичное событие для обмена данными между модулями или внешними системами.
/// </summary>
/// <remarks>
/// Контракт не зависит от конкретного транспорта. Его можно передать через RabbitMQ,
/// сохранить в Outbox или опубликовать иным способом.
/// </remarks>
public interface IIntegrationEvent
{
    /// <summary>
    /// Уникальный идентификатор сообщения, используемый для дедупликации потребителями.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Момент возникновения бизнес-события в UTC.
    /// </summary>
    DateTimeOffset OccurredAt { get; }
}
