# WebFlow

WebFlow — набор небольших библиотек для .NET 10-приложений с разделением на Domain, Application, Infrastructure и Presentation. Он задаёт минимальные контракты команд, запросов и событий, а также предоставляет интеграцию с `ErrorsFlow`, FluentValidation и ASP.NET Core.

Библиотека не навязывает ORM, брокер сообщений, DI-контейнер, CQRS-фреймворк или архитектуру модулей. Она содержит только общие контракты и адаптеры на границе HTTP.

## Цели

- Явные контракты команд, запросов и их обработчиков.
- Единый `Result`-подход на основе `CSharpFunctionalExtensions` и `ErrorsFlow`.
- Стандартизированные HTTP-ответы без повторения маппинга ошибок в контроллерах.
- Разделение доменных и интеграционных событий.
- Никаких зависимостей от EF Core, Npgsql, RabbitMQ или ASP.NET Core в базовом пакете.

## Пакеты

| Пакет | Назначение | Зависимости |
| --- | --- | --- |
| `WebFlow.Abstractions` | Команды, запросы, обработчики, события, Unit of Work и транзакции. | `CSharpFunctionalExtensions`, `ErrorsFlow` |
| `WebFlow.AspNetCore` | HTTP-envelope, базовый контроллер и маппинг `ErrorsFlow` в HTTP-ответы. | `ErrorsFlow`, ASP.NET Core |
| `WebFlow.FluentValidation` | Преобразование ошибок FluentValidation в `ErrorList`. | `ErrorsFlow`, FluentValidation |

## Установка

Подключайте только те пакеты, которые требуются конкретному слою.

```xml
<!-- Application -->
<PackageReference Include="WebFlow.Abstractions" Version="2.0.0" />

<!-- Presentation / ASP.NET Core -->
<PackageReference Include="WebFlow.AspNetCore" Version="2.0.0" />

<!-- Application, если используется FluentValidation -->
<PackageReference Include="WebFlow.FluentValidation" Version="2.0.0" />
```

Все пакеты рассчитаны на .NET 10. Для проектов на .NET 8 остаётся предыдущий монолитный пакет `WebApplicationFlow` версии 1.x.

## Рекомендуемое размещение зависимостей

```text
Domain          → не зависит от WebFlow
Application     → WebFlow.Abstractions, WebFlow.FluentValidation
Infrastructure  → Application, конкретные реализации БД и транспорта
Presentation    → Application, WebFlow.AspNetCore
Messaging       → WebFlow.Abstractions (только для IIntegrationEvent, если нужен общий контракт)
```

`WebFlow.AspNetCore` не должен использоваться в Domain или Application. Домен также не должен зависеть от ASP.NET Core, EF Core и RabbitMQ.

## Команды и запросы

Команда меняет состояние приложения, запрос читает его.

```csharp
using WebFlow.Abstractions.Interfaces;

public sealed record RegisterClientCommand(
    string UserName,
    string Email,
    string Password) : ICommand;

public sealed record GetUserQuery(Guid UserId) : IQuery;
```

Обработчик команды с полезным результатом возвращает `Result<TResponse, ErrorList>`:

```csharp
using CSharpFunctionalExtensions;
using ErrorsFlow.Models;
using WebFlow.Abstractions.Interfaces;

public sealed class RegisterClientHandler
    : ICommandHandler<RegisterClientCommand, Guid>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        RegisterClientCommand command,
        CancellationToken cancellationToken = default)
    {
        // Валидация, создание агрегата и сохранение.
        return await Task.FromResult(Result.Success<Guid, ErrorList>(Guid.NewGuid()));
    }
}
```

Для команды без полезного результата используйте `ICommandHandler<TCommand>` с `UnitResult<ErrorList>`. Запросы реализуют `IQueryHandler<TQuery, TResponse>` и также возвращают `Result`.

## Ошибки и Result

WebFlow использует `ErrorList` из `ErrorsFlow` как единый тип ошибок обработчиков. Обработчик возвращает ожидаемые ошибки через `Result`, а не выбрасывает исключения.

```csharp
if (user is null)
    return GeneralErrors.NotFound("User", nameof(command.UserId)).ToErrorList();
```

Исключения оставляйте для непредвиденных технических сбоев. Их обработку и журналирование следует выполнять централизованно в Presentation или middleware приложения.

## FluentValidation

`WebFlow.FluentValidation` добавляет `ToErrorList()` для неуспешного `ValidationResult`. У каждой ошибки сохраняются код, сообщение и имя поля из `ValidationFailure`.

```csharp
var validationResult = await validator.ValidateAsync(command, cancellationToken);

if (!validationResult.IsValid)
    return validationResult.ToErrorList();
```

Вызывайте `ToErrorList()` только при `IsValid == false`: при успешной валидации метод намеренно выбрасывает `ArgumentException`, поскольку `ErrorList` не может быть пустым.

## ASP.NET Core

`ApplicationController` даёт готовые методы для успешных и ошибочных ответов. Контроллер самостоятельно выбирает семантически верный успешный статус: например, `201 Created` для создания и `200 OK` для чтения или изменения.

```csharp
using Microsoft.AspNetCore.Mvc;
using WebFlow.AspNetCore.Controllers;

[Route("api/auth")]
public sealed class AuthController : ApplicationController
{
    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient(
        RegisterClientRequest request,
        RegisterClientHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request.ToCommand(), cancellationToken);

        if (result.IsFailure)
            return Error(result.Error);

        return CreatedAtActionEnvelope(
            nameof(RegisterClient),
            new { id = result.Value },
            result.Value);
    }
}
```

