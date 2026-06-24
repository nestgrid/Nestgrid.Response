# Nestgrid.Response.Extensions.Validation

`Nestgrid.Response.Extensions.Validation` converts `System.ComponentModel.DataAnnotations.ValidationResult` values into Nestgrid response messages and invalid results.

The package is an optional convenience layer. It does not add result types, statuses, factories, or validation abstractions.

## Installation

```bash
dotnet add package Nestgrid.Response.Extensions.Validation
```

## Usage

```csharp
using Nestgrid.Response;
using Nestgrid.Response.Extensions.Validation;

IEnumerable<ValidationResult> validationResults = validator.Validate(model);

return validationResults.ToInvalidResult();
```

For typed service results:

```csharp
using Nestgrid.Response;
using Nestgrid.Response.Extensions.Validation;

IEnumerable<ValidationResult> validationResults = validator.Validate(model);

return validationResults.ToInvalidResult<UserDto>();
```

To convert validation results to messages:

```csharp
using Nestgrid.Response.Extensions.Validation;

var messages = validationResults.ToMessages();
```

Single validation results are supported too:

```csharp
ValidationResult validationResult = new("Name is required");

Result result = validationResult.ToInvalidResult();
```

## Behaviour

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
