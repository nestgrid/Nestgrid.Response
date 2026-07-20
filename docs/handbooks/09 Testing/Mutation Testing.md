# Mutation Testing

> Part of the **[Testing](README.md)** handbook.

Nestgrid.Response uses Stryker.NET to validate the effectiveness of the unit test suite.

Unlike line coverage, mutation testing verifies that tests fail when production code is intentionally changed.

## Current Target

```text
Line Coverage:    100%
Mutation Score:   100%
```

These are project expectations rather than a substitute for review. Tests should still describe observable behavior clearly.

## Tooling

Current test stack:

```text
xUnit 2.9.3
xunit.runner.visualstudio 2.8.x
Microsoft.NET.Test.Sdk 17.x
Stryker.NET 4.14.2
```

xUnit v3 was evaluated during development. The project remains on xUnit 2.9.3 until mutation results are reliable with the selected Stryker.NET configuration.

## Running Mutation Tests

From the solution root:

```bash
dotnet stryker --config-file stryker/stryker-config-core.json
dotnet stryker --config-file stryker/stryker-config-http.json
dotnet stryker --config-file stryker/stryker-config-aspnetcore.json
dotnet stryker --config-file stryker/stryker-config-mvc.json
dotnet stryker --config-file stryker/stryker-config-validation.json
```

Each configuration targets one production package and its matching test project.

## Dashboard Reporting

All Stryker configurations include the `dashboard` reporter:

```json
"reporters": ["html", "progress", "cleartext", "json", "dashboard"]
```

Dashboard publishing is configured through the mutation-testing workflow. Local runs still produce local HTML, cleartext and JSON reports.

## Mutation Thresholds

```json
{
  "high": 100,
  "low": 95,
  "break": 90
}
```

Builds should not be considered complete if the mutation score falls below the configured break threshold.

## Philosophy

A test suite with high line coverage but weak mutation results can still miss important behavior.

Nestgrid.Response aims to maintain both high coverage and high mutation scores so changes remain safe across the core package, HTTP mapping, adapters and validation extensions.

---

## Navigation

**Book**

- [Testing](README.md)

**Documentation**

- [Documentation index](../../README.md)

**Repository**

- [Nestgrid.Response](../../../README.md)
