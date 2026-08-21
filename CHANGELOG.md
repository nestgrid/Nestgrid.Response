# Changelog

All notable changes to this project will be documented in this file.

## [0.8.0] - 2026-08-21

### Added

- Added the additive `Nestgrid.Response.Http.Client` package for explicit HTTP response interpretation into `Result` and `Result<T>`.
- Added explicit `FullResult` and `ValueOnly` payload modes, immutable client options and client-owned HTTP status mappings.
- Added safe protocol failures for malformed, mismatched and unmapped responses while preserving transport and cancellation exceptions.
- Added reusable licence-service and Portal-to-Finance proving scenarios.

### Changed

- Updated the global package version to `0.8.0` for the additive HTTP client capability.
- Documented response ownership, normal `HttpClient` composition and the distinction between client and server HTTP mapping semantics.

## [0.7.0] - 2026-08-18

### Changed

- Changed `Results.Error(Exception)` and `Results.Error<T>(Exception)` to return the client-safe message `An unexpected error occurred.` without exception-derived details.
- Added explicitly named `ErrorWithDiagnosticDetails` overloads for trusted internal diagnostic workflows.
- Added opt-in member-aware validation conversion in `Nestgrid.Response.Extensions.Validation` through `ToMessagesWithProperties`, `ToInvalidResultWithProperties` and `ToInvalidResultWithProperties<T>`.
- Preserved the existing validation conversion methods and added structured property, code, fallback-message and severity behaviour for the new opt-in APIs.
- Centralised package versions in `Directory.Packages.props` while retaining package ownership in each project.
- Updated the minimum-compatible dependency baseline for the affected graphs: `System.Text.Encodings.Web` `4.7.2`, `Microsoft.AspNetCore.Http` `2.1.22` and `Newtonsoft.Json` `13.0.1`.
- Documented the distinction between client-safe, diagnostic and consumer-controlled output.
- Documented the security implications of custom HTTP status mappings.

### Migration

- Consumers that relied on raw exception messages or type-name codes from `Error(Exception)` must use the explicitly named diagnostic methods only in trusted internal workflows, or provide an intentional safe message through the existing string overloads.

## [0.6.0] - 2026-07-20

### Added

- Documentation index for repository-level documentation.
- Methodology-aligned `docs/handbooks`, `docs/artefacts`, and `docs/decisions` structure.
- Product philosophy handbook.
- Sample README files for all sample applications.
- Package relationship diagram in the root README.
- Package README links to documentation, samples and the main repository.

### Changed

- Reworked the root README around the "lightweight Result pattern library" positioning.
- Improved package READMEs with consistent installation, quick start, realistic examples and feature summaries.
- Moved decision records from `docs/adr` to `docs/decisions`.
- Moved enduring product documentation into `docs/handbooks`.
- Moved roadmap material into `docs/artefacts`.
- Updated roadmap documentation to reflect the current pre-1.0 package set.
- Clarified mutation-testing documentation for all configured Stryker projects and dashboard reporting.
- Improved package metadata consistency for the core package.

### Quality

- Reviewed sample applications for minimal, realistic usage.
- Reviewed markdown navigation, naming consistency and duplicated content.
- No public API changes.

## [0.5.0] - 2026-06-26

### Added

- Initial public NuGet release.
- `Nestgrid.Response`
- `Nestgrid.Response.Http`
- `Nestgrid.Response.AspNetCore`
- `Nestgrid.Response.Mvc`
- `Nestgrid.Response.Extensions.Validation`
- DataAnnotations validation extensions.
- ASP.NET Core and MVC adapters.
- Shared HTTP response mapping.
- Sample applications.
- GitHub Actions CI, mutation testing and Trusted Publishing.
- SourceLink, symbols and package metadata.

### Quality

- 100% mutation score across all packages.
- Comprehensive unit test coverage.
