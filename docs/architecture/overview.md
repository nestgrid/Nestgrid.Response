# Architecture Overview

## Purpose

Nestgrid.Response is a lightweight, framework-agnostic result library for .NET applications.

Its purpose is to provide a consistent and expressive way to communicate operation outcomes between layers of an application without relying on exceptions for expected business outcomes.

The library is designed to be simple, explicit, immutable, and easy to integrate with presentation frameworks such as ASP.NET Core.

---

## Goals

- Provide a clear representation of operation outcomes.
- Support both non-generic and generic results.
- Remain framework-agnostic.
- Be easy to test and reason about.
- Support validation scenarios.
- Support API and UI applications through adapter packages.
- Minimise dependencies.
- Prioritise readability over clever abstractions.

---

## Non-Goals

Nestgrid.Response is not intended to be:

- A functional programming framework.
- A workflow engine.
- A mediator framework.
- An exception replacement.
- An HTTP abstraction.
- A validation framework.

---

## Architecture

The solution is separated into a lightweight core package, shared HTTP policy, and optional framework-specific adapters.

```text
Nestgrid.Response
        │
        ▼
Nestgrid.Response.Http
        │
        ├───────────────┐
        ▼               ▼
AspNetCore             Mvc
```

The core package contains the result model and supporting abstractions.

`Nestgrid.Response.Http` owns HTTP status and payload mapping policy.

Adapter packages are responsible for resolving options and writing framework-specific responses.

---

## Package Structure

```text
Nestgrid.Response
Nestgrid.Response.Http
Nestgrid.Response.AspNetCore
Nestgrid.Response.Mvc
Nestgrid.Response.Extensions.Validation
```

### Nestgrid.Response

Contains:

- Result
- Result<T>
- ResultStatus
- ResultMessage
- ResultMessageSeverity
- Results factories
- ResultMessages factories

No framework-specific dependencies.

### Nestgrid.Response.Http

Contains:

- NestgridResponseOptions
- SuccessResponseMode
- Default HTTP status mappings
- HttpResultMapper
- HttpResultMapping

Depends on:

- Nestgrid.Response

No ASP.NET Core or MVC dependencies.

### Nestgrid.Response.AspNetCore

Contains:

- Minimal API `IResult` conversion
- Controller `IActionResult` conversion
- ASP.NET Core service registration
- ASP.NET Core response execution

Depends on:

- Nestgrid.Response.Http
- ASP.NET Core

### Nestgrid.Response.Mvc

Contains:

- MVC `IActionResult` conversion
- MVC service registration
- MVC response execution

Depends on:

- Nestgrid.Response.Http
- Microsoft.AspNetCore.Mvc.Core

Adapters intentionally do not duplicate HTTP policy. They consume `Nestgrid.Response.Http` and translate the mapping into framework execution.

---

## Result Lifecycle

A service performs work and returns a result.

Example:

```csharp
var user = await store.GetByIdAsync(id);

if (user == null)
{
    return Results.NotFound("User not found");
}

return Results.Ok(user);
```

The presentation layer converts the result into a framework-specific response.

Example:

```csharp
var result = await service.GetAsync(id);

return result.ToIResult();
```

---

## Status Model

Nestgrid.Response uses a status-driven model.

The status communicates the semantic outcome of an operation.

Initial statuses:

```text
Ok
Created
Accepted
NoContent

Invalid
NotFound
Unauthorized
Forbidden
Conflict

Cancelled
Failed
Error
```

Status is the authoritative source of outcome information.

`IsSuccess()` and `IsFailure()` are public extension methods for code that only needs the fixed success classification.

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

This classification is not configurable. Code that needs to distinguish specific outcomes should inspect `Result.Status`.

---

## Functional Extensions

The core package includes lightweight functional extensions in `Nestgrid.Response.Extensions`.

`Map()` is status-driven. It invokes the mapper only when `IsSuccess()` returns `true`, preserves the original status and messages, and returns a result with no value for non-success statuses.

```csharp
Result<UserDto> dto = user.Map(x => mapper.Map<UserDto>(x));
```

`Match()` is also status-driven. It invokes the success delegate for `Ok`, `Created`, `Accepted`, and `NoContent`, and invokes the failure delegate for all other statuses.

```csharp
var text = result.Match(
    success => success?.Name ?? "Unknown",
    failure => $"Failed: {failure.Status}");
```

---

## NoContent Results

`Results.NoContent<T>()` exists for strongly typed service signatures:

```csharp
Task<Result<UserDto>> GetAsync(int id)
{
    return Task.FromResult(Results.NoContent<UserDto>());
}
```

The generic factory creates a `Result<T>` with `ResultStatus.NoContent` and the default value for `T`.

HTTP adapters treat `ResultStatus.NoContent` as a true no-content response. `ToIResult()` and `ToActionResult()` return a response with no body for `NoContent`, regardless of whether the result is generic, whether a value is associated with the result, or whether `SuccessResponseMode` is `FullResult` or `ValueOnly`.

---

## Immutability

Results are immutable.

Once created, a result cannot be modified.

Benefits include:

- Predictability
- Thread Safety
- Simpler Testing
- Easier Debugging

Factory methods are the preferred mechanism for creating results.

Example:

```csharp
Results.Ok();

Results.Ok(user);

Results.Created(account);

Results.NotFound("User not found");

Results.Invalid(errors);

Results.Error(exception);
```

---

## Validation

Validation is a first-class scenario.

Validation failures should be represented using the `Invalid` status.

Validation details are represented by structured result messages.

Example:

```csharp
return Results.Invalid(
    ResultMessages.Error(
        "Username is required",
        code: "username_required",
        property: "Username"));
```

---

## Exceptions

Expected business outcomes should be represented as results.

Examples:

- Validation Failure
- Not Found
- Unauthorized
- Forbidden
- Conflict

Unexpected system failures should continue to use exceptions.

Applications may choose to convert exceptions into results when appropriate.

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

---

## Design Principles

### Explicit Over Magic

Public APIs should be obvious and predictable.

### Immutable By Default

Results represent completed outcomes and should not change.

### Framework Independence

Core should not depend on presentation technologies.

### Readability First

Code should be easy to understand and maintain.

### Minimal Dependencies

Avoid unnecessary external dependencies.

### Stable Public API

Breaking changes should be carefully considered and documented.

---

## Future Considerations

Potential future enhancements include:

- ProblemDetails Enhancements
- OpenAPI Integration
- Source Generators
- Result Codes
- Additional Adapter Packages

These features should be added only when they provide clear value and do not compromise the simplicity of the core library.
