# SEC-006 Dependency Path Matrix

```yaml
title: Nestgrid.Response v0.7.0 SEC-006 Dependency Path Matrix
version: 0.1
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

## Current Conclusion

The baseline confirms that SEC-006 affects published or supported package families, especially the MVC package. Central package management did not create the advisories; it preserves the previously selected versions under ADR-007. No remediation version is selected in this review package. Engineering must not change package versions until Architecture and Security approve the remediation direction and its compatibility evidence.
