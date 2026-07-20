# Nestgrid.Response.Mvc.Sample

MVC controller sample for the MVC adapter.

## Purpose

This sample demonstrates converting service-layer `Result<T>` values to MVC `IActionResult` responses with `ToActionResult()`.

## Run

From the repository root:

```bash
dotnet run --project samples/Nestgrid.Response.Mvc.Sample
```

## Demonstrates

- Registering Nestgrid response services.
- Returning results from controller actions.
- Mapping service outcomes to MVC HTTP responses.
- Keeping result creation in the service layer and response conversion in the controller layer.

## Limitations

The sample uses an in-memory singleton service. It is intended to show MVC response conversion, not persistence, authentication or model validation.

## Navigation

**Samples**

- [Samples index](../README.md)

**Repository**

- [Nestgrid.Response](../../README.md)
