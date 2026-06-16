# ADR-006: ASP.NET Core and MVC Package Separation

## Status

Accepted

## Context

Nestgrid.Response is designed as a lightweight, framework-agnostic result library.

Consumers require integration with ASP.NET presentation technologies to convert `Result` and `Result<T>` instances into HTTP responses.

Modern ASP.NET Core applications commonly use:

- Minimal APIs (`IResult`)
- MVC Controllers (`ActionResult`)

Legacy applications may use:

- ASP.NET MVC
- Older ASP.NET Core versions
- .NET Framework applications

These environments have different dependency and compatibility requirements.

## Decision

The solution will be split into dedicated presentation-layer packages.

### Nestgrid.Response.AspNetCore

Target:

```text
Modern ASP.NET Core
```

Responsibilities:

- Convert Result to IResult
- Convert Result to ActionResult
- Provide ASP.NET Core configuration options
- Support Minimal APIs and Controllers

Dependencies:

```text
Microsoft.AspNetCore.App
```

Implementation will use:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

### Nestgrid.Response.Mvc

This package is under consideration.

Target:

```text
Legacy ASP.NET MVC and compatibility scenarios
```

Responsibilities:

- Convert Result to ActionResult
- Support .NET Framework and older ASP.NET Core applications

Dependencies:

```text
Microsoft.AspNetCore.Mvc.Core 2.3.0
```

No support for IResult will be provided by this package.

## Consequences

### Positive

- Modern ASP.NET Core can fully embrace framework features.
- Core library remains framework-agnostic.
- Legacy support remains available without constraining modern consumers.
- Clear separation of responsibilities.

### Negative

- Two adapter packages must be maintained.
- Some mapping logic may be duplicated between packages.

## Alternatives Considered

### Single ASP.NET Package

A single package supporting both modern and legacy scenarios was considered.

This was rejected because legacy compatibility requirements would constrain modern ASP.NET Core integration.

### ASP.NET Core Only

Supporting only modern ASP.NET Core was considered.

This was rejected because compatibility with older applications remains valuable for migration scenarios.

## Outcome

Nestgrid.Response consists of two packages, with a third under consideration:

```text
Nestgrid.Response
Nestgrid.Response.AspNetCore
Nestgrid.Response.Mvc (under consideration)
```

with modern and legacy presentation concerns separated into dedicated packages.
