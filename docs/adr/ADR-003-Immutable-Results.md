# ADR-003: Immutable Results

## Status

Accepted

## Context

Result objects are intended to represent the final outcome of an operation.

Mutable result objects can introduce several problems:

- State changes after creation.
- Difficult debugging.
- Unexpected side effects.
- Thread-safety concerns.
- Inconsistent behaviour between layers.

Many result implementations allow mutation through methods such as:

```csharp
result.WithError("...");
result.WithMessage("...");
result.WithMetadata(...);
```

While flexible, these patterns make it harder to reason about the state of a result at any given point in time.

Nestgrid.Response aims to favor predictability, simplicity, and explicit intent.

## Decision

All result objects in Nestgrid.Response will be immutable.

Once created, a result cannot be modified.

Properties will be exposed as read-only and assigned during construction.

Examples:

```csharp
var result = Results.NotFound("User not found");

var result = Results.Invalid(errors);

var result = Results.Ok(account);
```

Collections exposed by results will be immutable or read-only.

Example:

```csharp
IReadOnlyList<ResultMessage>
```

Factory methods will be the preferred mechanism for creating results.

Consumers should not construct result objects directly unless a specific advanced scenario requires it.

## Consequences

Results become easier to reason about because their state cannot change after creation.

Results become naturally thread-safe.

Unit tests become simpler because assertions can rely on a stable object state.

Logging and diagnostics become more predictable because a logged result cannot later be modified.

Factory methods become the primary mechanism for creating results.

Examples:

```csharp
Results.Ok();

Results.Ok(user);

Results.Created(account);

Results.NotFound("User not found");

Results.Invalid(errors);

Results.Error(exception);
```

## Rejected Alternatives

### Mutable Results

Allowing result objects to be modified after creation introduces additional complexity and makes application flow harder to understand.

### Fluent Mutation APIs

Patterns such as:

```csharp
new Result()
    .WithMessage("...")
    .WithError("...")
    .WithMetadata(...);
```

were rejected because they encourage incremental construction and mutable state.

Nestgrid.Response prefers explicit construction of a complete result.

### Public Setters

Public setters were rejected because they allow consumers to alter the meaning of a result after creation.

Example:

```csharp
result.Status = ResultStatus.Ok;
```

Such behaviour can lead to inconsistent and difficult-to-debug applications.

## Notes

- Results represent completed outcomes.
- Results should not change after creation.
- Factory methods are the preferred creation mechanism.
- Readability and predictability take precedence over flexibility.
- Immutability applies to both `Result` and `Result<T>`.
