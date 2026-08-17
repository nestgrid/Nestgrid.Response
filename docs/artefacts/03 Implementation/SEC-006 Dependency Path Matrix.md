# SEC-006 Dependency Path Matrix

```yaml
title: Nestgrid.Response v0.7.0 SEC-006 Dependency Path Matrix
version: 0.3
status: In Review
owner: Software Engineer
contributors:
  - Mason profile
produced_by: Software Engineer
consumed_by: Solution Architect, Security Engineer, Quality Engineer, Platform Engineer
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
  - Implementation Plan.md
  - ../02 Architecture/Architecture Feedback - SEC-006 Dependency Remediation.md
  - ../05 Security/Security Assessment.md
  - ../05 Security/Security Feedback - SEC-006 Candidate A Approval.md
  - ../04 Quality/Release Readiness Report.md
```

## Purpose and Review Boundary

This matrix records the Engineering baseline for SEC-006 before any dependency or source changes. It separates published package closure, supported consumer graphs and repository-only test/sample graphs as required by the approved Architecture Feedback. It is a review input, not an acceptance of the current vulnerable graph and not evidence that remediation is complete.

The resolved versions below come from the current solution restore graph on 2026-08-17. Published-closure entries must be confirmed from freshly generated `.nupkg` and `.nuspec` files after the remediation plan is approved.

## Advisory Matrix

| Advisory / reported package | Ownership and affected graph | Dependency path | Requested and resolved versions | Published closure | Runtime context | Candidate remediation / decision gate | Verification status |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Critical — `System.Text.Encodings.Web` | Published: Core, HTTP and Validation package families. Supported: Core, HTTP, Validation and their consumer graphs. Repository-only: test/sample graphs also inherit it through project references. | `Nestgrid.Response` → `System.Text.Json` → `System.Text.Encodings.Web`; HTTP and Validation reach the same path through Core. | Direct `System.Text.Json` requested/resolved `4.6.0`; transitive `System.Text.Encodings.Web` resolved `4.6.0` in Core, HTTP and Validation graphs. MVC also resolves `4.5.0` through its legacy framework dependency. | Expected in Core package closure through `System.Text.Json`; expected transitively in HTTP and Validation closures. Exact generated dependency groups are pending pack inspection. | `netstandard2.0` library runtime for Core, HTTP and Validation; consumer runtime determines execution. | Evaluate the lowest compatible patched `System.Text.Json` and/or encoding dependency supported by the target framework. A direct pin is not acceptable without graph and compatibility proof. Escalate any target-framework or package-boundary change. | Baseline graph confirmed. Advisory confirmation, patched candidate and final package closure pending. |
| High — `Microsoft.AspNetCore.Http` | Published: MVC package family. Supported: MVC consumers using the approved legacy baseline. Repository-only: MVC tests and MVC sample. | `Nestgrid.Response.Mvc` → `Microsoft.AspNetCore.Mvc.Core 2.1.38` → `Microsoft.AspNetCore.Http 2.1.1`. | Direct `Microsoft.AspNetCore.Mvc.Core` requested/resolved `2.1.38`; transitive `Microsoft.AspNetCore.Http` resolved `2.1.1`. | Expected in MVC package closure through `Microsoft.AspNetCore.Mvc.Core`; exact generated `.nuspec` dependency group pending confirmation. | `netstandard2.0` MVC adapter and the approved MVC consumer baseline; MVC support promise must not be broadened silently. | Evaluate whether a compatible patched MVC dependency exists without changing the active support boundary. If not, Architecture/Product/Sponsor must decide whether the boundary changes or an explicit exception is proposed. | Baseline graph confirmed. Compatibility and support-boundary decision pending. |
| High — `Newtonsoft.Json` | Published: MVC package family. Supported: MVC consumers using the approved legacy baseline. Repository-only: MVC tests and MVC sample. | `Nestgrid.Response.Mvc` → `Microsoft.AspNetCore.Mvc.Core 2.1.38` → ASP.NET Core 2.1 dependency graph → `Newtonsoft.Json 9.0.1`. | Direct `Microsoft.AspNetCore.Mvc.Core` requested/resolved `2.1.38`; transitive `Newtonsoft.Json` resolved `9.0.1` in MVC and MVC sample graphs. Test projects resolve `Newtonsoft.Json 13.0.1` through test tooling and are not the reported published path. | Expected in MVC package closure through the legacy MVC dependency graph; exact generated `.nuspec` dependency group pending confirmation. | `netstandard2.0` MVC adapter and approved legacy consumer baseline. | Evaluate a patched compatible MVC dependency path. Do not suppress the advisory with an unrelated direct pin. If remediation requires leaving the approved MVC baseline, escalate before implementation; otherwise prepare a scoped exception for Security review. | Baseline graph confirmed. Compatibility and exception/remediation decision pending. |

## Graph Classification

