# Nestgrid.Response.Mvc

`Nestgrid.Response.Mvc` converts Nestgrid results to MVC `IActionResult` responses.

The package targets `netstandard2.0` and uses the shared `Nestgrid.Response.Http` package for all HTTP mapping policy.

## Installation

```bash
dotnet add package Nestgrid.Response.Mvc
```

## Register Options

```csharp
using Nestgrid.Response.Mvc.Extensions;
using Nestgrid.Response.Http;

services.AddNestgridResponse();
```

Configure response policy:

```csharp
services.AddNestgridResponse(options =>
{
    options.SuccessResponseMode = SuccessResponseMode.ValueOnly;
    options.StatusMappings[ResultStatus.Failed] = 400;
});
```

## Controller Usage

```csharp
using Nestgrid.Response.Mvc.Extensions;
using Nestgrid.Response.Http;

public IActionResult Get(int id)
{
    Result<UserDto> result = service.Get(id);

    return result.ToActionResult();
}
```

Per-call options:

```csharp
var options = new NestgridResponseOptions
{
    SuccessResponseMode = SuccessResponseMode.ValueOnly
};

return result.ToActionResult(options);
```

## Behaviour

MVC behaviour matches the ASP.NET Core adapter:

- `FullResult` writes the result envelope.
- `ValueOnly` writes only the value for successful generic results.
- Failures write the result envelope.
- Custom status mappings are supported.
- Missing custom mappings fall back to defaults.
- `NoContent` never writes a response body.

HTTP mapping is owned by `Nestgrid.Response.Http`; this package only adapts the mapping to MVC execution.
