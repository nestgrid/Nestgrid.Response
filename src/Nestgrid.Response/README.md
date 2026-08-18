# Nestgrid.Response

Framework-independent Result pattern primitives for .NET applications.

`Nestgrid.Response` provides immutable `Result` and `Result<T>` types for returning expected operation outcomes without throwing exceptions for routine control flow or coupling application logic to HTTP.

## Installation

```bash
dotnet add package Nestgrid.Response
```

## Quick Start

```csharp
using Nestgrid.Response;

public Result<UserDto> FindUser(int id)
{
    var user = users.Find(id);

    return user is null
        ? Results.NotFound<UserDto>("User was not found.")
        : Results.Ok(new UserDto(user.Id, user.Name));
}
```

## Realistic Example

```csharp
using Nestgrid.Response;
using Nestgrid.Response.Extensions;

public Result<UserDto> RenameUser(int id, string name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Results.Invalid<UserDto>(
            ResultMessages.Warning(
                "Name is required.",
                code: "name_required",
                property: nameof(name)));
    }

    var user = users.Find(id);

    if (user is null)
    {
        return Results.NotFound<UserDto>("User was not found.");
    }

    user.Rename(name);

    return Results.Ok(user)
        .Map(value => new UserDto(value!.Id, value.Name));
}
```

## Feature Summary

- Immutable `Result` and `Result<T>` models.
- Semantic statuses such as `Ok`, `Invalid`, `NotFound`, `Conflict`, `Failed`, and `Error`.
- `Results` factory methods for successful and non-success outcomes.
- Structured `ResultMessage` values with severity, code, property and message text.
- `ResultMessages` factory methods for information, warning and error messages.
- `IsSuccess()`, `IsFailure()`, `Map()`, and `Match()` extensions.
- JSON serialization that omits `Result.Status` from payloads by default.
- Generic no-content results through `Results.NoContent<T>()`.

## Result Messages

Create messages with `ResultMessages.Info`, `ResultMessages.Warning`, or `ResultMessages.Error`:

```csharp
var message = ResultMessages.Error(
    "Name is required.",
    code: "name_required",
    property: "Name");
```

Messages are exposed as a read-only snapshot through `Result.Messages`.

## Status Values

| Status | Meaning |
|---|---|
| `Ok` | Completed successfully |
| `Created` | Completed and created a resource |
| `Accepted` | Accepted for processing |
| `NoContent` | Completed without content |
| `Invalid` | Supplied input was invalid |
| `NotFound` | Requested resource was not found |
| `Unauthorized` | Caller is not authenticated |
| `Forbidden` | Caller is not permitted |
| `Conflict` | Conflicts with the current state |
| `Cancelled` | Operation was cancelled |
| `Failed` | Failed for an expected reason |
| `Error` | Failed because of an unexpected error |

Statuses are semantic application outcomes. Presentation packages decide how to map them to HTTP.

## Functional Extensions

Import the extension namespace:

```csharp
using Nestgrid.Response.Extensions;
```

`IsSuccess()` returns `true` for `Ok`, `Created`, `Accepted`, and `NoContent`. `IsFailure()` returns `true` for all other statuses.

`Map()` transforms the value of a successful `Result<T>` while preserving the original status and messages:

```csharp
Result<UserDto> dto = user.Map(value =>
    new UserDto(value!.Id, value.Name));
```

`Match()` branches on the same fixed success classification:

```csharp
var displayName = dto.Match(
    success => success?.Name ?? "Unknown",
    failure => $"Could not load user: {failure.Status}");
```

Use `Result.Status` directly when code needs to distinguish specific outcomes.

## Error(Exception)

`Results.Error(Exception)` converts an exception into an error result:

```csharp
try
{
    await service.RunAsync();
    return Results.Ok();
}
catch (Exception exception)
{
    logger.LogError(exception, "The operation failed.");
    return Results.Error(exception);
}
```

The exception itself is not retained and diagnostic details are not copied into the normal result:

```csharp
return Results.Error(exception);
```

This returns the client-safe message `An unexpected error occurred.` with no exception-derived code. Log the exception separately. This safe default is an intentional behaviour correction in v0.7.0; callers must not depend on the previous raw message or type-name output.

For trusted internal diagnostics only, use the explicitly named methods:

```csharp
var diagnostic = Results.ErrorWithDiagnosticDetails(exception);
var typedDiagnostic = Results.ErrorWithDiagnosticDetails<UserDto>(exception);
```

These methods preserve the exception message and type name. Do not return their results directly to untrusted clients or serialise them without an explicit output policy.

## NoContent<T>

`Results.NoContent<T>()` supports strongly typed service signatures that can complete without returning a value:

```csharp
Task<Result<UserDto>> GetAsync(int id)
{
    return Task.FromResult(Results.NoContent<UserDto>());
}
```

HTTP adapters treat `ResultStatus.NoContent` as a bodyless response.

## Documentation

- [Main repository](https://github.com/nestgrid/Nestgrid.Response)
- [Architecture overview](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/handbooks/05%20Architecture/Overview.md)
- [Roadmap](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/artefacts/07%20Release/Roadmap.md)
- [Mutation testing](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/handbooks/09%20Testing/Mutation%20Testing.md)

## Samples

- [Core sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Sample)
- [ASP.NET Core sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.AspNetCore.Sample)
- [MVC sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Mvc.Sample)
- [Validation sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Extensions.Validation.Sample)
