# Nestgrid.Response.Http

Shared HTTP response mapping policy for Nestgrid.Response adapters.

`Nestgrid.Response.Http` maps result statuses to HTTP status codes and selects response payloads. It does not execute responses and does not depend on ASP.NET Core MVC packages.

## Installation

```bash
dotnet add package Nestgrid.Response.Http
```

Most applications should install an adapter package instead:

```bash
dotnet add package Nestgrid.Response.AspNetCore
```

or:

```bash
dotnet add package Nestgrid.Response.Mvc
```

## Quick Start

```csharp
using Nestgrid.Response;
using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;

var options = new NestgridResponseOptions
{
    SuccessResponseMode = SuccessResponseMode.ValueOnly
};

HttpResultMapping mapping = HttpResultMapper.Map(
    Results.NotFound<UserDto>("User was not found."),
    options,
    hasValue: false,
    value: null);
```

## Realistic Example

Use this package directly when you are building a custom adapter around the core result model:

```csharp
using Nestgrid.Response;
using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;

public static CustomResponse ToCustomResponse<T>(
    Result<T> result,
    NestgridResponseOptions options)
{
    var mapping = HttpResultMapper.Map(
        result,
        options,
        hasValue: true,
        value: result.Value);

    return new CustomResponse(
        statusCode: mapping.StatusCode,
        body: mapping.Body);
}
```

Application developers usually use `ToIResult()` or `ToActionResult()` from an adapter package instead of calling `HttpResultMapper` directly.

## Feature Summary

- Default mapping from every `ResultStatus` to an HTTP status code.
- Configurable status mappings through `NestgridResponseOptions`.
- `FullResult` and `ValueOnly` success payload modes.
- Consistent failure payload behavior.
- Bodyless handling for `ResultStatus.NoContent`.
- Shared policy used by ASP.NET Core and MVC adapters.

## Default Status Mappings

| Result status | HTTP status |
|---|---:|
| `Ok` | 200 |
| `Created` | 201 |
| `Accepted` | 202 |
| `NoContent` | 204 |
| `Invalid` | 400 |
| `Unauthorized` | 401 |
| `Forbidden` | 403 |
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Cancelled` | 409 |
| `Failed` | 422 |
| `Error` | 500 |

If a mapping is removed from an options instance, the mapper falls back to the default mapping.

## Payload Modes

`FullResult` writes the result envelope.

`ValueOnly` writes only the value for successful generic results.

Failures always write the result envelope. `NoContent` never writes a response body.

## Documentation

- [Main repository](https://github.com/nestgrid/Nestgrid.Response)
- [Architecture overview](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/handbooks/05%20Architecture/Overview.md)
- [ASP.NET Core package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response.AspNetCore)
- [MVC package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response.Mvc)

## Samples

- [ASP.NET Core sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.AspNetCore.Sample)
- [MVC sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Mvc.Sample)
