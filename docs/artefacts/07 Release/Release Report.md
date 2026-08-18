# Release Report

```yaml
title: Nestgrid.Response v0.7.0 Release Report
version: 1.0
status: Published to NuGet; GitHub Release intentionally deferred until 1.0.0
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

The approved candidate was merged and the `v0.7.0` tag was pushed. The tag-triggered protected GitHub Actions workflow completed the final restore, build, test, pack, verification and NuGet publication in one controlled run. All five packages and their symbol packages were published successfully. This report records the resulting protected-CI and publication evidence.

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
| Release commit | [`ffe5e69e4450863b0f881f47ce8e2a22f624ff59`](https://github.com/nestgrid/Nestgrid.Response/commit/ffe5e69e4450863b0f881f47ce8e2a22f624ff59) | Merged release baseline. |
| Dependency implementation commit | `3c0e151` | Candidate A dependency implementation. |
| Engineering evidence commit | `6a0598fefda587ef283129c51d4f56e611534fa` | Candidate package and implementation evidence baseline. |
| MVC evidence package SHA-256 | `88b250cd14d34a60565489ef78b15b7ec4ef9a60aed97c42ef91a232e14b3d63` | Retained Engineering evidence. |
| Git tag | [`v0.7.0`](https://github.com/nestgrid/Nestgrid.Response/tree/v0.7.0) | Verified tag at the release commit. |
| Protected workflow run | [Publish workflow 32158331286](https://github.com/nestgrid/Nestgrid.Response/actions/runs/32158331286) | Successful; publish job completed in 1m 6s. |
| Publish artifact SHA-256 | `a1eca24cdd83186f566fe3743f18d2cb386b42ed193827bc2b3f3074950eb4df` | Retained from the protected publish workflow artifact. |

The release commit, tag, protected workflow and publication artifact provenance are recorded above. Any source or dependency change after this baseline requires the release evidence to be reassessed.

### Release Evidence Links

| Evidence | Link / result |
| --- | --- |
| Main CI | [Successful CI run 32157570178](https://github.com/nestgrid/Nestgrid.Response/actions/runs/32157570178) |
| Mutation testing | [Successful mutation run 32157570179](https://github.com/nestgrid/Nestgrid.Response/actions/runs/32157570179) — five package suites completed. |
| Protected publication | [Successful publish run 32158331286](https://github.com/nestgrid/Nestgrid.Response/actions/runs/32158331286) — verification, upload, Trusted Publishing and package publication completed. |
| `Nestgrid.Response` 0.7.0 | [NuGet package](https://www.nuget.org/packages/Nestgrid.Response/0.7.0) |
| `Nestgrid.Response.Http` 0.7.0 | [NuGet package](https://www.nuget.org/packages/Nestgrid.Response.Http/0.7.0) |
| `Nestgrid.Response.AspNetCore` 0.7.0 | [NuGet package](https://www.nuget.org/packages/Nestgrid.Response.AspNetCore/0.7.0) |
| `Nestgrid.Response.Extensions.Validation` 0.7.0 | [NuGet package](https://www.nuget.org/packages/Nestgrid.Response.Extensions.Validation/0.7.0) |
| `Nestgrid.Response.Mvc` 0.7.0 | [NuGet package](https://www.nuget.org/packages/Nestgrid.Response.Mvc/0.7.0) |

## Quality Summary

Quality’s current [Release Readiness Report](../04%20Quality/Release%20Readiness%20Report.md) records:

- 289 Release tests passed, with 0 failures and 0 skips;
- package-owned line coverage above the 90% target for all five packages;
- 100% mutation scores for the configured package suites;
- package and supported consumer verification;
- no vulnerable packages in the current source, test and sample advisory scan; and
- SEC-006/Q-007 reconciliation against the retained MVC closure evidence.

Quality’s pre-publication recommendation was conditional on protected publication/provenance evidence. That evidence is now retained in this report; the non-blocking sample endpoint limitation remains open for follow-up.

The remaining Quality follow-up is endpoint-level exercise of web samples, currently treated as non-blocking. It is not represented as completed Release evidence.

## Security Summary

The current [Security Assessment](../05%20Security/Security%20Assessment.md) and [SEC-006 Candidate A Approval](../05%20Security/Security%20Feedback%20-%20SEC-006%20Candidate%20A%20Approval.md) record:

- SEC-001 through SEC-006 resolved or dispositioned for the evaluated candidate;
- SEC-006 remediated using the approved patched dependency pins without changing the MVC support boundary;
- no accepted vulnerability risk;
- safe exception output by default under ADR-008; and
- protected-CI execution and package provenance remaining post-publication evidence conditions.

Security’s pre-publication recommendation was conditional on the tag-triggered workflow and its provenance. The required control completed successfully and its evidence is retained in this report.

## Operational Summary

The [Operational Readiness Review](../06%20Platform/Operational%20Readiness%20Review.md) confirms that the NuGet publication, installation, upgrade, rollback and support model is documented and repeatable in principle.

The protected `nuget` environment, immutable workflow action pins and NuGet Trusted Publishing controls were used successfully. The initial package-verification script issue was corrected before publication. The `v0.7.0` tag-triggered run then verified the packages and consumer installation, authenticated through NuGet Trusted Publishing, and published the five packages and symbol packages.

## Release Recommendations

| Area | Recommendation | Evidence |
| --- | --- | --- |
| Quality | Release evidence is complete for the published packages; retain the non-blocking sample endpoint limitation. | [Release Readiness Report](../04%20Quality/Release%20Readiness%20Report.md) |
| Security | Release evidence is complete; no current vulnerability risk is accepted. | [Security Assessment](../05%20Security/Security%20Assessment.md) |
| Platform | Publication completed through the protected workflow and the resulting provenance is retained. | [Operational Readiness Review](../06%20Platform/Operational%20Readiness%20Review.md) |

## Known Issues and Release Conditions

| Issue | Impact | Mitigation / Acceptance |
| --- | --- | --- |
| GitHub Release was not created for v0.7.0. | The repository has a verified tag and NuGet publication, but no GitHub Release page. | Intentionally deferred by the Project Sponsor until the 1.0.0 public-release convention. The tag, Release Report and NuGet records remain the authoritative release traceability for v0.7.0. |
| GitHub Actions reports Node.js 20 deprecation warnings for actions currently forced to Node.js 24. | The protected CI and publication jobs succeeded, but the action runtime configuration requires maintenance. | Non-blocking follow-up for Platform; update the affected actions before their enforced runtime transition. |
| Web samples have startup evidence but not endpoint-level assertions. | Some adapter response regressions could remain undetected by the current smoke evidence. | Quality treats this as non-blocking; preserve the limitation and address it as follow-up validation if required by the final Release review. |

No residual dependency vulnerability, package-boundary change or MVC support-boundary change is accepted by this report.

## Deployment Details

Deployment is NuGet package publication through the protected GitHub Actions workflow and NuGet Trusted Publishing.

Release/Platform completed:

1. merge the approved candidate to `main`;
2. create and push the immutable `v0.7.0` tag, triggering the protected publication workflow;
3. retain the successful workflow run, commit/tag and package provenance;
4. verify the generated five-package artefacts, NuGet availability, versions, dependency metadata and publication artifact hash; and
5. update this report with the final release identifiers and evidence links.

The tag-triggered workflow published the packages after its validation steps succeeded. A GitHub Release is a separate action and was intentionally deferred for this pre-1.0 release.

## Rollback Position

Rollback is consumer-driven. Consumers should retain or pin the previous known-good package version and redeploy through their normal application release process. Release evidence must retain the v0.7.0 package artefacts and hashes sufficient to identify the published version.

Unpublishing is not the normal rollback mechanism. If the protected workflow or package provenance does not match the approved candidate, publication must stop and the discrepancy must return to Architecture, Security and Platform review.

## Release Decision

The Project Sponsor authorised the merge and tag because the tag triggers the protected publication workflow. Following successful publication and verification, the Project Sponsor decided to defer creation of a GitHub Release until 1.0.0.

| Approved By | Date | Decision | Accepted Risks | Notes |
| --- | --- | --- | --- | --- |
| Knight — Project Sponsor | 2026-08-18 | Proceed with merging the approved candidate and pushing the `v0.7.0` tag, authorising the tag-triggered protected NuGet publication workflow. | None. The absence of pre-tag CI/provenance was a sequencing condition, not an accepted publication risk. | Protected publication completed successfully. GitHub Release creation is intentionally deferred until 1.0.0. |

Release progression is **Published to NuGet with evidence complete; GitHub Release deferred**. A failed workflow or provenance mismatch would require publication follow-up and a return to the relevant role review; neither condition occurred for v0.7.0.
