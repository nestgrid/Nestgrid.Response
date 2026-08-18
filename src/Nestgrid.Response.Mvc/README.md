# Nestgrid.Response.Mvc

MVC adapter for converting Nestgrid results into `IActionResult` responses.

`Nestgrid.Response.Mvc` targets `netstandard2.0` and uses the shared HTTP mapping policy in `Nestgrid.Response.Http`. For modern .NET 8 ASP.NET Core applications, prefer `Nestgrid.Response.AspNetCore` unless you specifically need this older MVC-focused adapter.

## Support Lifecycle

`Nestgrid.Response.Mvc` is maintained as part of the full Nestgrid.Response library. It shares the library's versioning, support, security, compatibility and release-review lifecycle; it has no separate maintenance lifecycle or independent end-of-support policy. The documented compatibility baseline is `Microsoft.AspNetCore.Mvc.Core` `2.1.38`.

## Installation

```bash
dotnet add package Nestgrid.Response.Mvc
```

## Quick Start

```csharp
using Nestgrid.Response;
using Nestgrid.Response.Mvc.Extensions;

services.AddNestgridResponse();

public IActionResult Get(int id)
{
    Result<UserDto> result = users.Get(id);

    return result.ToActionResult();
}
```

## Realistic Example

```csharp
using Microsoft.AspNetCore.Mvc;
using Nestgrid.Response;
using Nestgrid.Response.Mvc.Extensions;

[ApiController]
[Route("users")]
public sealed class UsersController(UserService users) : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        Result<UserDto> result = users.Get(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public IActionResult Create(UserDto user)
    {
        Result<UserDto> result = users.Create(user);

        return result.ToActionResult();
    }
}
```

## Feature Summary

- Converts `Result` and `Result<T>` to MVC `IActionResult`.
- Supports global and per-call `NestgridResponseOptions`.
- Uses shared HTTP mapping from `Nestgrid.Response.Http`.
- Supports `FullResult` and `ValueOnly` success payload modes.
- Suppresses response bodies for `ResultStatus.NoContent`.
- Keeps MVC execution separate from core result modeling.

## Options

Register default options:

```csharp
using Nestgrid.Response.Http.Options;
using Nestgrid.Response.Mvc.Extensions;

services.AddNestgridResponse(options =>
{
    options.SuccessResponseMode = SuccessResponseMode.ValueOnly;
    options.StatusMappings[ResultStatus.Failed] = 400;
});
```

Pass options for one response:

```csharp
var options = new NestgridResponseOptions
{
    SuccessResponseMode = SuccessResponseMode.ValueOnly
};

return result.ToActionResult(options);
```

## Behavior

MVC behavior matches the ASP.NET Core adapter:

- `FullResult` writes the result envelope.
- `ValueOnly` writes only the value for successful generic results.
- Failures write the result envelope.
- Custom status mappings are supported.
- Missing custom mappings fall back to defaults.
- `NoContent` never writes a response body.

HTTP mapping is owned by `Nestgrid.Response.Http`; this package only adapts the mapping to MVC execution.

Custom mappings are consumer-owned security configuration. Preserve the defaults for `Unauthorized`, `Forbidden`, `Error` and `NoContent` unless the integration has explicitly reviewed the authentication, authorisation, caching and client-control consequences.

## Documentation

- [Main repository](https://github.com/nestgrid/Nestgrid.Response)
- [Core package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response)
- [HTTP mapping package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response.Http)
- [ASP.NET Core package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response.AspNetCore)
- [Architecture overview](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/handbooks/05%20Architecture/Overview.md)

## Samples

- [MVC sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Mvc.Sample)
- [ASP.NET Core sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.AspNetCore.Sample)
