# Release Report

```yaml
title: Nestgrid.Response v0.7.0 Release Report
version: 1.0
status: Approved for tagged publication; post-publication evidence pending
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

The current evidence supports merging the approved candidate and pushing the `v0.7.0` tag. The tag-triggered protected GitHub Actions workflow performs the final restore, build, test, pack, verification and NuGet publication in one controlled run. Protected-CI execution and immutable publication provenance are therefore post-tag evidence. They must be retained and verified after the workflow succeeds before the Release Report is marked complete.

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
- Publication through any path other than the protected tag-triggered workflow.

## Candidate and Evidence Baseline

| Item | Current value | Position |
| --- | --- | --- |
| Product version | `0.7.0` | Intended release version. |
| Pre-report candidate commit | `d352c0fd06c43eb66c3cae822fac3fc7b3652172` | Current repository evidence baseline before this Release Report commit. |
| Dependency implementation commit | `3c0e151` | Candidate A dependency implementation. |
| Engineering evidence commit | `6a0598fefda587ef283129c51d4f56e611534fa` | Candidate package and implementation evidence baseline. |
| MVC evidence package SHA-256 | `88b250cd14d34a60565489ef78b15b7ec4ef9a60aed97c42ef91a232e14b3d63` | Retained Engineering evidence; final protected-CI package provenance remains outstanding. |
| Git tag | Not yet created | Must be recorded after merge and at publication. |
| Protected workflow run | Not yet executed | Must be retained after the tag-triggered publication workflow. |
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
- protected-CI execution and package provenance remaining post-publication evidence conditions.

Security recommends proceeding with conditions. The tag-triggered workflow remains the required control and its successful execution and provenance must be retained after publication.

## Operational Summary

The [Operational Readiness Review](../06%20Platform/Operational%20Readiness%20Review.md) confirms that the NuGet publication, installation, upgrade, rollback and support model is documented and repeatable in principle.

The protected `nuget` environment, immutable workflow action pins and NuGet Trusted Publishing controls are configured. The current candidate requires one supported-CI execution through that path when the `v0.7.0` tag is pushed. The workflow itself performs publication; its run, commit/tag, package artefacts, hashes and provenance must then be retained.

## Release Recommendations

| Area | Recommendation | Evidence |
| --- | --- | --- |
| Quality | Proceed to merge and tagged publication; complete the Release evidence after the workflow succeeds. | [Release Readiness Report](../04%20Quality/Release%20Readiness%20Report.md) |
| Security | Proceed with conditions; no current vulnerability risk is accepted, and protected-CI provenance must be retained after publication. | [Security Assessment](../05%20Security/Security%20Assessment.md) |
| Platform | Proceed to the tag-triggered protected publication workflow; retain and verify the resulting operational evidence. | [Operational Readiness Review](../06%20Platform/Operational%20Readiness%20Review.md) |

## Known Issues and Release Conditions

| Issue | Impact | Mitigation / Acceptance |
| --- | --- | --- |
| Protected-CI execution and immutable publication provenance are not available before the candidate is pushed to GitHub. | The evidence is necessarily generated by the tag-triggered workflow that also publishes the packages. | This is a sequencing condition, not an accepted publication risk. The Project Sponsor authorises the merge and tag; Platform/Release must retain and verify the workflow and package provenance immediately afterwards. |
| Web samples have startup evidence but not endpoint-level assertions. | Some adapter response regressions could remain undetected by the current smoke evidence. | Quality treats this as non-blocking; preserve the limitation and address it as follow-up validation if required by the final Release review. |

No residual dependency vulnerability, package-boundary change or MVC support-boundary change is accepted by this report.

## Deployment Details

Deployment is NuGet package publication through the protected GitHub Actions workflow and NuGet Trusted Publishing.

Release/Platform must:

1. merge the approved candidate to `main`;
2. create and push the immutable `v0.7.0` tag, which triggers the protected publication workflow;
3. monitor the workflow and retain its run, commit/tag, environment approval and package provenance;
4. verify the generated five-package artefacts, NuGet availability, versions, dependency metadata and hashes; and
5. update this report with the final release identifiers and evidence links.

The tag-triggered workflow publishes the packages after its validation steps succeed. A GitHub Release is a separate action and is not created by the current publication workflow.

## Rollback Position

Rollback is consumer-driven. Consumers should retain or pin the previous known-good package version and redeploy through their normal application release process. Release evidence must retain the v0.7.0 package artefacts and hashes sufficient to identify the published version.

Unpublishing is not the normal rollback mechanism. If the protected workflow or package provenance does not match the approved candidate, publication must stop and the discrepancy must return to Architecture, Security and Platform review.

## Release Decision

The Project Sponsor’s decision authorises the merge and tag because the tag triggers the protected publication workflow. The final report completion remains conditional on retaining and verifying the resulting evidence.

| Approved By | Date | Decision | Accepted Risks | Notes |
| --- | --- | --- | --- | --- |
| Knight — Project Sponsor | 2026-08-18 | Proceed with merging the approved candidate and pushing the `v0.7.0` tag, authorising the tag-triggered protected NuGet publication workflow. | None. The absence of pre-tag CI/provenance is a sequencing condition, not an accepted publication risk. | Retain and verify the protected workflow, package provenance and NuGet publication evidence after the run; then create the GitHub Release and complete this report. |

Release progression is **Approved with post-publication evidence conditions**. The report must be updated after the workflow succeeds with the final merge commit, tag, workflow run, NuGet package links and package hashes. A failed workflow or provenance mismatch requires publication follow-up and a return to the relevant role review.
