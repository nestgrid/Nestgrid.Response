# Implementation Plan

```yaml
title: Nestgrid.Response v0.7.0 Implementation Plan
version: 1.5
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
  - ../02 Architecture/Architecture Feedback - SEC-006 Dependency Remediation.md
related_work_items:
  - IR-004
  - SEC-006
  - Q-007
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../02 Architecture/Architecture Pack.md
  - ../02 Architecture/Engineering Handover.md
  - ../../reviews/Nestgrid.Response Independent Review.md
  - SEC-006 Dependency Path Matrix.md
  - ../05 Security/Security Feedback - SEC-006 Candidate A Approval.md
```

## Scope

This plan covers the approved Engineering retrofit of the existing v0.6.0 Nestgrid.Response implementation for v0.7.0. It includes Architecture conformance checks, the additive TDR-001 validation enhancement, the approved Security follow-on for ADR-008, proportionate tests, package and sample documentation, IDE visibility and downstream handover evidence.

The central package-management change described below was a separate Engineering task from the completed security remediation. This amendment adds the SEC-006 dependency analysis and remediation planning task. Security has approved Candidate A for implementation with conditions recorded in the Security Feedback artefact; the approval does not close SEC-006 or approve release.

The work does not add OpenAPI, ProblemDetails, additional adapters, persistence, hosting or a general validation framework. SEC-006 remediation must not silently change the actively supported MVC boundary or introduce a new product capability.

## Engineering Readiness Assessment

### Readiness Outcome

**Ready with conditions.**

The approved Architecture Pack, Engineering Handover, ADR-007, the approved SEC-006 Architecture Feedback and canonical Independent Review provide sufficient direction to prepare a remediation proposal. The conditions are to preserve public behaviour and supported target-framework promises by default, classify package closure accurately, evaluate the lowest compatible patched versions, and obtain Architecture and Security approval before implementation. The plan itself is not approval to change dependencies.

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

- Exact MVC compatibility and maintenance policy remains an Architecture/Product governance follow-up and must not be inferred beyond current project dependencies; Candidate A approval preserves the currently approved `2.1.38` boundary.
- Current mutation, coverage, CI and release evidence is not retained in the repository; this remains IR-004 and is handed to Quality.
- Package publication and trusted NuGet execution remain Platform/Release responsibilities.
- Central package management is a repository dependency-governance change and must preserve the versions selected under ADR-007.
- The patched dependency baseline is selected for Candidate A; the authority for any MVC support-boundary change remains an explicit Architecture/Product decision gate.

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
- Approved Architecture Feedback — SEC-006 Dependency Remediation.
- SEC-006 Dependency Path Matrix, which records the pre-remediation graph classification and evidence gaps.

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

For SEC-006, the current dependency baseline is evidence only. Engineering will not upgrade, downgrade, override or remove a dependency until the candidate graph has been reviewed against ADR-007, the approved MVC support boundary and Security’s advisory findings.

## Implementation Principles

- Preserve compatibility before improving convenience.
- Prefer additive, explicit APIs over changing established conversion semantics.
- Keep source and tests organised by responsibility.
- Make consumer-visible assumptions and evidence gaps explicit.
- Keep member-aware validation output opt-in and deterministic.
- Never place exception-derived diagnostics on the normal client-facing result path.
- Centralise package versions without centralising package ownership: projects retain their own `PackageReference Include` declarations.
- Preserve the ADR-007 minimum-compatible versions unless separate advisory or compatibility evidence authorises a change.
- Treat dependency remediation as a compatibility decision, not a recency upgrade.
- Inspect published package closure separately from repository-only restore graphs.
- Prefer the smallest compatible change and escalate support-boundary changes before implementation.

## Source and Test Organisation

The new APIs remain in `ValidationResultExtensions`, the existing responsibility owner. Focused tests remain in `ValidationResultExtensionsTests`, covering ordering, member filtering, defaults, overrides and empty input. No broad shared utility or unrelated source bucket is introduced.

## Tooling and IDE Visibility

The solution will expose the root documentation, `Directory.Build.props`, `Directory.Packages.props`, approved Architecture artefacts, all decisions including TDR-001, the canonical review, and the Engineering README, Implementation Plan and Implementation Report as solution items grouped under their filesystem structure.

## Operationalisation Plan

This is a NuGet library. Engineering will validate Release build, tests, package creation, package README inclusion, sample builds/runs and dependency/target-framework consistency. CI and mutation workflows remain the intended repeatable automation. Publication, credentials, release approval and rollback remain downstream responsibilities.

## Implementation Decisions

