using ErrorsFlow.Models;

namespace WebFlow.AspNetCore.Models;

/// <summary>
/// Единый HTTP-контейнер для успешного результата или списка ошибок.
/// </summary>
/// <typeparam name="T">Тип полезной нагрузки успешного ответа.</typeparam>
public sealed record Envelope<T>
{
    private Envelope(T? data, ErrorList? errors)
    {
        Data = data;
        Errors = errors;
    }

    /// <summary>Полезная нагрузка успешного ответа.</summary>
    public T? Data { get; }

    /// <summary>Список ошибок неуспешного ответа.</summary>
    public ErrorList? Errors { get; }

    /// <summary>Создаёт успешный ответ.</summary>
    public static Envelope<T> Success(T? data) => new(data, null);

    /// <summary>Создаёт неуспешный ответ.</summary>
    public static Envelope<T> Failure(ErrorList errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new Envelope<T>(default, errors);
    }
}