| Product area | Published package(s) | Supported consumer graph | Repository-only graph | Current SEC-006 position |
| --- | --- | --- | --- | --- |
| Core / JSON | `Nestgrid.Response` | `netstandard2.0` package consumed by maintained application targets | Core tests and core/sample project references | Vulnerable transitive closure must be remediated or explicitly accepted. |
| Shared HTTP | `Nestgrid.Response.Http` | Shared policy package consumed by both adapters | HTTP tests and samples | Inherits the Core JSON closure; verify package closure independently. |
| Validation | `Nestgrid.Response.Extensions.Validation` | `netstandard2.0` package consumed by DataAnnotations applications | Validation tests and sample | Inherits the Core JSON closure; verify package closure independently. |
| MVC | `Nestgrid.Response.Mvc` | Approved legacy MVC baseline using `Microsoft.AspNetCore.Mvc.Core 2.1.38` | MVC tests and MVC sample | Contains the reported ASP.NET Core HTTP, Newtonsoft.Json and legacy encoding paths; requires compatibility decision. |
| Modern ASP.NET Core | `Nestgrid.Response.AspNetCore` | `net8.0` adapter using `Microsoft.AspNetCore.App` | ASP.NET Core tests and sample | Framework reference is host-provided; verify whether reported packages are absent from the published package closure and assess host graph separately. |

## Evidence Required Before Remediation Approval

- Fresh restore output for all source, test and sample projects, including direct and transitive versions.
- Dependency-path evidence identifying the introducing package for each advisory.
- Generated `.nupkg` and `.nuspec` inspection for all five published packages and each target framework dependency group.
- Affected consumer-installation checks from a local package source rather than project references alone.
- Lowest compatible patched-version candidates, with target-framework, API and MVC support analysis.
- Advisory scan results for the baseline and each candidate graph.
- Explicit Architecture and Security disposition for any support-boundary change or residual-risk exception.

## Candidate A Validation Evidence

Candidate A was evaluated in an isolated worktree from commit `662fb15`; no product-branch files were changed.

| Check | Result | Interpretation |
| --- | --- | --- |
| Restore | Passed for the complete solution | Candidate package declarations restore successfully from NuGet. |
| Release build | Passed, 0 warnings, 0 errors | All source, test and sample projects compile with the proposed parent/dependency combination. |
| Regression tests | 289 passed, 0 failed, 0 skipped | Core, HTTP, ASP.NET Core, MVC and Validation behaviour remains compatible at test level. |
| Advisory scan | No vulnerable packages reported across source, test and sample projects | The three reported SEC-006 findings are absent from the evaluated Candidate A graphs. |
| MVC parent compatibility | Passed at compile and regression-test level | `Microsoft.AspNetCore.Mvc.Core 2.1.38` remains resolved alongside `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1`. |
| MVC encoding path | Passed at resolved-graph level | `System.Text.Encodings.Web` resolves to `4.7.2`; the previous `4.5.0` path is no longer selected. |

## Candidate A Published Dependency Metadata

The table shows the final dependency groups expected in each published package’s `.nuspec`. Core, HTTP and Validation metadata was generated and inspected from the Candidate A pack. ASP.NET Core metadata is unchanged by Candidate A and was cross-checked against the existing generated package metadata. MVC metadata is derived from the evaluated Candidate A project/package references; the isolated MSBuild pack target hung before emitting the MVC archive, so a final MVC `.nuspec` inspection remains a release-evidence task.

| Published package | Target framework | Direct dependency metadata in Candidate A `.nuspec` | Resolved affected closure | Evidence status |
| --- | --- | --- | --- | --- |
| `Nestgrid.Response` | `netstandard2.0` | `System.Text.Encodings.Web 4.7.2`; `System.Text.Json 4.6.0` | `System.Text.Encodings.Web 4.7.2`; no reported vulnerable JSON encoding version | Generated and inspected. |
| `Nestgrid.Response.Http` | `netstandard2.0` | `Nestgrid.Response 0.7.0` | Inherits Core’s `System.Text.Encodings.Web 4.7.2` closure | Generated and inspected; transitive closure verified by restore. |
| `Nestgrid.Response.AspNetCore` | `net8.0` | `Nestgrid.Response.Http 0.7.0`; framework reference `Microsoft.AspNetCore.App` | Host framework supplies ASP.NET Core dependencies; project graph resolves `System.Text.Encodings.Web 4.7.2` through Core | Project unchanged; existing generated metadata cross-checked; fresh pack archive blocked by MSBuild hang. |
| `Nestgrid.Response.Mvc` | `netstandard2.0` | `Nestgrid.Response.Http 0.7.0`; `Microsoft.AspNetCore.Mvc.Core 2.1.38`; `Microsoft.AspNetCore.Http 2.1.22`; `Newtonsoft.Json 13.0.1` | `Microsoft.AspNetCore.Http 2.1.22`; `Newtonsoft.Json 13.0.1`; `System.Text.Encodings.Web 4.7.2`; no reported `Http 2.1.1`, `Newtonsoft.Json 9.0.1` or encoding `4.5.0` selected | Evaluated metadata and resolved closure verified; fresh MVC `.nuspec` inspection remains outstanding due MSBuild hang. |
| `Nestgrid.Response.Extensions.Validation` | `netstandard2.0` | `Nestgrid.Response 0.7.0`; `System.ComponentModel.Annotations 4.1.0` | Inherits Core’s `System.Text.Encodings.Web 4.7.2` closure | Generated and inspected; transitive closure verified by restore. |

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

