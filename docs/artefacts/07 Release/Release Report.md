# Release Report

```yaml
title: Nestgrid.Response v0.7.0 Release Report
version: 1.0
status: Approved for protected-CI execution; final release decision pending
owner: Project Sponsor
contributors:
  - Solution Architect
  - Quality Engineer
  - Security Engineer
  - Platform Engineer
  - Software Engineer
produced_by: Solution Architect for Project Sponsor approval
consumed_by: Project Sponsor, Stakeholders, Operations, Software Engineer
date: 2026-08-18
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
  - IR-009
  - Q-006
  - Q-007
  - SEC-002
  - SEC-003
  - SEC-006
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../04 Quality/Release Readiness Report.md
  - ../05 Security/Security Assessment.md
  - ../06 Platform/Operational Readiness Review.md
  - ../03 Implementation/Implementation Report.md
  - ../03 Implementation/SEC-006 Closure Evidence.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Release Summary

Nestgrid.Response v0.7.0 is the controlled release of the existing v0.6.0 five-package product following its retrofit into the Nestgrid Engineering Operating System.

The candidate preserves the approved package identities, target frameworks, result model, HTTP adapter boundaries and actively supported MVC support boundary. It includes the approved additive validation conversion, the ADR-008 safe exception-conversion correction, central package-version management and the SEC-006 Candidate A dependency remediation.

The current evidence supports progression to the protected GitHub Actions execution. Protected-CI execution and immutable publication provenance cannot be produced before the candidate is pushed to GitHub. That evidence gap is explicitly accepted for this pre-execution decision only; it is not accepted as permission to publish without the protected workflow evidence.

## Scope

- Existing five-package Nestgrid.Response product baseline.
- Additive member-aware DataAnnotations validation conversion.
- Safe-by-default exception conversion under ADR-008, with explicit diagnostic methods.
- Central package-version management under ADR-007.
- SEC-006 Candidate A pins:
  - `System.Text.Encodings.Web` 4.7.2;
  - `Microsoft.AspNetCore.Http` 2.1.22; and
  - `Newtonsoft.Json` 13.0.1.
- Updated package documentation, samples, release notes, lifecycle artefacts and solution visibility.

## Exclusions

- OpenAPI support.
- `ProblemDetails` support.
- Additional adapters.
- Persistence, hosting, authentication or workflow features.
- A change to the active MVC support boundary.
- Final publication before protected-CI execution and provenance retention.

## Candidate and Evidence Baseline

| Item | Current value | Position |
| --- | --- | --- |
| Product version | `0.7.0` | Intended release version. |
| Pre-report candidate commit | `d352c0fd06c43eb66c3cae822fac3fc7b3652172` | Current repository evidence baseline before this Release Report commit. |
| Dependency implementation commit | `3c0e151` | Candidate A dependency implementation. |
| Engineering evidence commit | `6a0598fefda587ef283129c51d4f56e611534fa` | Candidate package and implementation evidence baseline. |
| MVC evidence package SHA-256 | `88b250cd14d34a60565489ef78b15b7ec4ef9a60aed97c42ef91a232e14b3d63` | Retained Engineering evidence; final protected-CI package provenance remains outstanding. |
| Git tag | Not yet created | Must be recorded after Sponsor approval and before publication. |
| Protected workflow run | Not yet executed | Required before final release approval. |
| Published package hashes | Not yet available | Must be retained from the protected workflow. |

The final release tag and commit must be recorded in this report after the Release Report commit is included in the approved release baseline. Any source or dependency change after the recorded candidate commit requires the release evidence to be reassessed.

## Quality Summary

Quality’s current [Release Readiness Report](../04%20Quality/Release%20Readiness%20Report.md) records:

- 289 Release tests passed, with 0 failures and 0 skips;
- package-owned line coverage above the 90% target for all five packages;
- 100% mutation scores for the configured package suites;
- package and supported consumer verification;
- no vulnerable packages in the current source, test and sample advisory scan; and
- SEC-006/Q-007 reconciliation against the retained MVC closure evidence.

Quality recommends proceeding to the downstream Release gate but defers final release approval until protected publication/provenance evidence is retained and the final dispositions are recorded.

The remaining Quality follow-up is endpoint-level exercise of web samples, currently treated as non-blocking. It must not be represented as completed Release evidence unless executed.

## Security Summary

The current [Security Assessment](../05%20Security/Security%20Assessment.md) and [SEC-006 Candidate A Approval](../05%20Security/Security%20Feedback%20-%20SEC-006%20Candidate%20A%20Approval.md) record:

- SEC-001 through SEC-006 resolved or dispositioned for the evaluated candidate;
- SEC-006 remediated using the approved patched dependency pins without changing the MVC support boundary;
- no accepted vulnerability risk;
- safe exception output by default under ADR-008; and
- protected-CI execution and package provenance remaining release conditions.

Security recommends proceeding with conditions. This report does not convert the protected-CI evidence gap into a security-risk acceptance or close the operational evidence conditions.

## Operational Summary

The [Operational Readiness Review](../06%20Platform/Operational%20Readiness%20Review.md) confirms that the NuGet publication, installation, upgrade, rollback and support model is documented and repeatable in principle.

The protected `nuget` environment, immutable workflow action pins and NuGet Trusted Publishing controls are configured. The current candidate still requires one supported-CI execution through that path, with the workflow run, immutable commit/tag, package artefacts, hashes and provenance retained.

## Release Recommendations

| Area | Recommendation | Evidence |
| --- | --- | --- |
| Quality | Proceed to protected-CI and Release execution; defer final publication approval until provenance is retained. | [Release Readiness Report](../04%20Quality/Release%20Readiness%20Report.md) |
| Security | Proceed with conditions; no current vulnerability risk is accepted, but protected-CI provenance remains required. | [Security Assessment](../05%20Security/Security%20Assessment.md) |
| Platform | Proceed to Release review and protected workflow execution; retain current-candidate operational evidence before publication. | [Operational Readiness Review](../06%20Platform/Operational%20Readiness%20Review.md) |

## Known Issues and Accepted Pre-Execution Condition

| Issue | Impact | Mitigation / Acceptance |
| --- | --- | --- |
| Protected-CI execution and immutable publication provenance are not available before the candidate is pushed to GitHub. | The hardened publication controls have not yet been demonstrated for this v0.7.0 candidate, so final publication is not yet auditable. | The Project Sponsor accepts this as a known pre-execution evidence gap for the purpose of pushing the candidate and executing the protected workflow. Platform/Release must execute the workflow and retain the evidence before publication. This is not acceptance of publication without evidence. |
| Web samples have startup evidence but not endpoint-level assertions. | Some adapter response regressions could remain undetected by the current smoke evidence. | Quality treats this as non-blocking; preserve the limitation and address it as follow-up validation if required by the final Release review. |

No residual dependency vulnerability, package-boundary change or MVC support-boundary change is accepted by this report.

## Deployment Details

Deployment is NuGet package publication through the protected GitHub Actions workflow and NuGet Trusted Publishing.

Before publication, Release/Platform must:

1. push the approved candidate and create the immutable release tag;
2. execute the protected publication workflow;
3. retain the workflow run, commit/tag, environment approval and package provenance;
4. verify the generated five-package artefacts, versions, dependency metadata and hashes; and
5. update this report with the final release identifiers and evidence links.

No package has been approved for publication by this report yet.

## Rollback Position

Rollback is consumer-driven. Consumers should retain or pin the previous known-good package version and redeploy through their normal application release process. Release evidence must retain the v0.7.0 package artefacts and hashes sufficient to identify the published version.

Unpublishing is not the normal rollback mechanism. If the protected workflow or package provenance does not match the approved candidate, publication must stop and the discrepancy must return to Architecture, Security and Platform review.

## Release Decision

The Project Sponsor’s current decision is limited to progression into protected-CI execution.

| Approved By | Date | Decision | Accepted Risks | Notes |
| --- | --- | --- | --- | --- |
| Knight — Project Sponsor | 2026-08-18 | Proceed to push the approved v0.7.0 candidate and execute the protected-CI workflow; final release publication decision deferred. | Pre-execution absence of protected-CI and immutable provenance evidence. | The accepted condition must be closed by retained supported-CI evidence before publication. This is not final release approval. |

Final release approval remains **Pending** until the protected workflow has executed successfully, package provenance has been retained, all release findings have been dispositioned and the Project Sponsor records a final Proceed or Stop decision in this report.
