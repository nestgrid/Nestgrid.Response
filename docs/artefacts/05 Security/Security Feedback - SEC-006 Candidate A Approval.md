# Security Feedback

```yaml
title: Nestgrid.Response v0.7.0 Security Feedback - SEC-006 Candidate A Approval
version: 1.0
status: Approved
owner: Software Engineer
contributors:
  - Security Engineer
produced_by: Security Engineer
consumed_by: Solution Architect, Software Engineer, Quality Engineer, Platform Engineer, Project Sponsor
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
related_work_items:
  - SEC-006
  - Q-007
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../03 Implementation/Implementation Plan.md
  - ../03 Implementation/SEC-006 Dependency Path Matrix.md
  - ../02 Architecture/Architecture Feedback - SEC-006 Dependency Remediation.md
  - Security Assessment.md
```

## Context

The Software Engineer completed the SEC-006 Candidate A investigation in an isolated worktree and updated the Implementation Plan and Dependency Path Matrix. No Candidate A dependency changes have been made on the product branch. The review concerns permission to implement the approved remediation direction, not final SEC-006 closure or release approval.

## Feedback Summary

Security approves Candidate A for implementation. The proposal uses the lowest identified patched versions while preserving package identities, target frameworks and the approved MVC `2.1.38` parent/support boundary. The evidence reported so far is proportionate to approve implementation, but the remaining package metadata and final consumer evidence must be completed before SEC-006 can be closed.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-006 | P1 | The baseline supported/package-consumer graphs contain known vulnerable dependencies. Candidate A identifies a compatible remediation path. | Retaining the baseline would expose consumers to known vulnerable dependency versions. | Implement Candidate A under the conditions below, then complete the evidence gate before closure or release. |
| SEC-006-A | P2 | Fresh MVC package archive/`.nuspec` inspection remains outstanding because isolated MSBuild pack generation hung. | The exact published MVC dependency metadata has not yet been independently confirmed. | Retain fresh MVC `.nuspec` evidence, or document an equivalent authoritative package-metadata inspection, before SEC-006 closure. |

## Blocking Issues

- The following exact Candidate A pins must be used unless the work returns to Architecture and Security for review: `System.Text.Encodings.Web 4.7.2`, `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1`.
- `Microsoft.AspNetCore.Mvc.Core 2.1.38`, package identities, target frameworks and the approved MVC support boundary must be preserved.
- Candidate B or any MVC parent/support-boundary change is outside this approval and requires the appropriate Architecture/Product authority before implementation.
- Before SEC-006 closure, retain fresh restore and advisory evidence, inspect generated metadata for all five published packages including MVC, verify supported consumer restore/install/execute paths, and record final dependency graphs and regression results.
- If a vulnerable path remains, compatibility changes, or the final metadata differs materially from the reviewed Candidate A, stop and return the deviation to Architecture and Security. No implicit exception is granted.

## Non-blocking Issues

- Protected-environment publication evidence, package provenance and SEC-003 evidence remain separate Platform/Release conditions.
- The isolated MSBuild pack limitation may be resolved through a repeatable equivalent inspection, provided the evidence is retained and the MVC published closure is authoritative.

## Questions

- Which retained build/package evidence will demonstrate the final MVC `.nuspec` and all five published dependency closures?
- Which supported consumer restore/install/execute checks will be attached to the SEC-006 handover?

## Recommendation

Security approves Mason to begin Candidate A implementation within the conditions above. This is an implementation approval only. SEC-006 remains open and release-blocking until the final evidence gate is satisfied or an explicitly authorised exception is recorded by the responsible authorities.
