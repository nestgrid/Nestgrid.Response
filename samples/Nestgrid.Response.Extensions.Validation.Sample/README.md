# Nestgrid.Response.Extensions.Validation.Sample

Console sample for data annotations validation extensions.

## Purpose

This sample demonstrates converting `ValidationResult` values into Nestgrid response messages and invalid results.

## Run

From the repository root:

```bash
dotnet run --project samples/Nestgrid.Response.Extensions.Validation.Sample
```

## Demonstrates

- Validating a model with `System.ComponentModel.DataAnnotations`.
- Converting one validation result to a `ResultMessage`.
- Converting many validation results to messages.
- Creating non-generic and typed invalid results.
- Creating member-aware messages and invalid results with a stable validation code.

## Limitations

The sample uses direct `Validator` calls to keep the validation-extension behavior clear. Real applications may wrap validation inside services, filters or endpoint pipelines.

## Navigation

**Samples**

- [Samples index](../README.md)

**Repository**

- [Nestgrid.Response](../../README.md)
