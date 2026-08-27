# Deployment Guide

```yaml
title: Nestgrid.Response v0.8.0 Deployment Guide
version: 1.1
status: Complete with conditions
owner: Platform Engineer
contributors: Knight
produced_by: Platform Engineer
consumed_by: Operations, Project Sponsor, Release Owner, package maintainers
date: 2026-08-27
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
related_work_items:
  - SEC-002
related_repositories:
  - Nestgrid.Response
```

## Scope

This guide covers the six NuGet packages in Nestgrid.Response v0.8.0, including the additive `Nestgrid.Response.Http.Client` package. It does not deploy a hosted process or shared infrastructure. Consumers deploy applications that reference the packages through their own release process.

## Environments

| Environment | Purpose | Notes |
| --- | --- | --- |
| Pull request | Change validation | Build, test, pack and mutation workflows provide automated evidence. |
| `main` | Integration baseline | CI and mutation workflows run on pushes to `main`. |
| NuGet | Public distribution | Publication is triggered by a `v*.*.*` tag through Trusted Publishing. |
| Consumer environment | Runtime use | Configuration, hosting, logging and monitoring belong to the consuming application. |

## Prerequisites

- .NET 8 SDK for repository validation and sample execution.
- Maintainer access to the repository and its protected release/tag process.
- NuGet Trusted Publishing configured for the repository owner.
- A release decision identifying the version and approved evidence.

## Packaging and Publication

The repository builds the solution once in Release configuration and packs all six projects, including symbol packages and package README files. The controlled publication workflow is [`.github/workflows/publish.yml`](../../../.github/workflows/publish.yml).

Publication is tag-driven:

1. Confirm the version in `Directory.Build.props`, release notes and package metadata.
2. Confirm the approved Quality, Security and Platform recommendations are available.
3. Create and push the matching `v*.*.*` tag through the repository release process.
4. Allow the publish workflow to restore, build, test and pack the solution.
5. Retain the workflow run, generated `.nupkg` and `.snupkg` artefacts, and publication result as release evidence.

The workflow uses NuGet Trusted Publishing through the protected `nuget` GitHub Environment. Third-party workflow actions are pinned to immutable commit SHAs, with their release versions retained in comments for maintenance. API credentials must not be copied into source, local configuration, logs or documentation.

## Installation or Consumption

Consumers should install only the package required by their layer:

- `Nestgrid.Response` for framework-independent application or domain code.
- `Nestgrid.Response.Http` plus the appropriate adapter for HTTP mapping.
- `Nestgrid.Response.Http.Client` to interpret responses from a remote Nestgrid.Response endpoint using standard `HttpClient` composition.
- `Nestgrid.Response.AspNetCore` for the supported .NET 8 ASP.NET Core baseline.
- `Nestgrid.Response.Mvc` with `Microsoft.AspNetCore.Mvc.Core` 2.1.38 for the documented MVC baseline.
- `Nestgrid.Response.Extensions.Validation` for DataAnnotations conversion.

The MVC package follows the common Nestgrid.Response library maintenance, versioning, support and release-review lifecycle. It has no separate maintenance lifecycle or independent end-of-support policy.

Before publication, validate the generated packages from a package feed rather than relying only on project references. The minimum matrix is recorded in [Test Strategy](../04%20Quality/Test%20Strategy.md).

## Configuration

Nestgrid.Response has no deployment-time secrets or environment-specific runtime configuration. HTTP response behaviour is configured by the consuming application through `NestgridResponseOptions`.

Consumers own the safe handling of any result messages, property names and response payloads. The opt-in detailed validation conversion may expose member names and validation text and must be reviewed against the consumer's disclosure policy.

## Secrets

The library has no runtime secrets. NuGet Trusted Publishing identity and repository permissions are release infrastructure secrets and must remain managed by GitHub and NuGet controls.

## Deployment Process