Успешный ответ имеет форму:

```json
{
  "data": "полезный результат",
  "errors": null
}
```

Ошибочный ответ:

```json
{
  "data": null,
  "errors": [
    {
      "code": "value.is.invalid",
      "message": "The value is invalid.",
      "type": "Validation",
      "target": "Email"
    }
  ]
}
```

### Соответствие ошибок HTTP-статусам

| `ErrorType` | HTTP-статус |
| --- | --- |
| `Validation` | 400 Bad Request |
| `Unauthorized` | 401 Unauthorized |
| `Forbidden` | 403 Forbidden |
| `NotFound` | 404 Not Found |
| `Conflict` | 409 Conflict |
| `Failure`, `InternalServer` | 500 Internal Server Error |

Если один `ErrorList` содержит ошибки разных типов, возвращается `500`. Такой набор не имеет одного корректного HTTP-статуса; обработчик должен группировать ошибки по одному смысловому типу.

## События

WebFlow различает два вида событий:

```csharp
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}
```

Интерфейсы намеренно имеют одинаковые базовые поля, но являются разными типами: это не позволяет случайно передать внутреннее доменное событие в publisher внешних сообщений.

- `IDomainEvent` — внутренний факт доменной модели; он не является публичным контрактом и не отправляется в RabbitMQ напрямую.
- `IIntegrationEvent` — стабильный сериализуемый контракт для других модулей или внешних систем.

Пример:

```csharp
public sealed record UserRegisteredDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    User User) : IDomainEvent;

public sealed record UserRegisteredIntegrationEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid UserId,
    string Email) : IIntegrationEvent;
```

WebFlow не публикует сообщения и не зависит от RabbitMQ. Преобразование доменных событий в интеграционные, Outbox и отправка в брокер — ответственность Application и Infrastructure конкретного приложения.

## Unit of Work и транзакции

`IUnitOfWork` определяет сохранение изменений и создание явной транзакции без зависимости от EF Core:

```csharp
public interface IUnitOfWork
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### Обычная команда

Если все изменения выполняются через один `DbContext`, одного `SaveChangesAsync` достаточно. EF Core сохранит отслеживаемые изменения одной транзакцией базы данных.

```csharp
await users.AddAsync(user, cancellationToken);
await outbox.AddAsync(integrationEvent, cancellationToken);
await unitOfWork.SaveChangesAsync(cancellationToken);
```

Так в одной транзакции БД сохраняются пользователь и Outbox-сообщение. Публиковать RabbitMQ-сообщение внутри этой транзакции нельзя: для этого Outbox обрабатывается отдельным worker-ом после commit.

### Явная транзакция

Используйте её только если нужно объединить несколько сохранений или SQL-операций одной базы данных.

```csharp
using System.Data;

await using var transaction = await unitOfWork.BeginTransactionAsync(
    IsolationLevel.Serializable,
    cancellationToken);

try
{
    // Несколько операций с одной БД и одним DbContext.
    await unitOfWork.SaveChangesAsync(cancellationToken);

    await transaction.CommitAsync(cancellationToken);
}
catch
{
    await transaction.RollbackAsync(cancellationToken);
    throw;
}
```

Без параметра используется уровень изоляции базы данных по умолчанию. Не выбирайте `Serializable` «на всякий случай»: он повышает вероятность блокировок и конфликтов. Реализация `ITransaction` обязана откатить незавершённую транзакцию при `DisposeAsync`.

## Что намеренно не входит в WebFlow

- EF Core, Npgsql и конкретные реализации репозиториев;
- RabbitMQ, Outbox и dispatcher событий;
- аутентификация, авторизация и HTTP-клиенты;
- value objects, сущности и бизнес-правила конкретных модулей;
- автоматическое выполнение всех команд в транзакции.

Эти решения зависят от конкретного приложения и должны находиться в его Domain, Application или Infrastructure.

## Совместимость и миграция

WebFlow 2.0 — breaking change относительно `WebApplicationFlow` 1.x:

- целевая платформа — .NET 10;
- старый монолитный пакет разделён на три узких пакета;
- удалены зависимости от EF Core, Npgsql и банковской предметной логики;
- `ErrorsFlow` обновлён до 2.x;
- контроллеры сами выбирают успешный HTTP-статус, а не получают всегда `200 OK`.

Новые приложения используйте с WebFlow 2.0. Существующие проекты на 1.x мигрируйте отдельной задачей после проверки всех публичных контрактов.

## Разработка

```powershell
dotnet restore WebFlow.sln --source https://api.nuget.org/v3/index.json
dotnet test WebFlow.sln --configuration Release
dotnet pack WebFlow.sln --configuration Release
```

Перед выпуском новой версии выполняйте тесты, проверяйте изменения публичного API и публикуйте все три пакета с согласованной версией.

## Выпуск NuGet-пакетов

Публикация выполняется workflow-ом `.github/workflows/main.yml` после push тега формата `vX.Y.Z`. Версия пакетов берётся из тега без префикса `v`.

Перед первым выпуском необходимо:

1. Создать на nuget.org Trusted Publishing policy для репозитория `Ionefor/WebFlow` и workflow-файла `main.yml`.
2. Создать repository variable GitHub `NUGET_USER` со значением имени профиля nuget.org, а не email.
3. Убедиться, что политика разрешает публикацию всех трёх идентификаторов пакетов.

Постоянный NuGet API key в GitHub Secrets не нужен: workflow получает одноразовый ключ через OIDC непосредственно перед публикацией.

```powershell
git tag v2.0.0
git push origin v2.0.0
```

## Лицензия

MIT.
