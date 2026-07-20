# Architecture Overview

> Part of the **[Architecture](README.md)** handbook.

## Purpose

Nestgrid.Response is a lightweight, framework-agnostic result library for .NET applications.

Its purpose is to provide a consistent way to communicate operation outcomes between application layers without relying on exceptions for expected business outcomes.

The library is designed to be simple, explicit, immutable and easy to integrate with presentation frameworks such as ASP.NET Core.

## Goals

- Provide a clear representation of operation outcomes.
- Support both non-generic and generic results.
- Remain framework-agnostic in the core package.
- Be easy to test and reason about.
- Support validation scenarios.
- Support API applications through adapter packages.
- Minimise dependencies.
- Prioritise readability over clever abstractions.

## Non-Goals

Nestgrid.Response is not intended to be:

- A functional programming framework.
- A workflow engine.
- A mediator framework.
- An exception replacement.
- An HTTP abstraction.
- A validation framework.

## Package Relationships

```text
Nestgrid.Response
        |
        +--> Nestgrid.Response.Extensions.Validation
        |
        +--> Nestgrid.Response.Http
                    |
                    +--> Nestgrid.Response.AspNetCore
                    |
                    +--> Nestgrid.Response.Mvc
```

The core package contains the result model and supporting factories.

`Nestgrid.Response.Http` owns HTTP status and payload mapping policy.

Adapter packages resolve options and write framework-specific responses.

## Package Responsibilities

### Nestgrid.Response

Contains:

- `Result`
- `Result<T>`
- `ResultStatus`
- `ResultMessage`
- `ResultMessageSeverity`
- `Results` factories
- `ResultMessages` factories
- Core functional extensions

No framework-specific dependencies.

### Nestgrid.Response.Http

Contains:

- `NestgridResponseOptions`
- `SuccessResponseMode`
- Default HTTP status mappings
- `HttpResultMapper`
- `HttpResultMapping`

Depends on `Nestgrid.Response`.

No ASP.NET Core or MVC dependencies.

### Nestgrid.Response.AspNetCore

Contains:

- Minimal API `IResult` conversion.
- Controller `IActionResult` conversion.
- ASP.NET Core service registration.
- ASP.NET Core response execution.

Depends on `Nestgrid.Response.Http` and ASP.NET Core.

### Nestgrid.Response.Mvc

Contains:

- MVC `IActionResult` conversion.
- MVC service registration.
- MVC response execution.

Depends on `Nestgrid.Response.Http` and `Microsoft.AspNetCore.Mvc.Core`.

## Result Lifecycle

A service performs work and returns a result:

```csharp
var user = await store.GetByIdAsync(id);

if (user is null)
{
    return Results.NotFound<UserDto>("User was not found.");
}

return Results.Ok(new UserDto(user.Id, user.Name));
```

The presentation layer converts the result into a framework-specific response:

```csharp
var result = await service.GetAsync(id);

return result.ToIResult();
```

## Status Model

Nestgrid.Response uses a status-driven model. The status communicates the semantic outcome of an operation.

Success statuses:

- `Ok`
- `Created`
- `Accepted`
- `NoContent`

Non-success statuses:

- `Invalid`
- `Unauthorized`
- `Forbidden`
- `NotFound`
- `Conflict`
- `Cancelled`
- `Failed`
- `Error`

This classification is fixed. Code that needs to distinguish specific outcomes should inspect `Result.Status`.

## Functional Extensions

The core package includes lightweight functional extensions in `Nestgrid.Response.Extensions`.

`Map()` invokes the mapper only when `IsSuccess()` returns `true`, preserves the original status and messages, and returns a result with no mapped value for non-success statuses.

```csharp
Result<UserDto> dto = user.Map(value =>
    new UserDto(value!.Id, value.Name));
```

`Match()` uses the same fixed success and failure status groups.

```csharp
var text = result.Match(
    success => success?.Name ?? "Unknown",
    failure => $"Failed: {failure.Status}");
```

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

## Immutability

Results are immutable. Once created, a result cannot be modified.

Benefits include:

- Predictability.
- Thread safety.
- Simpler testing.
- Easier debugging.

Factory methods are the preferred mechanism for creating results.

## Validation

Validation failures should be represented using the `Invalid` status.

Validation details are represented by structured result messages:

```csharp
return Results.Invalid(
    ResultMessages.Error(
        "Username is required.",
        code: "username_required",
        property: "Username"));
```

The optional validation package converts data annotations validation results into this shape.

---

## Navigation

**Book**

- [Architecture](README.md)

**Documentation**

- [Documentation index](../../README.md)

**Repository**

- [Nestgrid.Response](../../../README.md)
