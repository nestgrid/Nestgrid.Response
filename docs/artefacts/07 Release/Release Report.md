# Release Report

```yaml
title: Nestgrid.Response v0.8.0 Release Report
version: 1.1
status: Published to NuGet; post-publication evidence recorded; GitHub Release deferred until 1.0.0
owner: Project Sponsor
contributors:
  - Solution Architect
  - Quality Engineer
  - Security Engineer
  - Platform Engineer
  - Software Engineer
produced_by: Solution Architect for Project Sponsor approval
consumed_by: Project Sponsor, Stakeholders, Operations, Software Engineer
date: 2026-08-28
supersedes: Release Report v1.0 for v0.7.0
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
  - ../../decisions/ADR-012-HTTP-Client-Safety-Boundaries.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
  - IR-016
  - Q-HTTP-002
  - SEC-002
  - SEC-003
  - SEC-006
  - SEC-007
  - SEC-008
  - SEC-009
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../04 Quality/Release Readiness Report - HTTP Client Capability.md
  - ../05 Security/Security Assessment.md
  - ../06 Platform/Operational Readiness Review.md
  - ../03 Implementation/Implementation Report - HTTP Client Capability.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Release Summary

Nestgrid.Response v0.8.0 is the controlled six-package release of the existing
v0.7.0 five-package product. It adds the approved `Nestgrid.Response.Http.Client`
adapter without changing the existing core, HTTP mapping, ASP.NET Core, MVC or
validation package boundaries.

The Project Sponsor approved the release. The immutable `v0.8.0` tag was created
against the approved release commit, the protected publication workflow
completed successfully, and all six packages and their symbol packages were
published to NuGet.

## Scope

- Existing five-package Nestgrid.Response product baseline.
- Additive `Nestgrid.Response.Http.Client` package targeting `netstandard2.0`.
- Explicit `FullResult` and `ValueOnly` payload modes.
- Client-side HTTP outcome mapping, protocol-safe failures and bounded response reading.
- Cancellation-aware content reading and response ownership guarantees.
- Package documentation, samples, solution visibility and release evidence.

OpenAPI, `ProblemDetails`, additional adapters, authentication, resilience,
telemetry and hosting remain outside the approved v0.8.0 scope.

## Release Candidate and Publication Evidence

| Item | Evidence |
| --- | --- |
| Product version | `0.8.0` |
| Release commit | [`8bd19c20500c8a69e55be7310daf1878b98e1d52`](https://github.com/nestgrid/Nestgrid.Response/commit/8bd19c20500c8a69e55be7310daf1878b98e1d52) |
| Git tag | [`v0.8.0`](https://github.com/nestgrid/Nestgrid.Response/tree/v0.8.0) |
| Protected publication workflow | [Publish workflow 33198249181](https://github.com/nestgrid/Nestgrid.Response/actions/runs/33198249181) |
| GitHub packages artefact digest | `sha256:50043bd498ed2beb59ec97a95ed3ec051ea1ff33c7a92e7845c55afcc1dccdae` |
| Main CI | Successful on commit `8bd19c2`; screenshot evidence supplied by the Project Sponsor |
| Mutation testing | Successful six-job matrix, including `http-client`; screenshot evidence supplied by the Project Sponsor |

The protected publish workflow succeeded after validating, packing and uploading
the candidate. The GitHub Actions warning about Node.js 20 deprecation did not
fail the workflow and is retained as a Platform follow-up.

## Published Package Evidence

| Package | NuGet package | SHA-256 |
| --- | --- | --- |
| `Nestgrid.Response` | [0.8.0](https://www.nuget.org/packages/Nestgrid.Response/0.8.0) | `4568503f2dccd62384a48370924523242837629748e7737afbc3efdd76d5b1c7` |
| `Nestgrid.Response.Http` | [0.8.0](https://www.nuget.org/packages/Nestgrid.Response.Http/0.8.0) | `3e3b1f4fdeb731abe3d2ab40470e2c344386d326f58f0e9c67ea8c224d5f954c` |
| `Nestgrid.Response.AspNetCore` | [0.8.0](https://www.nuget.org/packages/Nestgrid.Response.AspNetCore/0.8.0) | `93dcbbe59736bd55d4bcd130b684015248432b42a3042ca8f032f56a89c2dbdb` |
| `Nestgrid.Response.Extensions.Validation` | [0.8.0](https://www.nuget.org/packages/Nestgrid.Response.Extensions.Validation/0.8.0) | `6871189bba243b0b128248e3f484f3c3cebba774f057374ea906a993ad49c1e5` |
| `Nestgrid.Response.Mvc` | [0.8.0](https://www.nuget.org/packages/Nestgrid.Response.Mvc/0.8.0) | `ec14cd59140cab873be9863b147d895ded03b544b8273265a9054b9713bab15a` |
| `Nestgrid.Response.Http.Client` | [0.8.0](https://www.nuget.org/packages/Nestgrid.Response.Http.Client/0.8.0) | `306cff6e6da3cb39f1fb34705a5fbc8307f5e6030009604c4d0eb952b81a278c` |

The six package pages show `0.8.0` as the latest published version. The
package-level hashes identify the published `.nupkg` files; the GitHub Actions
digest identifies the retained publication artefact bundle.

## Quality, Security and Platform Summary

Quality evidence records 380 passing Release tests, including 91 HTTP client
tests, 97.95% client line coverage, 95.90% branch coverage and 90.85% client
mutation effectiveness. The GitHub CI and six-job mutation workflows also
completed successfully on the release commit.

Security records SEC-007, SEC-008 and SEC-009 as resolved for the evaluated
candidate, with no accepted vulnerability risk. Platform confirms that the
protected NuGet publication path operated successfully for this release.

The full role-owned assessments remain authoritative for their detailed
conditions: [Quality](../04%20Quality/Release%20Readiness%20Report%20-%20HTTP%20Client%20Capability.md), [Security](../05%20Security/Security%20Assessment.md) and [Platform](../06%20Platform/Operational%20Readiness%20Review.md).

## Release Decision

| Approved By | Date | Decision | Accepted Risks | Notes |
| --- | --- | --- | --- | --- |
| Knight — Project Sponsor | 2026-08-28 | Approve v0.8.0 publication through the protected tag-triggered workflow. | No security or dependency risk accepted. | Publication completed successfully; package hashes, commit, tag, workflow and artefact digest are recorded. |

Release progression is **Published to NuGet with post-publication evidence
recorded**. The GitHub Release page remains intentionally deferred until the
1.0.0 public-release convention.

## Outstanding Follow-up

These are not release blockers after the successful publication, but remain
visible for normal follow-up:

1. Add endpoint-level assertions or repeat the proving sample where stronger
   sample evidence is required; the current CI workflow proves package
   installation but does not itself provide live endpoint evidence.
2. Update actions affected by the Node.js 20 deprecation warning before the
   enforced runtime transition.
3. Re-review the canonical Independent Review to record closure of IR-016
   against this Release Report and retain the final publication evidence.
4. Resolve the pre-1.0 API findings IR-011 through IR-015 and IR-018 before
   the 1.0.0 API freeze.

Rollback remains consumer-driven: consumers should pin the previous known-good
version and redeploy through their normal release process. Unpublishing is not
the normal rollback mechanism.
