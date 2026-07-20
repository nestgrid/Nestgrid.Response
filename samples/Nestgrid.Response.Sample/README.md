# Nestgrid.Response.Sample

Console sample for the core Nestgrid.Response package.

## Purpose

This sample demonstrates result factories, messages, `Map()`, and `Match()` without involving HTTP or validation packages.

## Run

From the repository root:

```bash
dotnet run --project samples/Nestgrid.Response.Sample
```

## Demonstrates

- Creating successful and non-success `Result<T>` values.
- Printing statuses, values and messages.
- Mapping a domain value to a DTO.
- Matching success and failure flows.

## Limitations

The sample uses in-memory records and console output so the result behavior stays visible.

## Navigation

**Samples**

- [Samples index](../README.md)

**Repository**

- [Nestgrid.Response](../../README.md)
