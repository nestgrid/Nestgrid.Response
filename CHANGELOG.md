# Changelog

All notable changes to this project will be documented in this file.

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
