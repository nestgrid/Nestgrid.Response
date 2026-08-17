# Nestgrid.Response.AspNetCore

ASP.NET Core adapters for converting Nestgrid results into Minimal API and controller responses.

`Nestgrid.Response.AspNetCore` adds `ToIResult()` and `ToActionResult()` adapters backed by the shared HTTP mapping policy in `Nestgrid.Response.Http`.

## Installation

```bash
dotnet add package Nestgrid.Response.AspNetCore
```

## Quick Start

```csharp
using Nestgrid.Response;
using Nestgrid.Response.AspNetCore.Extensions;

builder.Services.AddNestgridResponse();

app.MapGet("/users/{id:int}", (int id, UserService users) =>
{
    Result<UserDto> result = users.Get(id);

    return result.ToIResult();
});
```

`AddNestgridResponse()` is optional when the built-in defaults are sufficient. Register it when you want global response options.

## Realistic Example

```csharp
using Nestgrid.Response;
using Nestgrid.Response.AspNetCore.Extensions;
using Nestgrid.Response.Http.Options;

builder.Services.AddNestgridResponse(options =>
{
    options.SuccessResponseMode = SuccessResponseMode.ValueOnly;
});

app.MapPost("/users", (CreateUserRequest request, UserService users) =>
{
    Result<UserDto> result = users.Create(request);

    return result.ToIResult();
});
```

For a single endpoint, pass options directly:

```csharp
var options = new NestgridResponseOptions
{
    SuccessResponseMode = SuccessResponseMode.ValueOnly
};

return result.ToIResult(options);
```

## Feature Summary

- Converts `Result` and `Result<T>` to Minimal API `IResult`.
- Converts `Result` and `Result<T>` to MVC `IActionResult`.
- Supports global and per-call `NestgridResponseOptions`.
- Uses shared HTTP mapping from `Nestgrid.Response.Http`.
- Supports `FullResult` and `ValueOnly` success payload modes.
- Suppresses response bodies for `ResultStatus.NoContent`.

## SuccessResponseMode

`FullResult` is the default. It serializes the result envelope:

```json
{
  "value": {
    "id": 1,
    "name": "Ada"
  },
  "messages": []
}
```

`ValueOnly` serializes only the value for successful generic results:

```json
{
  "id": 1,
  "name": "Ada"
}
```

Failures always write the result envelope. `NoContent` results never write a response body.

## Custom Status Mappings

Override mappings globally:

```csharp
builder.Services.AddNestgridResponse(options =>
{
    options.StatusMappings[ResultStatus.Failed] =
        StatusCodes.Status400BadRequest;
});
```

Default mappings:

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

Custom mappings are consumer-owned security configuration. Preserve the defaults for `Unauthorized`, `Forbidden`, `Error` and `NoContent` unless the integration has explicitly reviewed the authentication, authorisation, caching and client-control consequences.

## Mapping Results

Map application results before converting them to HTTP responses:

```csharp
using Nestgrid.Response.Extensions;

app.MapGet("/users/{id:int}", (int id, UserService users) =>
{
    return users.GetDomainUser(id)
        .Map(user => new UserDto(user!.Id, user.Name))
        .ToIResult();
});
```

`Map()` only executes for successful results. Non-success statuses preserve their status and messages and bypass the mapper.

## OpenAPI Guidance

The package converts responses at runtime but does not add OpenAPI metadata. Declare response status codes and payload types on each endpoint.

For `FullResult`:

```csharp
app.MapGet("/users/{id:int}", GetUser)
    .Produces<Result<UserDto>>(StatusCodes.Status200OK)
    .Produces<Result<UserDto>>(StatusCodes.Status404NotFound);
```

For `ValueOnly`, describe successful responses with the value type and failures with the result type:

```csharp
app.MapGet("/users/{id:int}", GetUser)
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces<Result<UserDto>>(StatusCodes.Status404NotFound);
```

Document `204 No Content` without a response type.

## Documentation

- [Main repository](https://github.com/nestgrid/Nestgrid.Response)
- [Core package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response)
- [HTTP mapping package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response.Http)
- [Architecture overview](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/handbooks/05%20Architecture/Overview.md)

## Samples

- [ASP.NET Core sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.AspNetCore.Sample)
- [MVC sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Mvc.Sample)
