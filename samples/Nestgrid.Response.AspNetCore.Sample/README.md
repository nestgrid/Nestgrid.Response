# Nestgrid.Response.AspNetCore.Sample

Minimal API sample for the ASP.NET Core adapter.

## Purpose

This sample demonstrates converting service-layer `Result<T>` values to HTTP responses with `ToIResult()`.

## Run

From the repository root:

```bash
dotnet run --project samples/Nestgrid.Response.AspNetCore.Sample
```

Use the included HTTP file:

```text
samples/Nestgrid.Response.AspNetCore.Sample/Nestgrid.Response.AspNetCore.Sample.http
```

## Demonstrates

- Registering Nestgrid response services.
- Returning default full-result responses.
- Returning value-only success responses with per-call options.
- Returning created, not-found, invalid and no-content outcomes.

## Limitations

The sample uses an in-memory singleton service. It is intended to show response conversion, not persistence or authentication.

## Navigation

**Samples**

- [Samples index](../README.md)

**Repository**

- [Nestgrid.Response](../../README.md)
