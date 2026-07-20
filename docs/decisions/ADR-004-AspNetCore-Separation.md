# ADR-004: ASP.NET Separation

## Status

Accepted

## Context

Nestgrid.Response is intended to be a reusable library that can be used across multiple application types and frameworks.

Potential consumers include:

- Class Libraries
- Console Applications
- Background Services
- Worker Services
- ASP.NET Core APIs
- ASP.NET MVC Applications
- Desktop Applications
- Unit Test Projects

The core purpose of Nestgrid.Response is to represent operation outcomes.

HTTP concerns such as:

- Status Codes
- IActionResult
- IResult
- ProblemDetails
- OpenAPI

are presentation-layer concerns and are not required by many consumers.

Coupling the core library directly to ASP.NET Core would introduce unnecessary dependencies and reduce portability.

## Decision

Nestgrid.Response will remain framework-agnostic.

The core package will contain:

- Result
- Result<T>
- ResultStatus
- Validation Models
- Factory Methods
- Core Abstractions

The core package will not reference:

- Microsoft.AspNetCore.*
- System.Web.*
- HTTP Status Codes
- IActionResult
- IResult
- ProblemDetails

Framework-specific integrations will be implemented in separate packages.

Example package structure:

```text
Nestgrid.Response
Nestgrid.Response.AspNetCore
Nestgrid.Response.Mvc
```

The ASP.NET Core package will provide extensions for converting results into API responses.

Examples:

```csharp
return result.ToIResult();

return result.ToActionResult();
```

The mapping between `ResultStatus` and HTTP responses will be defined within adapter packages rather than the core library.

## Consequences

The core package remains lightweight and portable.

Consumers can use Nestgrid.Response without pulling ASP.NET Core dependencies into their projects.

Framework integrations can evolve independently from the core library.

The package structure becomes clearer:

```text
Application Layer
        │
        ▼
Nestgrid.Response
        │
        ▼
Presentation Layer
        │
        ▼
Nestgrid.Response.AspNetCore
```

Consumers only reference the packages they require.

Examples:

```text
Console App
    └── Nestgrid.Response

Worker Service
    └── Nestgrid.Response

ASP.NET Core API
    ├── Nestgrid.Response
    └── Nestgrid.Response.AspNetCore

ASP.NET MVC Application
    ├── Nestgrid.Response
    └── Nestgrid.Response.Mvc
```

## Rejected Alternatives

### ASP.NET Core in the Core Package

Referencing ASP.NET Core directly from the core package would create unnecessary dependencies and reduce portability.

### HTTP Status Codes in ResultStatus

Using HTTP status codes directly would tightly couple business outcomes to a specific transport protocol.

Nestgrid.Response represents semantic outcomes, not transport-specific responses.

### Single Package Approach

Combining all functionality into a single package would increase package size, increase dependencies, and make adoption more difficult for non-web consumers.

## Notes

- Core remains transport-agnostic.
- Core remains framework-agnostic.
- Presentation concerns belong in adapter packages.
- Adapter packages are responsible for HTTP mapping.
- Consumers should be able to use Nestgrid.Response without any ASP.NET Core dependency.
