# Nestgrid.Response

`Nestgrid.Response` provides immutable, framework-independent results for representing operation outcomes in .NET applications.

## Installation

```bash
dotnet add package Nestgrid.Response
```

## Creating Results

Use the static `Results` factory to create `Result` or `Result<T>` instances:

```csharp
using Nestgrid.Response;

Result completed = Results.Ok();
Result<User> found = Results.Ok(user);
Result<User> missing = Results.NotFound<User>("User not found");
Result invalid = Results.Invalid("The request is invalid");
```

Success factories accept values and messages where appropriate:

```csharp
var result = Results.Ok(
    user,
    ResultMessages.Info("Loaded from cache"));
```

Failure factories have non-generic and generic overloads. A generic failure has a default value:

```csharp
Result<User> result = Results.Invalid<User>("Name is required");

// result.Value is null for a reference type.
```

## Result Messages

Create messages with `ResultMessages.Info`, `ResultMessages.Warning`, or `ResultMessages.Error`:

```csharp
var message = ResultMessages.Error(
    "Name is required",
    code: "name_required",
    property: "Name");
```

Each `ResultMessage` contains:

- `Message`: human-readable text
- `Code`: optional machine-readable code
- `Property`: optional related property
- `Severity`: `Information`, `Warning`, or `Error`

Messages are exposed as a read-only snapshot through `Result.Messages`.

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
    logger.LogError(exception, "The operation failed");
    return Results.Error(exception);
}
```

The exception itself is not retained. Its message becomes the result message, and its type name becomes the message code. Applications remain responsible for logging exceptions and deciding what information is safe to expose.

For a typed result, use `Results.Error<T>(exception)`.

## Result & Result<T>

`Result` represents an outcome without a value:

```csharp
Result result = Results.NoContent();

ResultStatus status = result.Status;
IReadOnlyList<ResultMessage> messages = result.Messages;
```

`Result<T>` inherits from `Result` and adds `Value`:

```csharp
Result<User> result = Results.Ok(user);
User? value = result.Value;
```

Results are immutable. Create them through `Results` rather than constructing them directly.

## Functional Extensions

Import the extension namespace:

```csharp
using Nestgrid.Response.Extensions;
```

### IsSuccess and IsFailure

Use `IsSuccess()` and `IsFailure()` when a branch only needs to distinguish successful outcomes from non-success outcomes:

```csharp
if (result.IsSuccess())
{
    // Continue the successful flow.
}
```

Success statuses:

- `Ok`
- `Created`
- `Accepted`
- `NoContent`

Failure statuses:

- `Invalid`
- `Unauthorized`
- `Forbidden`
- `NotFound`
- `Conflict`
- `Cancelled`
- `Failed`
- `Error`

This definition is fixed and not configurable. Use `Result.Status` when code needs to distinguish specific outcomes.

### Map

`Map` is status-driven. It transforms the value of a successful `Result<T>` while preserving the original status and messages:

```csharp
Result<User> user = await service.GetAsync(id);

Result<UserDto> dto = user.Map(x => mapper.Map<UserDto>(x));
```

The mapper is invoked only when the source result status is `Ok`, `Created`, `Accepted`, or `NoContent`. For all other statuses, the mapper is not invoked. The returned result keeps the original status and messages, and has no mapped value.

### Match

`Match` is status-driven. It uses the same fixed success and failure status groups:

```csharp
var name = user.Match(
    success => success?.Name ?? "Unknown",
    failure => "Unknown");
```

Non-generic results are also supported:

```csharp
var text = result.Match(
    () => "Success",
    failure => $"Failed: {failure.Status}");
```

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

Statuses are semantic application outcomes, not HTTP status codes. Presentation packages decide how to map them.

## NoContent<T>

`Results.NoContent<T>()` supports strongly typed service signatures that can complete without returning a value:

```csharp
Task<Result<UserDto>> GetAsync(int id)
{
    return Task.FromResult(Results.NoContent<UserDto>());
}
```

The generic factory creates a `Result<T>` with `ResultStatus.NoContent` and the default value for `T`.

HTTP adapters treat `ResultStatus.NoContent` as a true no-content response. `Results.NoContent<UserDto>().ToIResult()` returns `204 No Content` with no response body. This also applies in `SuccessResponseMode.FullResult` and `SuccessResponseMode.ValueOnly`.

## Why Status Is Not Serialized

`Result.Status` is excluded from JSON serialization by default.

The status exists for application control flow and adapter mapping. Omitting it avoids exposing transport-independent state in response payloads or duplicating an HTTP status code. Serialized results contain the value and messages only.

## Examples

Validation:

```csharp
return Results.Invalid<User>(
    ResultMessages.Error(
        "Name is required",
        code: "name_required",
        property: "Name"));
```

Conflict:

```csharp
return Results.Conflict<User>("A user with this email already exists");
```

Created result with context:

```csharp
return Results.Created(
    user,
    ResultMessages.Info("User created"));
```

Branching on the semantic outcome:

```csharp
var result = await service.UpdateAsync(command);

if (result.Status == ResultStatus.Conflict)
{
    // Handle the expected conflict.
}
```
