# Contributing

Contributions to Nestgrid.Response should keep the public API small, predictable, and framework-independent unless a change belongs to an integration package.

## Prerequisites

- .NET 8 SDK
- Stryker.NET for mutation testing

Install Stryker.NET as a global tool when needed:

```bash
dotnet tool install --global dotnet-stryker
```

## Build

From the solution root:

```bash
dotnet restore
dotnet build Nestgrid.Response.sln --configuration Release --no-restore
```

Warnings are treated as errors.

## Test

The test projects use xUnit and Shouldly:

```bash
dotnet test Nestgrid.Response.sln --configuration Release --no-build
```

Add or update tests for every behavior change. Tests should be deterministic and describe observable behavior.

## Coverage

Collect coverage with Coverlet:

```bash
dotnet test Nestgrid.Response.sln \
  --configuration Release \
  --collect:"XPlat Code Coverage"
```

The production projects are expected to maintain 100% unit test coverage.

## Mutation Testing

Run Stryker separately for each production package:

```bash
dotnet stryker --config-file stryker/stryker-config-core.json
dotnet stryker --config-file stryker/stryker-config-aspnetcore.json
```

Mutation score is expected to remain at 100%.

## Coding Standards

- Follow standard .NET naming and formatting conventions.
- Keep nullable reference types enabled and resolve all warnings.
- Prefer explicit, readable code and immutable public models.
- Use `Results` and `ResultMessages` factories in consumer examples and tests.
- Keep the core package free of presentation and transport concerns.
- Add XML documentation for public APIs.
- Use xUnit for tests and Shouldly for assertions.
- Update relevant documentation when public behavior changes.

See [docs/architecture/coding-standards.md](docs/architecture/coding-standards.md) for the repository standards.

## Pull Requests

Pull requests should:

- Have a focused scope and clear description.
- Explain public API or behavior changes.
- Include tests for new and changed behavior.
- Preserve unit test and mutation coverage expectations.
- Update package documentation and architectural records where required.
- Build without warnings and pass all tests before review.