1. Review the approved scope, version, compatibility matrix and release notes.
2. Run or confirm Release build, solution tests, package-owned coverage, mutation suites, package inspection and package-consumer smoke validation.
3. Confirm the CI workflow repeats the checks in its supported environment.
4. Create the approved version tag.
5. Monitor the publish workflow and confirm all six packages and symbol packages are published.
6. Verify package pages, version metadata, README content, dependency graphs and install commands.
7. Record the immutable commit, tag, workflow run, package versions and evidence links in the Release Report.

## Service Registration or Runtime Setup

None. Nestgrid.Response is a library. It does not register a service, expose a process, require a health endpoint or require Nestgrid Infrastructure services.

## Upgrade and Uninstall

Consumers should review release notes, update the selected package references together where they share a version, run their own tests, and deploy through their normal application release process. Breaking changes require migration guidance and approval.

To downgrade, pin the previous known-good package version and redeploy the consuming application. To uninstall, remove the package references and any adapter usage, then rebuild and run the consuming application's tests. Unpublishing a NuGet package is not the rollback mechanism.

## Database Changes

None. The product has no persistence, schema, migration or startup database behaviour.

## Health Checks

No product health endpoint is applicable. Deployment validation is package- and consumer-based:

- all six packages restore from the generated package feed;
- representative core, validation, ASP.NET Core and MVC paths compile or execute;
- the HTTP client package restores and its standard `HttpClient` consumer path is validated;
- package metadata, README files and dependency graphs are correct;
- the consuming application's own health and smoke checks pass after it adopts the package.

## Observability

The library must not add a logging, metrics or tracing provider. Publication observability is the GitHub Actions workflow run, uploaded package artefacts and NuGet publication status. Runtime observability is owned by consuming applications; they should apply their normal request, error and dependency telemetry around calls into the library.

## Rollback

If a published package causes consumer failures, stop further adoption, identify the affected package and version, and publish or redeploy consumers using the previous known-good version. Preserve the affected artefact and evidence for investigation. Do not delete or overwrite the published package as a normal rollback action.

## Verification

The release owner should verify:

- the tag and `Directory.Build.props` version agree;
- six `.nupkg` and six `.snupkg` files were produced;
- package READMEs, symbols, target frameworks and dependencies are present;
- the documented MVC baseline remains `Microsoft.AspNetCore.Mvc.Core` 2.1.38;
- the packages can be restored from a clean package source;
- release evidence is linked from the Release Report.

Historical execution evidence confirms that the CI, mutation and publish workflows have completed successfully and that all five v0.6.0 packages have been published to NuGet. Treat this as evidence that the operational path has worked previously; current v0.7.0 release approval still requires the hardened workflow run and current package provenance.

The current v0.8.0 Quality evidence records a successful 380-test Release baseline, package metadata inspection and HTTP client package evidence. The local proving sample has an environment limitation and supported CI remains authoritative for the six-package consumer check, mutation job and protected publication provenance.

## Operational Documentation

The root README, package READMEs, CONTRIBUTING guide, Test Strategy, this guide and the Operational Readiness Review are the support baseline. Consumer issues follow the repository contribution and issue process.

## Operational Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| The protected publication environment is misconfigured | Trusted publication may be unavailable or insufficiently restricted | Retain a successful `nuget`-environment workflow run and review the GitHub/NuGet policy evidence. |
| Tag, project version and release notes diverge | Wrong or ambiguous package version is published | The publication workflow performs an explicit version-consistency check. |
| Publish workflow does not validate package installation from generated packages | A package can publish despite consumer-facing packaging defects | CI and publication run the six-package package-feed consumer smoke matrix. |
| MVC support promise drifts from the common library lifecycle | Consumers may receive inconsistent upgrade signals | Keep the common MVC maintenance, versioning, support and review policy current in the Architecture and package guidance. |
| Consumer runtime telemetry is absent | Library defects may be harder to diagnose in downstream applications | Keep the library provider-neutral and document consumer-side telemetry expectations. |
