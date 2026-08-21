# Nestgrid.Response

[![CI](https://github.com/nestgrid/Nestgrid.Response/actions/workflows/ci.yml/badge.svg)](https://github.com/nestgrid/Nestgrid.Response/actions/workflows/ci.yml)
[![Mutation Testing](https://github.com/nestgrid/Nestgrid.Response/actions/workflows/mutation.yml/badge.svg)](https://github.com/nestgrid/Nestgrid.Response/actions/workflows/mutation.yml)
[![NuGet](https://img.shields.io/nuget/v/Nestgrid.Response.svg)](https://www.nuget.org/packages/Nestgrid.Response)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Nestgrid.Response is a lightweight Result pattern library for modern .NET applications, designed to make application outcomes explicit, predictable and framework-independent.

It gives application and domain code a clear way to return expected outcomes such as `Ok`, `Invalid`, `NotFound`, `Conflict`, and `Error` without using exceptions for routine control flow or coupling business logic to HTTP.

## Packages

| Package | Purpose | Target |
|---|---|---|
| [`Nestgrid.Response`](src/Nestgrid.Response/README.md) | Framework-independent `Result` and `Result<T>` types, statuses, messages, factories, and functional extensions. | `netstandard2.0` |
| [`Nestgrid.Response.Http`](src/Nestgrid.Response.Http/README.md) | Shared HTTP status and payload mapping policy used by response adapters. | `netstandard2.0` |
| [`Nestgrid.Response.AspNetCore`](src/Nestgrid.Response.AspNetCore/README.md) | Minimal API `IResult` and controller `IActionResult` adapters for ASP.NET Core. | `net8.0` |
| [`Nestgrid.Response.Mvc`](src/Nestgrid.Response.Mvc/README.md) | MVC `IActionResult` adapter for older ASP.NET Core MVC applications. | `netstandard2.0` |
| [`Nestgrid.Response.Extensions.Validation`](src/Nestgrid.Response.Extensions.Validation/README.md) | Data annotations validation extensions for invalid results and result messages. | `netstandard2.0` |
| [`Nestgrid.Response.Http.Client`](src/Nestgrid.Response.Http.Client/README.md) | Explicit HTTP response interpretation into `Result` and `Result<T>` values. | `netstandard2.0` |

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
        |
        +--> Nestgrid.Response.Http.Client
```

The core package has no presentation-framework dependency. HTTP policy lives in one shared package so the ASP.NET Core and MVC adapters behave consistently.

## Why Use Nestgrid.Response?

Nestgrid.Response is useful when you want:

- A small Result pattern model with predictable statuses.
- Typed and untyped results with immutable messages.
- Application-layer outcomes that are not HTTP-specific.
- Consistent HTTP mapping when results reach Minimal APIs or MVC controllers.
- Lightweight `Map`, `Match`, `IsSuccess`, and `IsFailure` helpers without adopting a broader functional framework.
- Validation integration that works with existing data annotations.

The library is intentionally modest. It focuses on common result-flow needs and keeps the public API easy to read, test, and document.

It is designed to remain small, framework-agnostic where possible, and easy to adopt incrementally.

## Quick Start

Install the core package:

```bash
dotnet add package Nestgrid.Response
```

Return results from application services:

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

Map values without losing the original outcome:

```csharp
using Nestgrid.Response.Extensions;

Result<UserDto> result = userResult.Map(user =>
    new UserDto(user!.Id, user.Name));
```

Use the ASP.NET Core adapter at the application boundary:

```bash
dotnet add package Nestgrid.Response.AspNetCore
```

```csharp
using Nestgrid.Response.AspNetCore.Extensions;

app.MapGet("/users/{id:int}", (int id, UserService users) =>
    users.Find(id).ToIResult());
```

## Supported Frameworks

| Package | Supported frameworks |
|---|---|
| `Nestgrid.Response` | .NET Standard 2.0 consumers, including modern .NET applications |
| `Nestgrid.Response.Http` | .NET Standard 2.0 consumers |
| `Nestgrid.Response.Extensions.Validation` | .NET Standard 2.0 consumers |
| `Nestgrid.Response.AspNetCore` | ASP.NET Core on .NET 8 |
| `Nestgrid.Response.Mvc` | ASP.NET Core MVC applications compatible with `Microsoft.AspNetCore.Mvc.Core` 2.1.x |
| `Nestgrid.Response.Http.Client` | .NET Standard 2.0 consumers interpreting Nestgrid HTTP responses |

## Ecosystem

Nestgrid.Response is part of the Nestgrid engineering libraries.

Other libraries are being developed to provide reusable building blocks for modern .NET applications.

## Documentation

- [Documentation index](docs/README.md)
- [Philosophy](docs/handbooks/01%20Philosophy/README.md)
- [Architecture overview](docs/handbooks/05%20Architecture/Overview.md)
- [Coding standards](docs/handbooks/08%20Coding%20Standards/Coding%20Standards.md)
- [Mutation testing](docs/handbooks/09%20Testing/Mutation%20Testing.md)
- [Roadmap](docs/artefacts/07%20Release/Roadmap.md)
- [Architecture decisions](docs/decisions/README.md)

## Samples

| Sample | Demonstrates |
|---|---|
| [`samples/Nestgrid.Response.Sample`](samples/Nestgrid.Response.Sample/README.md) | Core result factories, `Map()`, and `Match()` in a console application. |
| [`samples/Nestgrid.Response.Extensions.Validation.Sample`](samples/Nestgrid.Response.Extensions.Validation.Sample/README.md) | Data annotations validation results converted to messages and invalid results. |
| [`samples/Nestgrid.Response.AspNetCore.Sample`](samples/Nestgrid.Response.AspNetCore.Sample/README.md) | Minimal API endpoints returning `result.ToIResult()`, including value-only responses. |
| [`samples/Nestgrid.Response.Mvc.Sample`](samples/Nestgrid.Response.Mvc.Sample/README.md) | MVC controller actions returning `result.ToActionResult()`. |
| [`samples/Nestgrid.Response.Http.Client.Sample`](samples/Nestgrid.Response.Http.Client.Sample/README.md) | FullResult and ValueOnly HTTP client interpretation for reusable consumer scenarios. |

Run a sample from the repository root:

```bash
dotnet run --project samples/Nestgrid.Response.Sample
```

## Roadmap

Nestgrid.Response is pre-1.0 and is currently focused on documentation quality, API stability, and adapter polish.

Planned areas are tracked in the [roadmap](docs/artefacts/07%20Release/Roadmap.md). Current candidates include OpenAPI documentation helpers, `ProblemDetails` guidance, and broader adapter evaluation. Breaking changes are avoided unless they are necessary before a stable 1.0 release.

## Contributing

Contributions are welcome when they keep the library small, predictable, and well tested.

Start with [CONTRIBUTING.md](CONTRIBUTING.md) for build, test, coverage, and mutation-testing guidance. Public behavior changes should include tests and documentation updates.

## License

Nestgrid.Response is licensed under the [MIT License](LICENSE).
