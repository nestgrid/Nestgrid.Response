# Nestgrid.Response

Nestgrid.Response is a small, status-driven result model for .NET applications. It provides immutable results for communicating expected operation outcomes without coupling application code to HTTP or relying on exceptions for routine control flow.

## Packages

| Package | Purpose | Status              |
|---|---|---------------------|
| `Nestgrid.Response` | Framework-independent `Result` and `Result<T>` types, statuses, messages, and factories. | Available           |
| `Nestgrid.Response.AspNetCore` | Converts results to Minimal API `IResult` and MVC `IActionResult` responses. | Available           |
| `Nestgrid.Response.Extensions.Validation` | Converts data annotations validation results to messages and invalid results. | Available           |
| `Nestgrid.Response.Mvc` | Integration for ASP.NET MVC applications. | Under Consideration |

## Quick Start

Install the core package:

```bash
dotnet add package Nestgrid.Response
```

Create results through the `Results` factory:

```csharp
using Nestgrid.Response;

Result<User> FindUser(int id)
{
    var user = repository.Find(id);

    return user is null
        ? Results.NotFound<User>("User not found")
        : Results.Ok(user);
}
```

For ASP.NET Core, install the adapter and convert results at the application boundary:

```bash
dotnet add package Nestgrid.Response.AspNetCore
```

```csharp
using Nestgrid.Response.AspNetCore.Extensions;

app.MapGet("/users/{id:int}", (int id) =>
    FindUser(id).ToIResult());
```

Compose result values with the core functional extensions:

```csharp
using Nestgrid.Response.Extensions;

Result<UserDto> dto = user.Map(x => mapper.Map<UserDto>(x));

var name = dto.Match(
    success => success?.Name ?? "Unknown",
    failure => "Unknown");
```

`Map()` and `Match()` are status-driven. They use the fixed `IsSuccess()` classification: `Ok`, `Created`, `Accepted`, and `NoContent` are successful; all other statuses are non-success outcomes. `Map()` preserves status and messages, and does not invoke the mapper for non-success results.

For typed no-content service signatures, use `Results.NoContent<T>()`:

```csharp
Task<Result<UserDto>> GetAsync(int id)
{
    return Task.FromResult(Results.NoContent<UserDto>());
}
```

ASP.NET Core adapters always treat `ResultStatus.NoContent` as a bodyless response. `Results.NoContent<UserDto>().ToIResult()` returns `204 No Content` with no response body, regardless of `SuccessResponseMode`.

See the package documentation for the [core library](src/Nestgrid.Response/README.md) and [ASP.NET Core integration](src/Nestgrid.Response.AspNetCore/README.md).

## Roadmap

- `Nestgrid.Response`: core result model
- `Nestgrid.Response.AspNetCore`: Minimal API and controller integration
- `Nestgrid.Response.Extensions.Validation`: data annotations validation extensions
- `Nestgrid.Response.Mvc`: ASP.NET MVC integration - under consideration

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for build, test, coverage, and mutation-testing guidance.

## License

Nestgrid.Response is licensed under the [MIT License](LICENSE).
