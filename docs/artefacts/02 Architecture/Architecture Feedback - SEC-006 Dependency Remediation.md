# Architecture Feedback — SEC-006 Dependency Remediation

```yaml
title: Nestgrid.Response v0.7.0 Architecture Feedback - SEC-006 Dependency Remediation
version: 1.1
status: Approved for Candidate A implementation with conditions
owner: Solution Architect
contributors:
  - Knight
produced_by: Solution Architect
consumed_by: Software Engineer, Security Engineer, Quality Engineer, Project Sponsor
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
related_work_items:
  - SEC-006
  - Q-007
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Pack.md
  - Engineering Handover.md
  - ../05 Security/Security Assessment.md
  - ../05 Security/Security Feedback - Publication and Dependency Controls.md
  - ../04 Quality/Release Readiness Report.md
```

## Purpose

This feedback converts SEC-006 into an implementation-ready architectural handover. It defines the required outcome, evidence, compatibility constraints and escalation boundaries. It does not prescribe package-editing or code-level implementation details.

## Architectural Position

SEC-006 is a P1 release blocker for v0.7.0. The current candidate must not receive final release approval while the reported Critical/High advisories remain unresolved in a supported or published dependency graph.

The default outcome is compatible remediation under [ADR-007](../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md). A dependency must not be upgraded merely because a newer version exists, but a known vulnerable graph cannot be retained without an explicit, authorised exception.

The current findings identify advisories involving:

- `System.Text.Encodings.Web` 4.6.0/4.5.0;
- `Microsoft.AspNetCore.Http` 2.1.1; and
- `Newtonsoft.Json` 9.0.1.

The reported graph must be separated into published package dependencies, supported consumer graphs, and repository-only test or sample dependencies before the remediation scope is finalised.

## Required Engineering Analysis

Engineering must provide a dependency-path matrix for every SEC-006 advisory containing:

| Evidence | Required detail |
| --- | --- |
| Package ownership | Nestgrid package, test project, sample, framework reference or consumer-only graph. |
| Dependency path | Direct or transitive origin, including the package that introduces the advisory. |
| Requested and resolved versions | Exact versions from the evaluated restore graph. |
| Published closure | Whether the dependency appears in each affected generated `.nuspec`/`.nupkg` dependency closure. |
| Runtime context | Target framework and whether the vulnerable component is present at runtime for supported consumers. |
| Remediation | Lowest compatible patched version or an explanation why no compatible version exists. |
| Verification | Restore, advisory, package-content and consumer-installation evidence. |

The analysis must use the repository’s current central package management and supported target-framework declarations as its baseline. It must not infer that a vulnerable version is required merely because it appears in one restore graph.

## Remediation Order

Engineering should evaluate the following outcomes in order:

1. Remove an advisory from the supported or published graph where it is caused only by a test or sample dependency and does not belong to the product package closure.
2. Select the lowest compatible patched dependency version where the dependency is required by a supported package or consumer graph.
3. If the dependency is constrained by the actively supported MVC boundary, identify whether a package-boundary or support-boundary change is required and escalate that change before implementation.
4. Propose an authorised exception only when compatible remediation is unavailable or would require a separately approved product or support decision.

The exception path is not a release waiver by implication. Any proposal must state the affected package and versions, scope, consumer exposure, mitigations, owner, expiry or review date, and the specific decision authority accepting the residual risk. Security must review the proposal before release consideration.

## Compatibility Constraints

- Preserve the actively supported package set and current target-framework promises unless a separate Product/Sponsor decision changes them.
- Prefer patch-level or otherwise compatible remediation that does not change public APIs, package identity or consumer usage.
- Do not change MVC support silently; any change to its supported range, dependency boundary or maintenance promise is an architectural and product decision.
- Do not introduce direct dependency pins solely to suppress an advisory without proving the resulting graph and compatibility.
- Breaking changes remain subject to documented rationale, migration guidance, evidence and approval, even though v0.7.0 is pre-1.0.

## Acceptance Criteria

SEC-006 is ready to return to Security and Quality when:

- the dependency-path matrix is complete;
- every affected published or supported graph is either remediated to a compatible non-vulnerable baseline or covered by an explicitly authorised exception;
- restore and advisory evidence is retained for the final candidate;
- package contents and generated dependency metadata are verified;
- consumer installation paths are tested for affected package families; and
- Engineering records any deviation from this feedback in the Implementation Report.

Security remains responsible for residual-risk assessment. Quality remains responsible for release-readiness evidence. Architecture will record any required exception or support-boundary decision.

## Architecture Approval

Architecture approves Candidate A — explicit lowest-compatible patched dependency pins — as the implementation direction for SEC-006.

This approval is conditional on:

- Security recording its role-owned concurrence with the remediation direction;
- Mason implementing only the evaluated Candidate A dependency changes;
- no change to package identity, target frameworks or the actively supported MVC boundary;
- final MVC `.nuspec` inspection after implementation;
- package-consumer, restore and final advisory evidence being retained; and
- Security completing the final SEC-006 re-review before release consideration.

This is approval to implement the remediation direction. It is not acceptance of residual vulnerability risk, closure of SEC-006 or release approval.

| Approved By | Date | Decision | Conditions |
| --- | --- | --- | --- |
| Solution Architect | 2026-08-17 | Candidate A approved for implementation | Security concurrence and final package/evidence gates remain required. |

## Recommendation

Proceed with Candidate A implementation once Security’s role-owned concurrence is recorded. Do not release or mark SEC-006 accepted until the final package, consumer, advisory and Security evidence requirements above are satisfied.
