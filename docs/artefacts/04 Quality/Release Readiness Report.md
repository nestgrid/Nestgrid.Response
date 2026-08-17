# Release Readiness Report

```yaml
title: Nestgrid.Response v0.7.0 Release Quality Recommendation
version: 1.2
status: Complete with conditions
owner: Quality Engineer
produced_by: Quality Engineer
consumed_by: Security Engineer, Platform Engineer, Release Owner, Project Sponsor
date: 2026-08-17
related_findings:
  - IR-004
related_artefacts:
  - Test Strategy.md
  - ../03 Implementation/Implementation Report.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Recommendation

**Quality recommends proceeding to Security, Platform and Release review, but does not recommend release yet.** Current-candidate functional and package verification is complete; unresolved Critical/High dependency advisories remain a release-blocking condition alongside the downstream security and publication conditions below.

This is a conditional quality position, not an acceptance of the open risks and not a production-release decision.

## Current evidence

| Evidence | Result | Assessment |
| --- | --- | --- |
| Quality regression evidence | 289 tests passed, 0 failed, 0 skipped | Current Release run passed |
| Package-owned coverage | Core 100%, HTTP 100%, ASP.NET Core 97.7%, MVC 100%, Validation 100% line coverage | Exceeds the over-90% target |
| Mutation | Core, HTTP, ASP.NET Core, MVC and Validation each reached 100% sequentially | Current evidence complete; concurrent results discarded as contaminated |
| Safe exception contract | Current core and adapter tests cover safe generic and explicit diagnostic paths, including typed and null cases | Quality verification supports ADR-008 behaviour |
| Central package management | `Directory.Packages.props` preserves direct versions, including MVC 2.1.38 | Restore/build completed; advisory evidence remains downstream |
| Package verification | Pack plus `scripts/verify-packages.sh` passed | README and net8.0 consumer restore/build verified |
| Dependency advisory audit | Critical `System.Text.Encodings.Web` 4.6.0/4.5.0, High `Microsoft.AspNetCore.Http` 2.1.1 and High `Newtonsoft.Json` 9.0.1 advisories reported in supported/package-consumer graphs | P1 release blocker pending compatible remediation or authorised exception |
| Security | SEC-001 implementation is reported complete by Engineering, while Security assessment still lists it as blocking | Reconcile through current tests and Security re-review |
| Platform | SEC-002 action pinning/protected environment wiring remains outstanding | Release blocker owned by Platform |
| MVC support policy | `Microsoft.AspNetCore.Mvc.Core` 2.1.38 remains the baseline | No broader MVC version claim made |

## Findings

### Q-004 — P1 — Current-candidate evidence is incomplete

The previous Quality run predates ADR-008 and the central package-management change. Engineering reports 289 passing tests, but current Quality-owned coverage, mutation, package-consumer, dependency-advisory and CI-equivalent evidence is not retained.

**Disposition:** Resolved for Quality. Current regression, coverage, mutation, packaging and consumer verification passed.

### Q-005 — P1 — Security and Quality dispositions are not yet reconciled

Engineering reports ADR-008 implemented and provides safe-default tests, but the current Security Assessment still describes SEC-001 as unresolved and release-blocking. The discrepancy must be resolved with current observable evidence and Security re-review.

**Disposition:** Quality evidence now supports the ADR-008 implementation. Security must update its stale SEC-001 wording and complete its own re-review before release approval.

### Q-006 — P1 — Publication and dependency evidence remain release conditions

SEC-002 remains a Platform-owned release blocker for immutable action references and protected-environment wiring. SEC-003 requires current advisory, restore and package-provenance evidence for the minimum-compatible versions.

**Disposition:** Remains open as a downstream release condition. Platform owns SEC-002; Security/Platform own current advisory and provenance evidence under ADR-007.

### Q-007 — P1 — Current package graphs contain unresolved Critical/High advisories

The current solution audit reported Critical `System.Text.Encodings.Web` 4.6.0 and 4.5.0, High `Microsoft.AspNetCore.Http` 2.1.1, and High `Newtonsoft.Json` 9.0.1. The findings occur in supported package or MVC/sample consumer graphs and therefore cannot be treated as development-only noise. The advisory references reported by the audit are GHSA-ghhp-997w-qr28, GHSA-hxrm-9w7p-39cc and GHSA-5crp-9r3c-p9vr.

**Disposition:** Open release blocker. Architecture, Security and Engineering must determine a compatible dependency remediation that preserves the approved MVC `2.1.38` support baseline, or obtain an explicit authorised risk exception with documented scope and expiry. Quality does not waive ADR-007 or accept the security risk.

## Downstream conditions

1. Security to re-review SEC-001/SEC-004/SEC-005 against the verified ADR-008 behaviour and current documentation.
2. Platform to complete SEC-002 action pinning and protected-environment wiring, then retain immutable publication evidence.
3. Security/Platform to retain current advisory, restore and package-provenance evidence under ADR-007.
4. Architecture/Security/Engineering to resolve Q-007 or record an authorised exception before release.
5. Release to record the final version, evidence, downstream recommendations and any accepted risks.

## Quality gate outcome

Quality **recommends proceeding to downstream release gates but does not recommend release yet**. Quality does not own final production-release approval and does not waive Q-007 or the open Security and Platform conditions.
