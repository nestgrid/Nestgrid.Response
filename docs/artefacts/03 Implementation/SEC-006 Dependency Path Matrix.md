# SEC-006 Dependency Path Matrix

```yaml
title: Nestgrid.Response v0.7.0 SEC-006 Dependency Path Matrix
version: 0.4
status: Complete with conditions
owner: Software Engineer
contributors:
  - Mason profile
produced_by: Software Engineer
consumed_by: Solution Architect, Security Engineer, Quality Engineer, Platform Engineer
date: 2026-08-18
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
  - Implementation Plan.md
  - ../02 Architecture/Architecture Feedback - SEC-006 Dependency Remediation.md
  - ../05 Security/Security Assessment.md
  - ../05 Security/Security Feedback - SEC-006 Candidate A Approval.md
  - ../04 Quality/Release Readiness Report.md
```

## Purpose and Review Boundary

This matrix records the Engineering baseline and Candidate A implementation evidence for SEC-006. It separates published package closure, supported consumer graphs and repository-only test/sample graphs as required by the approved Architecture Feedback. It is not release approval; SEC-006 remains subject to final Security, Quality and Platform evidence.

The resolved versions below come from the implemented solution restore graph. Published-closure entries are separated by evidence status. The MVC package archive and supported MVC package-consumer evidence are recorded in [SEC-006 Closure Evidence](SEC-006%20Closure%20Evidence.md); final Security closure remains outstanding.

## Advisory Matrix

| Advisory / reported package | Ownership and affected graph | Dependency path | Requested and resolved versions | Published closure | Runtime context | Candidate remediation / decision gate | Verification status |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Critical — `System.Text.Encodings.Web` | Published: Core, HTTP and Validation package families. Supported: Core, HTTP, Validation and their consumer graphs. Repository-only: test/sample graphs also inherit it through project references. | `Nestgrid.Response` → `System.Text.Json` → `System.Text.Encodings.Web`; HTTP and Validation reach the same path through Core. | Baseline: `System.Text.Encodings.Web` 4.6.0/4.5.0. Candidate A resolves the affected evaluated graphs to 4.7.2 while retaining `System.Text.Json` 4.6.0. | Candidate A package closure is non-vulnerable in the generated/inspected Core, HTTP and Validation metadata; final release evidence remains required. | `netstandard2.0` library runtime for Core, HTTP and Validation; consumer runtime determines execution. | Candidate A is the selected lowest-compatible patched dependency under ADR-007. | Baseline confirmed; Candidate A evaluated closure remediated. Final evidence remains open. |
| High — `Microsoft.AspNetCore.Http` | Published: MVC package family. Supported: MVC consumers using the approved legacy baseline. Repository-only: MVC tests and MVC sample. | Baseline: `Nestgrid.Response.Mvc` → `Microsoft.AspNetCore.Mvc.Core 2.1.38` → `Microsoft.AspNetCore.Http 2.1.1`. Candidate A adds the patched direct dependency. | Baseline: 2.1.1. Candidate A: 2.1.22 with MVC parent 2.1.38 retained. | Candidate A MVC closure is evidenced as non-vulnerable in the current-commit equivalent package metadata and consumer graph. | `netstandard2.0` MVC adapter and the approved MVC consumer baseline; MVC support promise must not be broadened silently. | Candidate A preserves the active MVC boundary under Architecture approval. | Baseline confirmed; Candidate A build, tests, metadata and consumer evidence passed. Security closure remains open. |
| High — `Newtonsoft.Json` | Published: MVC package family. Supported: MVC consumers using the approved legacy baseline. Repository-only: MVC tests and MVC sample. | Baseline: `Nestgrid.Response.Mvc` → `Microsoft.AspNetCore.Mvc.Core 2.1.38` → ASP.NET Core 2.1 dependency graph → `Newtonsoft.Json 9.0.1`. Candidate A adds the patched direct dependency. | Baseline: 9.0.1. Candidate A: 13.0.1 with MVC parent 2.1.38 retained. | Candidate A MVC closure is evidenced as non-vulnerable in the current-commit equivalent package metadata and consumer graph. | `netstandard2.0` MVC adapter and approved legacy consumer baseline. | Candidate A preserves the active MVC boundary under Architecture approval. | Baseline confirmed; Candidate A build, tests, metadata and consumer evidence passed. Security closure remains open. |