The parent package compiled and all MVC tests passed without changing the approved MVC target framework or parent package identity. The remaining `.nuspec` archive inspection is evidence completion, not an identified compatibility failure.

## Candidate Remediation Comparison

The following candidates are proposals for review. No candidate has been implemented or accepted.

| Candidate | Proposed change | Compatibility position | Security position | Recommendation |
| --- | --- | --- | --- | --- |
| A — Explicit lowest-compatible transitive pins | Preserve `System.Text.Json 4.6.0` and add an explicit `System.Text.Encodings.Web 4.7.2` dependency for the Core package family. Preserve `Microsoft.AspNetCore.Mvc.Core 2.1.38` and add explicit `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1` dependencies to the MVC package. Manage the selected versions centrally while keeping ownership declarations in the owning projects. | Lowest-change option. Preserves package identity, target frameworks and the active MVC boundary, subject to API/binary, restore and consumer verification. It changes published dependency metadata and therefore requires package-closure review. | Uses the patched versions recorded by the authoritative advisories. It must be confirmed that no vulnerable versions remain in every affected published or supported graph. | **Preferred for Architecture/Security review**, conditional on evidence. |
| B — Upgrade the parent packages | Upgrade `System.Text.Json` to a later compatible 4.x baseline and/or move the MVC adapter to a later ASP.NET Core MVC package line. | The JSON change may be compatible but must be checked against the full `netstandard2.0` graph. Moving MVC beyond 2.1.38 changes the approved legacy support baseline and requires a separate Architecture/Product decision. | Could remediate transitively, but a parent upgrade may introduce additional changes and advisories. | Do not select without evidence; MVC parent upgrades are outside Engineering authority under the current Architecture. |
| C — Authorised exception | Retain the current graph with explicit scope, mitigations, owner, expiry/review date and Security acceptance. | Preserves compatibility but retains known vulnerable dependencies. | Not preferred because patched package versions are available for the reported advisories. | **Fallback only** if Candidate A cannot preserve the approved support boundary and no compatible parent upgrade is authorised. |

### Candidate A Evidence Gate

Candidate A is approved for implementation by Security, subject to the following evidence and compatibility conditions:

- explicit transitive pins are acceptable under ADR-007 rather than advisory suppression;
- `System.Text.Encodings.Web 4.7.2` is compatible with the existing `System.Text.Json 4.6.0` API/runtime combination;
- `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1` remain compatible with `Microsoft.AspNetCore.Mvc.Core 2.1.38`;
- generated package dependency groups contain the intended patched versions and no reported vulnerable versions;
- package consumers can restore and execute the Core, HTTP, Validation and MVC package families; and
- the final advisory scan shows the reported findings resolved or identifies any residual finding for explicit disposition.

## Advisory Source Baseline

- [GHSA-ghhp-997w-qr28 — System.Text.Encodings.Web](https://github.com/advisories/GHSA-ghhp-997w-qr28): versions 4.6.0–4.7.1 are affected; 4.7.2 is patched.
- [GHSA-hxrm-9w7p-39cc — Microsoft.AspNetCore.Http](https://github.com/advisories/GHSA-hxrm-9w7p-39cc): versions below 2.1.22 are affected; 2.1.22 is patched.
- [GHSA-5crp-9r3c-p9vr — Newtonsoft.Json](https://github.com/advisories/GHSA-5crp-9r3c-p9vr): versions below 13.0.1 are affected; 13.0.1 is patched.
- [System.Text.Json 4.7.2 package metadata](https://www.nuget.org/packages/System.Text.Json/4.7.2): supports `netstandard2.0` and declares `System.Text.Encodings.Web` at or above 4.7.1, so Candidate A retains an explicit 4.7.2 pin pending restore proof.
- [Microsoft.AspNetCore.Http 2.1.22 package metadata](https://www.nuget.org/packages/Microsoft.AspNetCore.Http/2.1.22): supports `netstandard2.0`.
- [Newtonsoft.Json 13.0.1 package metadata](https://www.nuget.org/packages/Newtonsoft.Json/13.0.1): supports `netstandard2.0`.

## Current Conclusion

The baseline confirms that SEC-006 affects published or supported package families, especially the MVC package. Central package management did not create the advisories; it preserves the previously selected versions under ADR-007. Candidate A is approved for implementation because it uses the lowest identified patched versions without changing package identity, target frameworks or the approved MVC parent package. The isolated evidence indicates that it remediates all currently affected resolved graphs without changing the approved MVC support boundary. The final MVC `.nuspec` inspection, supported consumer checks and final advisory/restore evidence remain mandatory before SEC-006 closure or release consideration.
