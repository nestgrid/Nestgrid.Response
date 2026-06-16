# Nestgrid.Response.AspNetCore

`Nestgrid.Response.AspNetCore` converts `Nestgrid.Response` results into ASP.NET Core Minimal API and controller responses.

## Installation

```bash
dotnet add package Nestgrid.Response.AspNetCore
```

Import the extension namespaces:

```csharp
using Nestgrid.Response;
using Nestgrid.Response.AspNetCore.Extensions;
using Nestgrid.Response.AspNetCore.Options;
```

## AddNestgridResponse()

Register the default options:

```csharp
builder.Services.AddNestgridResponse();
```

Registration is optional when the built-in defaults are sufficient. Global options are resolved when the response executes.

Configure response behavior during registration:

```csharp
builder.Services.AddNestgridResponse(options =>
{
    options.SuccessResponseMode = SuccessResponseMode.ValueOnly;
});
```

## SuccessResponseMode

`SuccessResponseMode` controls successful `Result<T>` payloads.

### FullResult

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

### ValueOnly

`ValueOnly` serializes only the value for successful generic results:

```json
{
  "id": 1,
  "name": "Ada"
}
```

`ValueOnly` does not remove failure details. Failure results and non-generic results retain the full result payload. `NoContent` results never write a response body.

Options can also be supplied for a single conversion:

```csharp
var options = new NestgridResponseOptions
{
    SuccessResponseMode = SuccessResponseMode.ValueOnly
};

return result.ToIResult(options);
```

Per-call options take precedence over globally registered options.

## Custom Status Mappings

Override mappings globally:

```csharp
builder.Services.AddNestgridResponse(options =>
{
    options.StatusMappings[ResultStatus.Failed] =
        StatusCodes.Status400BadRequest;
    options.StatusMappings[ResultStatus.Cancelled] =
        StatusCodes.Status422UnprocessableEntity;
});
```

Or override them for one response:

```csharp
var options = new NestgridResponseOptions();
options.StatusMappings[ResultStatus.Failed] =
    StatusCodes.Status400BadRequest;

return result.ToIResult(options);
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

If a configured mapping is removed, the adapter falls back to the built-in mapping for that status.

## Minimal API Examples

Convert `Result` or `Result<T>` with `ToIResult()`:

```csharp
app.MapGet("/users/{id:int}", async (int id, IUserService service) =>
{
    Result<UserDto> result = await service.FindAsync(id);
    return result.ToIResult();
});
```

```csharp
app.MapPost("/users", async (CreateUser request, IUserService service) =>
{
    Result<UserDto> result = await service.CreateAsync(request);
    return result.ToIResult();
});
```

## Controller Examples

Convert results with `ToActionResult()`:

```csharp
[ApiController]
[Route("users")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        Result<UserDto> result = await service.FindAsync(id);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUser request)
    {
        Result<UserDto> result = await service.CreateAsync(request);
        return result.ToActionResult();
    }
}
```

Both conversion methods support per-call `NestgridResponseOptions`.

## Result.Status Is Not Serialized

`Result.Status` selects the HTTP status code but is excluded from JSON. The HTTP response status is the transport-level outcome; the result status remains an application-level value.

In `FullResult` mode, clients receive `value` and `messages`. In `ValueOnly` mode, successful generic responses contain only the value. Failure responses still contain the result envelope without `status`.

## OpenAPI Guidance

The package converts responses at runtime but does not add OpenAPI metadata. Declare the response status codes and payload types on each endpoint.

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

For controllers, use `ProducesResponseType` with the same payload rules:

```csharp
[ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status404NotFound)]
```

Document `204 No Content` without a response type.
