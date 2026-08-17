# Implementation Plan

```yaml
title: Nestgrid.Response v0.7.0 Implementation Plan
version: 1.2
status: In Review
owner: Software Engineer
contributors:
  - Mason profile
produced_by: Software Engineer
consumed_by: Software Engineer, Quality Engineer, Security Engineer, Platform Engineer
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
related_work_items:
  - IR-004
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../02 Architecture/Architecture Pack.md
  - ../02 Architecture/Engineering Handover.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Scope

This plan covers the approved Engineering retrofit of the existing v0.6.0 Nestgrid.Response implementation for v0.7.0. It includes Architecture conformance checks, the additive TDR-001 validation enhancement, the approved Security follow-on for ADR-008, proportionate tests, package and sample documentation, IDE visibility and downstream handover evidence.

The central package-management change described below is a separate Engineering task from the completed security remediation. This plan amendment is submitted for review before any `Directory.Packages.props` or project-file changes are made.

The work does not add OpenAPI, ProblemDetails, additional adapters, persistence, hosting or a general validation framework.

## Engineering Readiness Assessment

### Readiness Outcome

**Ready with conditions.**

The approved Architecture Pack, Engineering Handover, ADRs, TDR-001 and canonical Independent Review provide sufficient implementation direction. The conditions are to preserve existing public behaviour, keep the validation enhancement additive, confirm compatibility claims from repository evidence, and retain explicit evidence gaps for Quality, Security and Platform.

### Architecture Obligations and Invariants

- Preserve the five-package dependency graph and framework-independent core.
- Keep shared HTTP mapping policy in `Nestgrid.Response.Http`; adapters must not redefine semantics.
- Preserve immutable results, stable statuses, existing factory methods and existing validation conversion behaviour.
- Add TDR-001 member-aware validation conversion without changing existing methods.
- Keep validation member properties and generated codes opt-in because they may expose consumer data.
- Keep MVC actively supported but avoid expanding its support promise beyond evidence in the project and package documentation.
- Keep deferred OpenAPI, `ProblemDetails` and additional adapters out of scope.
- Make exception conversion safe by default while preserving explicit trusted diagnostic conversion.
- Distinguish client-safe, diagnostic and consumer-controlled output in first-party guidance.
- Preserve normative default status mappings and document the security implications of custom mappings.

### Affected Execution Paths

- Existing core result, HTTP mapping and ASP.NET Core/MVC adapter paths are regression paths.
- Existing validation conversion methods are compatibility paths.
- New member-aware validation conversion is the principal implementation path.
- Empty validation collections, memberless results, multiple members, blank member names, custom codes and severity overrides are alternate and failure paths.
- Package creation and sample execution are consumer paths.

### State, Failure and Compatibility Analysis

The product has no persistence, durable state, transactions, concurrency control or startup migration behaviour. Validation conversion is deterministic and in-memory. Existing public methods remain unchanged; new APIs are additive. Null collections and null validation elements continue to fail explicitly with `ArgumentNullException`. Unexpected exceptions remain the consumer application's responsibility.

### Ambiguities and Evidence Gaps

- Exact MVC compatibility and maintenance policy remains an Architecture/Product governance follow-up and must not be inferred beyond current project dependencies.
- Current mutation, coverage, CI and release evidence is not retained in the repository; this remains IR-004 and is handed to Quality.
- Package publication and trusted NuGet execution remain Platform/Release responsibilities.
- Central package management is a repository dependency-governance change and must preserve the versions selected under ADR-007.

## Inputs

- Approved Product Brief.
- Approved Architecture Pack and Engineering Handover.
- ADR-001 through ADR-006.
- Accepted TDR-001.
- Nestgrid Engineering Handbook, Software Engineer role and Mason profile.
- Canonical Nestgrid.Response Independent Review, including IR-004.
- Approved Architecture Security Feedback and Security Assessment, including SEC-001, SEC-004 and SEC-005.
- Existing source, tests, solution, samples and CI workflows.
- ADR-007 Minimum-Compatible Dependency Policy, which governs the package-version centralisation task.

## Solution Structure

The existing five-package solution is retained:

| Area | Responsibility | Dependencies |
| --- | --- | --- |
| `src/Nestgrid.Response` | Framework-independent result model, statuses, messages and functional helpers. | None from the presentation stack. |
| `src/Nestgrid.Response.Http` | Shared HTTP mapping and response-shape policy. | Core. |
| `src/Nestgrid.Response.AspNetCore` | Modern ASP.NET Core execution adapters. | HTTP policy and ASP.NET Core. |
| `src/Nestgrid.Response.Mvc` | MVC compatibility execution adapter. | HTTP policy and MVC Core 2.1.38 baseline. |
| `src/Nestgrid.Response.Extensions.Validation` | DataAnnotations-to-result conversion, including opt-in member-aware conversion. | Core and DataAnnotations. |
| `tests/*` | Responsibility-mirroring automated tests for each package. | Corresponding source package. |
| `samples/*` | Runnable consumer paths for core, validation, ASP.NET Core and MVC. | Relevant packages. |
| `docs/artefacts/03 Implementation` | Engineering plan and handover evidence. | Repository documentation only. |
| `Directory.Packages.props` | Central declaration of NuGet package versions used by source, tests and samples. | MSBuild package-version management only; no runtime dependency. |

## Technology Baseline Alignment

The implementation uses the existing .NET baseline, nullable reference types, implicit usings, XML documentation, xUnit, Shouldly and deterministic package build settings. No new runtime dependency or framework deviation is introduced.

The existing package targets and MVC dependency are retained pending the explicit compatibility evidence requested by Architecture.

## Implementation Principles

- Preserve compatibility before improving convenience.
- Prefer additive, explicit APIs over changing established conversion semantics.
- Keep source and tests organised by responsibility.
- Make consumer-visible assumptions and evidence gaps explicit.
- Keep member-aware validation output opt-in and deterministic.
- Never place exception-derived diagnostics on the normal client-facing result path.
- Centralise package versions without centralising package ownership: projects retain their own `PackageReference Include` declarations.
- Preserve the ADR-007 minimum-compatible versions unless separate advisory or compatibility evidence authorises a change.

## Source and Test Organisation

The new APIs remain in `ValidationResultExtensions`, the existing responsibility owner. Focused tests remain in `ValidationResultExtensionsTests`, covering ordering, member filtering, defaults, overrides and empty input. No broad shared utility or unrelated source bucket is introduced.

## Tooling and IDE Visibility

The solution will expose the root documentation, approved Architecture artefacts, all decisions including TDR-001, the canonical review, and the Engineering README, Implementation Plan and Implementation Report as solution items grouped under their filesystem structure.

## Operationalisation Plan

This is a NuGet library. Engineering will validate Release build, tests, package creation, package README inclusion, sample builds/runs and dependency/target-framework consistency. CI and mutation workflows remain the intended repeatable automation. Publication, credentials, release approval and rollback remain downstream responsibilities.

## Implementation Decisions

| Decision | Location | Notes |
| --- | --- | --- |
| Add member-aware conversion as new extension methods. | TDR-001; source extension class. | Existing conversion methods remain unchanged. |
| Use one message per usable member name, preserving source order. | TDR-001. | Memberless results produce one message. |
| Use `validation_failed` and `The entity is invalid.` defaults. | TDR-001. | Both are override-safe through the code parameter and existing error text. |
| Adopt central package version management as a separate repository change. | Prevents version drift across source, tests and samples while preserving ADR-007’s compatibility policy. | ADR-007; this Implementation Plan |

## Implementation Tasks

| ID | Task | Status |
| --- | --- | --- |
| ENG-001 | Create Engineering artefact folder, README and Implementation Plan. | Completed |
| ENG-002 | Implement TDR-001 additive validation APIs. | Completed |
| ENG-003 | Add focused validation edge-case tests. | Completed |
| ENG-004 | Update package documentation and validation sample. | Completed |
| ENG-005 | Expose approved lifecycle artefacts and TDR-001 in the solution. | Completed |
| ENG-006 | Run build, test, pack, sample and documentation checks. | Completed |
| ENG-007 | Produce Implementation Report and Engineering Assurance. | Completed with conditions |
| ENG-008 | Implement ADR-008 safe exception conversion and explicit diagnostic methods. | Completed |
| ENG-009 | Add security-focused tests and update output/mapping guidance and release notes. | Completed |
| ENG-010 | Introduce root `Directory.Packages.props` and remove project-local package version attributes without changing selected versions. | Planned for review |
| ENG-011 | Verify restore, build, tests, package output, dependency graph and package metadata after centralisation. | Planned |

## Interfaces and Contracts

The new public extension methods are:

```csharp
IEnumerable<ValidationResult>.ToMessagesWithProperties(
    string code = "validation_failed",
    ResultMessageSeverity severity = ResultMessageSeverity.Warning)

IEnumerable<ValidationResult>.ToInvalidResultWithProperties(
    string code = "validation_failed",
    ResultMessageSeverity severity = ResultMessageSeverity.Warning)

IEnumerable<ValidationResult>.ToInvalidResultWithProperties<T>(
    string code = "validation_failed",
    ResultMessageSeverity severity = ResultMessageSeverity.Warning)
```

The approved exception contract is:

```csharp
Results.Error(exception)
Results.Error<T>(exception)
Results.ErrorWithDiagnosticDetails(exception)
Results.ErrorWithDiagnosticDetails<T>(exception)
```

The separate package-management task will retain project-local ownership declarations:

```xml
<PackageReference Include="Shouldly" />
```

with versions maintained in the root `Directory.Packages.props`. The migration must not introduce package upgrades, package downgrades or new dependencies.

## Data Changes

There are no schema, migration, persistence or startup migration changes.

## Testing Approach

- Run all solution tests in Release configuration.
- Add focused unit coverage for all TDR-001 acceptance criteria.
- Add core tests for safe exception output, explicit diagnostic output and null exceptions.
- Add regression coverage for normative security-sensitive HTTP mappings in the shared policy and adapters where practical.
- Build and run each sample project.
- Pack all NuGet projects and inspect package outputs.
- Retain the resulting local verification and identify unavailable CI/mutation evidence for Quality.
- Compare the pre- and post-migration dependency graph and verify that all five packages, tests and samples restore with the same selected versions.

## Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| Member-aware output exposes property names or validation detail. | Medium | Keep it opt-in and document consumer output/privacy responsibility. |
| Public package claims drift from project files. | High | Compare project files, READMEs, Architecture Pack and report evidence. |
| Mutation or release evidence is unavailable locally. | Medium | Retain the limitation and hand IR-004 to Quality. |
| Centralisation accidentally changes a minimum-compatible dependency or transitive graph. | High | Migrate versions mechanically, compare restore assets and package metadata, and require review of the diff before commit. |
| A project-specific package reference is omitted during migration. | Medium | Keep `PackageReference Include` declarations in each owning project and verify the full solution restore/build. |

## Open Questions

- Quality must determine the required mutation, coverage and release-evidence threshold for the final candidate.
- Architecture/Product must maintain the exact MVC support policy and review triggers.
- Review whether any package requires an intentional project-specific version override; any exception must be documented against ADR-007.

## Definition of Done

- TDR-001 is implemented additively and tested.
- Existing tests pass and existing validation behaviour remains unchanged.
- Documentation, sample and solution visibility are updated.
- Implementation Plan and Implementation Report follow the standard templates.
- Engineering Assurance is recorded with evidence limitations and deviations.
- Open findings and downstream obligations are explicitly dispositioned.
- ADR-008 security behaviour and migration guidance are implemented and tested.
- Central package management is implemented only after this plan amendment is reviewed and the dependency graph remains compatible with ADR-007.
