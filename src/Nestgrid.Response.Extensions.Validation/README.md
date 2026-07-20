# Nestgrid.Response.Extensions.Validation

Data annotations validation extensions for Nestgrid.Response.

`Nestgrid.Response.Extensions.Validation` converts `System.ComponentModel.DataAnnotations.ValidationResult` values into Nestgrid result messages and invalid results.

## Installation

```bash
dotnet add package Nestgrid.Response.Extensions.Validation
```

## Quick Start

```csharp
using System.ComponentModel.DataAnnotations;
using Nestgrid.Response;
using Nestgrid.Response.Extensions.Validation;

var validationResults = new List<ValidationResult>();

Validator.TryValidateObject(
    request,
    new ValidationContext(request),
    validationResults,
    validateAllProperties: true);

Result result = validationResults.ToInvalidResult();
```

## Realistic Example

```csharp
using System.ComponentModel.DataAnnotations;
using Nestgrid.Response;
using Nestgrid.Response.Extensions.Validation;

public Result<UserDto> Create(CreateUserRequest request)
{
    var validationResults = new List<ValidationResult>();

    if (!Validator.TryValidateObject(
        request,
        new ValidationContext(request),
        validationResults,
        validateAllProperties: true))
    {
        return validationResults.ToInvalidResult<UserDto>();
    }

    var user = new UserDto(1, request.Name, request.Email);

    return Results.Created(user);
}
```

## Feature Summary

- Converts one `ValidationResult` to one `ResultMessage`.
- Converts many validation results to result messages.
- Creates non-generic and generic invalid results.
- Allows caller-selected message severity.
- Preserves validation message text exactly as supplied.
- Does not introduce a custom validation abstraction.

## Behavior

- Validation messages are preserved exactly as supplied.
- Member names are not prepended to messages.
- Message codes are not inferred.
- Message properties are not inferred.
- Default severity is `Warning`.
- `ToInvalidResult()` always returns `ResultStatus.Invalid`.

Override severity when needed:

```csharp
var result = validationResults.ToInvalidResult(ResultMessageSeverity.Error);
```

## Documentation

- [Main repository](https://github.com/nestgrid/Nestgrid.Response)
- [Core package](https://github.com/nestgrid/Nestgrid.Response/tree/main/src/Nestgrid.Response)
- [Architecture overview](https://github.com/nestgrid/Nestgrid.Response/blob/main/docs/handbooks/05%20Architecture/Overview.md)

## Samples

- [Validation sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Extensions.Validation.Sample)
- [Core sample](https://github.com/nestgrid/Nestgrid.Response/tree/main/samples/Nestgrid.Response.Sample)