## Graph Classification

| Product area | Published package(s) | Supported consumer graph | Repository-only graph | Current SEC-006 position |
| --- | --- | --- | --- | --- |
| Core / JSON | `Nestgrid.Response` | `netstandard2.0` package consumed by maintained application targets | Core tests and core/sample project references | Candidate A evaluated closure is remediated; retain final advisory/package evidence. |
| Shared HTTP | `Nestgrid.Response.Http` | Shared policy package consumed by both adapters | HTTP tests and samples | Candidate A evaluated closure is remediated; retain final advisory/package evidence. |
| Validation | `Nestgrid.Response.Extensions.Validation` | `netstandard2.0` package consumed by DataAnnotations applications | Validation tests and sample | Candidate A evaluated closure is remediated; retain final advisory/package evidence. |
| MVC | `Nestgrid.Response.Mvc` | Approved legacy MVC baseline using `Microsoft.AspNetCore.Mvc.Core 2.1.38` | MVC tests and MVC sample | Candidate A evaluated closure is remediated; fresh MVC package metadata and consumer evidence remain open. |
| Modern ASP.NET Core | `Nestgrid.Response.AspNetCore` | `net8.0` adapter using `Microsoft.AspNetCore.App` | ASP.NET Core tests and sample | Framework reference is host-provided; Candidate A leaves the project unchanged and no reported vulnerable package was found in the evaluated graph. |

## Evidence Gate for SEC-006 Closure

- Fresh restore output for all source, test and sample projects, including direct and transitive versions. **Complete.**
- Dependency-path evidence identifying the introducing package for each advisory.
- Generated `.nupkg` and `.nuspec` inspection for all five published packages and each target framework dependency group. **Complete for Core, HTTP and Validation; ASP.NET Core cross-checked; MVC equivalent current-commit package metadata and hash retained.**
- Affected consumer-installation checks from a local package source rather than project references alone. **Complete for Core, HTTP, Validation and MVC through the retained local evidence packages.**
- Lowest compatible patched-version candidates, with target-framework, API and MVC support analysis.
- Advisory scan results for the baseline and each candidate graph. **Candidate A final scan complete: no vulnerable packages reported.**
- Explicit Architecture and Security disposition for any support-boundary change or residual-risk exception.

## Candidate A Validation Evidence

Candidate A was implemented on the product branch in commit `3c0e151` after Architecture and Security approval. The candidate was first evaluated in an isolated worktree from commit `662fb15`, then the exact approved pins were applied to the product branch.

| Check | Result | Interpretation |
| --- | --- | --- |
| Restore | Passed for the complete solution | Candidate package declarations restore successfully from NuGet. |
| Release build | Passed, 0 warnings, 0 errors | All source, test and sample projects compile with the proposed parent/dependency combination. |
| Regression tests | 289 passed, 0 failed, 0 skipped | Core, HTTP, ASP.NET Core, MVC and Validation behaviour remains compatible at test level. |
| Advisory scan | No vulnerable packages reported across source, test and sample projects | The three reported SEC-006 findings are absent from the evaluated Candidate A graphs. |
| MVC parent compatibility | Passed at compile and regression-test level | `Microsoft.AspNetCore.Mvc.Core 2.1.38` remains resolved alongside `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1`. |
| MVC encoding path | Passed at resolved-graph level | `System.Text.Encodings.Web` resolves to `4.7.2`; the previous `4.5.0` path is no longer selected. |
| MVC package consumer | Passed | The current MVC evidence package restored, built and executed a temporary consumer exercising successful and invalid result conversion. |

## Candidate A Published Dependency Metadata