| Decision | Location | Notes |
| --- | --- | --- |
| Add member-aware conversion as new extension methods. | TDR-001; source extension class. | Existing conversion methods remain unchanged. |
| Use one message per usable member name, preserving source order. | TDR-001. | Memberless results produce one message. |
| Use `validation_failed` and `The entity is invalid.` defaults. | TDR-001. | Both are override-safe through the code parameter and existing error text. |
| Adopt central package version management as a separate repository change. | Prevents version drift across source, tests and samples while preserving ADR-007’s compatibility policy. | ADR-007; this Implementation Plan |
| Implement only the Security-approved SEC-006 Candidate A direction. | Dependency changes may alter published closure, supported MVC compatibility or consumer behaviour. | SEC-006 Dependency Path Matrix; Architecture Feedback; Security Feedback - SEC-006 Candidate A Approval |
| Evaluate remediation in this order: remove test/sample-only exposure, select the lowest compatible patched version, escalate support-boundary change, then consider a scoped exception. | Follows the approved Architecture Feedback and preserves compatibility by default. | Architecture Feedback - SEC-006 Dependency Remediation |
| Recommend Candidate A: explicit lowest-compatible transitive pins, subject to compatibility proof. | Addresses the reported advisories while preserving package identity, target frameworks and the approved MVC 2.1.38 parent boundary. | SEC-006 Dependency Path Matrix |

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
| ENG-010 | Introduce root `Directory.Packages.props` and remove project-local package version attributes without changing selected versions. | Completed |
| ENG-011 | Verify restore, build, tests, package output, dependency graph and package metadata after centralisation. | Completed |
| ENG-012 | Produce the SEC-006 baseline dependency-path matrix and classify published, supported and repository-only graphs. | Completed for review |
| ENG-013 | Obtain Architecture and Security review of the SEC-006 plan, matrix and remediation options before implementation. | Security review completed — Candidate A approved with conditions; deviations remain subject to Architecture authority |
| ENG-014 | Implement the approved compatible remediation or authorised exception path. | Completed — Candidate A pins implemented |
| ENG-015 | Re-verify advisory, restore, package closure, consumer installation and downstream handover evidence. | In progress — advisory, restore, build, test and three-package consumer evidence complete; MVC archive and MVC consumer evidence outstanding |

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

SEC-006 remediation is intentionally not yet a contract change. Candidate changes must be assessed for public API, target-framework, package identity, dependency closure and MVC consumer compatibility before selection.

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
- Produce baseline and candidate advisory evidence for every reported SEC-006 package.
- Pack all five packages from the candidate graph and inspect generated dependency metadata.
- Install affected package families from a local package source in representative supported consumer projects.
- Re-run the full regression suite and mutation/coverage checks required by Quality after remediation.

## Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| Member-aware output exposes property names or validation detail. | Medium | Keep it opt-in and document consumer output/privacy responsibility. |
| Public package claims drift from project files. | High | Compare project files, READMEs, Architecture Pack and report evidence. |
| Mutation or release evidence is unavailable locally. | Medium | Retain the limitation and hand IR-004 to Quality. |
| Centralisation accidentally changes a minimum-compatible dependency or transitive graph. | High | Migrate versions mechanically, compare restore assets and package metadata, and require review of the diff before commit. |
| A project-specific package reference is omitted during migration. | Medium | Keep `PackageReference Include` declarations in each owning project and verify the full solution restore/build. |
| A direct override hides rather than removes a vulnerable transitive dependency. | High | Require dependency-path and generated package metadata inspection for every candidate. |
| Remediation changes the supported MVC boundary or consumer behaviour. | High | Escalate the boundary decision to Architecture/Product/Sponsor before implementation. |
| A residual-risk exception becomes an implicit release waiver. | High | Require explicit scope, owner, expiry, mitigation and Security review; no blanket acceptance. |

## Open Questions

- Quality must determine the required mutation, coverage and release-evidence threshold for the final candidate.
- Architecture/Product must maintain the exact MVC support policy and review triggers.
- Review whether any package requires an intentional project-specific version override; any exception must be documented against ADR-007.
- Which patched versions are the lowest compatible choices for each advisory and target framework?
- Is Candidate A’s explicit transitive-pin approach acceptable under ADR-007 when supported by package-closure and compatibility evidence?
- What retained evidence will close the remaining fresh MVC `.nuspec` generation limitation before SEC-006 closure?
- Does MVC `2.1.38` have a compatible remediation path that preserves the approved active support promise?
- If not, which authority will decide between a support-boundary change and a time-limited exception?
- Which CI/advisory/provenance evidence must be retained for the final candidate?

## Definition of Done

- TDR-001 is implemented additively and tested.
- Existing tests pass and existing validation behaviour remains unchanged.
- Documentation, sample and solution visibility are updated.
- Implementation Plan and Implementation Report follow the standard templates.
- Engineering Assurance is recorded with evidence limitations and deviations.
- Open findings and downstream obligations are explicitly dispositioned.
- ADR-008 security behaviour and migration guidance are implemented and tested.
- Central package management is implemented and verified; the dependency graph remains compatible with ADR-007.
- SEC-006 matrix, remediation proposal and evidence requirements are reviewed by Architecture and Security before any implementation.
- Approved dependency remediation or authorised exception is implemented and documented.
- Final package closures, supported consumer installations, advisory results and downstream dispositions are retained.
