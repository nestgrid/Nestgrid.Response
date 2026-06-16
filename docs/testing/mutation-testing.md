# Mutation Testing

Nestgrid.Response uses Stryker.NET to validate the effectiveness of the unit test suite.

Unlike traditional code coverage, mutation testing verifies that tests fail when production code is intentionally modified.

## Current Results

```text
Line Coverage:    100%
Mutation Score:   100%
```

## Why Mutation Testing?

Code coverage only proves that code was executed.

Mutation testing proves that tests validate behaviour.

For example:

```csharp
if (exception is null)
{
    throw new ArgumentNullException(nameof(exception));
}
```

A mutation test may remove this check and verify that the test suite detects the behavioural change.

## Tooling

Current test stack:

```text
xUnit 2.9.3
xunit.runner.visualstudio 2.8.x
Microsoft.NET.Test.Sdk 17.x
Stryker.NET 4.14.2
```

## Notes

xUnit v3 was evaluated during development.

At the time of writing, Stryker.NET did not produce reliable mutation results with the selected xUnit v3 configuration.

The project therefore remains on xUnit 2.9.3 until tooling support matures and can be re-evaluated.

## Running Mutation Tests

From the solution root:

```bash
dotnet stryker --config-file stryker/stryker-config-core.json
dotnet stryker --config-file stryker/stryker-config-aspnetcore.json
```

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

A test suite with:

```text
100% Coverage
0% Mutation Score
```

is not a high-quality test suite.

Nestgrid.Response aims to maintain both high coverage and high mutation scores to ensure behaviour is thoroughly verified.