The table shows the final dependency groups expected in each published package’s `.nuspec`. Core, HTTP and Validation metadata was generated and inspected from the implemented branch pack. ASP.NET Core metadata is unchanged by Candidate A and was cross-checked against the existing generated package metadata. MVC metadata was inspected from the project-generated 0.7.0 `.nuspec` and retained in the equivalent current-commit evidence package described in [SEC-006 Closure Evidence](SEC-006%20Closure%20Evidence.md). The standard isolated MSBuild pack target still hangs, so the evidence package is not a release publication artefact.

| Published package | Target framework | Direct dependency metadata in Candidate A `.nuspec` | Resolved affected closure | Evidence status |
| --- | --- | --- | --- | --- |
| `Nestgrid.Response` | `netstandard2.0` | `System.Text.Encodings.Web 4.7.2`; `System.Text.Json 4.6.0` | `System.Text.Encodings.Web 4.7.2`; no reported vulnerable JSON encoding version | Implemented-branch `.nuspec` generated and inspected. |
| `Nestgrid.Response.Http` | `netstandard2.0` | `Nestgrid.Response 0.7.0` | Inherits Core’s `System.Text.Encodings.Web 4.7.2` closure | Implemented-branch `.nuspec` generated and inspected; transitive closure verified by restore. |
| `Nestgrid.Response.AspNetCore` | `net8.0` | `Nestgrid.Response.Http 0.7.0`; framework reference `Microsoft.AspNetCore.App` | Host framework supplies ASP.NET Core dependencies; project graph resolves `System.Text.Encodings.Web 4.7.2` through Core | Project unchanged; existing generated metadata cross-checked; fresh pack archive blocked by MSBuild hang. |
| `Nestgrid.Response.Mvc` | `netstandard2.0` | `Nestgrid.Response.Http 0.7.0`; `Microsoft.AspNetCore.Mvc.Core 2.1.38`; `Microsoft.AspNetCore.Http 2.1.22`; `Newtonsoft.Json 13.0.1` | `Microsoft.AspNetCore.Http 2.1.22`; `Newtonsoft.Json 13.0.1`; `System.Text.Encodings.Web 4.7.2`; no reported `Http 2.1.1`, `Newtonsoft.Json 9.0.1` or encoding `4.5.0` selected | Project-generated `.nuspec`, equivalent current-commit evidence package, resolved closure and supported consumer verified; standard `dotnet pack` remains environmentally limited. |
| `Nestgrid.Response.Extensions.Validation` | `netstandard2.0` | `Nestgrid.Response 0.7.0`; `System.ComponentModel.Annotations 4.1.0` | Inherits Core’s `System.Text.Encodings.Web 4.7.2` closure | Implemented-branch `.nuspec` generated and inspected; transitive closure verified by restore. |

### MVC Encoding Path

The original MVC graph contained `Microsoft.AspNetCore.Mvc.Core 2.1.38`, which resolved `Microsoft.AspNetCore.Http 2.1.1`, `Newtonsoft.Json 9.0.1` and `System.Text.Encodings.Web 4.5.0`. Candidate A retains the parent MVC package at `2.1.38` but adds explicit compatible dependencies. The evaluated graph now resolves:

```text
Nestgrid.Response.Mvc
├── Microsoft.AspNetCore.Mvc.Core 2.1.38
│   └── Microsoft.AspNetCore.Http 2.1.22
├── Microsoft.AspNetCore.Http 2.1.22
├── Newtonsoft.Json 13.0.1
└── Nestgrid.Response.Http
    └── Nestgrid.Response
        └── System.Text.Json 4.6.0
            └── System.Text.Encodings.Web 4.7.2
```

The parent package compiled and all MVC tests passed without changing the approved MVC target framework or parent package identity. MSBuild project evaluation and the inspected package metadata confirm the four direct published dependencies shown above. The supported package consumer restored, built and executed successfully. The standard pack limitation is retained as an environmental limitation, not an identified compatibility failure.

## Candidate Remediation Comparison

The following candidates record the approved implementation choice and the alternatives that remain out of scope.

