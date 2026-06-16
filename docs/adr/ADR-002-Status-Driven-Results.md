# ADR-002: Status-Driven Results

## Status

Accepted

## Context

Many result implementations rely on a simple success/failure flag.

Examples include:

```csharp
result.IsSuccess
```

While simple, a boolean outcome does not communicate the intent of the operation.

The following outcomes are fundamentally different despite all being considered "not successful":

- Validation Failure
- Not Found
- Unauthorised
- Forbidden
- Conflict
- Cancelled
- Unexpected Error

Likewise, successful operations may represent different outcomes:

- Ok
- Created
- Accepted
- No Content

Consumers frequently need to distinguish between these states without inspecting messages or exceptions.

## Decision

Nestgrid.Response will use a status-driven model based on a `ResultStatus` enumeration.

Results will expose a status rather than relying solely on a success flag.

Example:

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

Convenience properties such as `IsSuccess` may be provided, but the status remains the authoritative source of truth.

## Consequences

Consumers can determine the outcome of an operation without parsing messages.

Presentation-layer adapters can map statuses directly to HTTP response codes.

Service-layer code remains explicit and self-documenting.

Examples:

```csharp
return Results.NotFound("User not found");

return Results.Invalid(validationErrors);

return Results.Created(account);
```

Status-driven results also make testing simpler because assertions can focus on the intended outcome rather than message text.

The `Error` status remains part of the model and represents unexpected failures that have been intentionally converted into a result. This allows applications to return structured outcomes without requiring consumers to catch exceptions directly.

Example:

```csharp
try
{
    ...
}
catch (Exception ex)
{
    return Results.Error(ex);
}
```

## Rejected Alternatives

### Boolean Success Flag

A simple success/failure flag does not provide sufficient information about the outcome of an operation.

### Exception-Based Flow Control

Exceptions remain appropriate for unexpected system failures but should not be used to represent expected business outcomes such as validation failures or missing resources.

### HTTP Status Codes in Core

The core library should remain framework-agnostic.

`ResultStatus` represents semantic outcomes, while adapter packages are responsible for mapping those outcomes to HTTP status codes.

## Notes

- Status is the primary indicator of outcome.
- `IsSuccess` is a convenience property only.
- Core remains independent of ASP.NET Core and HTTP concepts.
- Status values should remain stable because they form part of the public API surface.
- New statuses should only be introduced when they represent a distinct semantic outcome.
