# Nestgrid.Response.Http

`Nestgrid.Response.Http` contains the shared HTTP mapping policy used by Nestgrid response adapters.

It is not an execution adapter. It does not write responses and does not depend on ASP.NET Core MVC packages.

## Installation

```bash
dotnet add package Nestgrid.Response.Http
```

Most applications should install an adapter package instead:

- `Nestgrid.Response.AspNetCore`
- `Nestgrid.Response.Mvc`

## Why This Package Exists

HTTP status mapping and payload selection are shared policy. Keeping that policy in one package ensures ASP.NET Core and MVC adapters behave the same way.

Adapters remain intentionally thin:

- resolve options
- call `HttpResultMapper.Map(...)`
- write the mapped status and body through their framework abstraction

## Options

```csharp
var options = new NestgridResponseOptions
{
    SuccessResponseMode = SuccessResponseMode.ValueOnly
};

options.StatusMappings[ResultStatus.Failed] = 400;
```

## Default Status Mappings

| Result status | HTTP status |
| --- | ---: |
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