| Candidate | Proposed change | Compatibility position | Security position | Recommendation |
| --- | --- | --- | --- | --- |
| A — Explicit lowest-compatible transitive pins | Preserve `System.Text.Json 4.6.0` and add an explicit `System.Text.Encodings.Web 4.7.2` dependency for the Core package family. Preserve `Microsoft.AspNetCore.Mvc.Core 2.1.38` and add explicit `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1` dependencies to the MVC package. Manage the selected versions centrally while keeping ownership declarations in the owning projects. | Lowest-change option. Preserves package identity, target frameworks and the active MVC boundary, subject to API/binary, restore and consumer verification. It changes published dependency metadata and therefore requires package-closure review. | Uses the patched versions recorded by the authoritative advisories. It must be confirmed that no vulnerable versions remain in every affected published or supported graph. | **Implemented and approved for this remediation**, conditional on final evidence. |
| B — Upgrade the parent packages | Upgrade `System.Text.Json` to a later compatible 4.x baseline and/or move the MVC adapter to a later ASP.NET Core MVC package line. | The JSON change may be compatible but must be checked against the full `netstandard2.0` graph. Moving MVC beyond 2.1.38 changes the approved legacy support baseline and requires a separate Architecture/Product decision. | Could remediate transitively, but a parent upgrade may introduce additional changes and advisories. | Do not select without evidence; MVC parent upgrades are outside Engineering authority under the current Architecture. |
| C — Authorised exception | Retain the current graph with explicit scope, mitigations, owner, expiry/review date and Security acceptance. | Preserves compatibility but retains known vulnerable dependencies. | Not preferred because patched package versions are available for the reported advisories. | **Fallback only** if Candidate A cannot preserve the approved support boundary and no compatible parent upgrade is authorised. |

### Candidate A Closure Evidence Gate

Candidate A implementation is approved and complete. The following conditions now govern SEC-006 closure:

- explicit transitive pins remain acceptable under ADR-007 rather than advisory suppression;
- `System.Text.Encodings.Web 4.7.2` is compatible with the existing `System.Text.Json 4.6.0` API/runtime combination;
- `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1` remain compatible with `Microsoft.AspNetCore.Mvc.Core 2.1.38`;
- generated package dependency groups contain the intended patched versions and no reported vulnerable versions;
- package consumers can restore and execute the Core, HTTP, Validation and MVC package families; and
- the final advisory scan shows the reported findings resolved or identifies any residual finding for explicit disposition.

## Advisory Source Baseline

- [GHSA-ghhp-997w-qr28 — System.Text.Encodings.Web](https://github.com/advisories/GHSA-ghhp-997w-qr28): versions 4.6.0–4.7.1 are affected; 4.7.2 is patched.
- [GHSA-hxrm-9w7p-39cc — Microsoft.AspNetCore.Http](https://github.com/advisories/GHSA-hxrm-9w7p-39cc): versions below 2.1.22 are affected; 2.1.22 is patched.
- [GHSA-5crp-9r3c-p9vr — Newtonsoft.Json](https://github.com/advisories/GHSA-5crp-9r3c-p9vr): versions below 13.0.1 are affected; 13.0.1 is patched.
- [System.Text.Json 4.6.0 package metadata](https://www.nuget.org/packages/System.Text.Json/4.6.0): Candidate A retains the approved `System.Text.Json 4.6.0` baseline and explicitly pins `System.Text.Encodings.Web 4.7.2`; restore, build, test and evaluated-graph evidence support the combination.
- [Microsoft.AspNetCore.Http 2.1.22 package metadata](https://www.nuget.org/packages/Microsoft.AspNetCore.Http/2.1.22): supports `netstandard2.0`.
- [Newtonsoft.Json 13.0.1 package metadata](https://www.nuget.org/packages/Newtonsoft.Json/13.0.1): supports `netstandard2.0`.

## Current Conclusion

Candidate A is implemented with the approved exact pins. It remediates all currently affected resolved graphs without changing package identity, target frameworks or the approved MVC parent package/support boundary. Engineering’s package metadata and supported consumer evidence gate is complete; final Quality reconciliation and Security closure remain. No Architecture/Product/Sponsor boundary decision is indicated by the implementation evidence.
