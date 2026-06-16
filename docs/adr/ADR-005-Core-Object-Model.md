# ADR-005: Core Object Model

## Status

Accepted

## Context

Nestgrid.Response is intended to provide a lightweight and framework-agnostic mechanism for communicating operation outcomes between application layers.

Several object model approaches were considered, including:

- Separate validation models.
- Exception storage within results.
- Mutable result objects.
- Complex error hierarchies.
- Message collections based on plain strings.

The goal is to provide a simple, expressive, and future-proof model that remains easy to understand and use.

## Decision

The core object model consists of the following types:

```text
ResultStatus
ResultMessageSeverity
ResultMessage
Result
Result<T>
Results
ResultMessages
```

The model intentionally remains small and focused.

### ResultStatus

Represents the semantic outcome of an operation.

Initial statuses are:

```csharp
public enum ResultStatus
{
    Ok,
    Created,
    Accepted,
    NoContent,

    Invalid,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,

    Cancelled,
    Failed,
    Error
}
```

Status is the primary indicator of outcome.

### ResultMessageSeverity

Represents the severity of a message.

```csharp
public enum ResultMessageSeverity
{
    Information,
    Warning,
    Error
}
```

Severity indicates importance, not category.

Validation messages are represented as error messages and do not require a dedicated severity.

### ResultMessage

Represents structured information associated with a result.

```csharp
public sealed class ResultMessage
{
    public string Message { get; }

    public string? Code { get; }

    public string? Property { get; }

    public ResultMessageSeverity Severity { get; }
}
```

The `Property` field allows validation scenarios without introducing a dedicated validation object model.

Examples:

```csharp
ResultMessages.Error(
    "Username is required",
    property: "Username");

ResultMessages.Error(
    "User already exists",
    code: "user_already_exists");
```

### Result

Represents a non-generic operation outcome.

```csharp
public class Result
{
    public ResultStatus Status { get; }

    public IReadOnlyList<ResultMessage> Messages { get; }
}
```

### Result<T>

Represents a generic operation outcome containing a value.

```csharp
public sealed class Result<T> : Result
{
    public T? Value { get; }
}
```

The payload property is named `Value`.

`Data` was considered but rejected because it is less precise and less consistent with established .NET conventions.

### Results

Provides factory methods for creating results.

Examples:

```csharp
Results.Ok();

Results.Ok(user);

Results.Created();

Results.Created(user);

Results.NotFound();

Results.NotFound("User not found");

Results.Invalid(...);

Results.Error(...);
```

### ResultMessages

Provides factory methods for creating messages.

Examples:

```csharp
ResultMessages.Info("Account created");

ResultMessages.Warning("Licence expires soon");

ResultMessages.Error("User not found");
```

## Consequences

The resulting object model is:

- Lightweight.
- Immutable.
- Framework-agnostic.
- Easy to serialise.
- Easy to test.
- Easy to extend.

The model avoids unnecessary abstractions while still supporting validation, API responses, and rich error information.

## Rejected Alternatives

### ValidationError

A dedicated validation model was rejected.

Validation information can be represented using `ResultMessage.Property`.

Introducing a separate validation object would increase complexity without providing sufficient additional value.

### Exception Property

Storing exceptions directly on results was rejected.

Exceptions are primarily diagnostic information and are better handled through logging and exception handling infrastructure.

Including exceptions within results increases serialisation complexity and risks exposing implementation details.

### Plain String Messages

Using `IReadOnlyList<string>` was rejected.

Structured messages provide support for:

- Error Codes
- Validation Properties
- Localisation
- Machine-Readable Information

without significantly increasing complexity.

### Mutable Results

Mutable result objects were rejected in favour of immutable models.

Immutability improves predictability, testability, and thread safety.

## Notes

- Status represents outcome.
- Messages provide context.
- Value represents payload.
- Results are immutable.
- Factory methods are preferred over constructors.
- The core model should remain stable and intentionally small.
