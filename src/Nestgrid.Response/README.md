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
