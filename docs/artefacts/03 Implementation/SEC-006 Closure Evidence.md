# SEC-006 Closure Evidence

```yaml
title: Nestgrid.Response v0.7.0 SEC-006 Closure Evidence
version: 1.0
status: Complete with conditions
owner: Software Engineer
contributors:
  - Mason profile
produced_by: Software Engineer
consumed_by: Quality Engineer, Security Engineer, Platform Engineer
date: 2026-08-18
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
related_work_items:
  - SEC-006
  - Q-007
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - SEC-006 Dependency Path Matrix.md
  - Implementation Report.md
  - ../04 Quality/Release Readiness Report.md
  - ../05 Security/Security Assessment.md
```

## Purpose

This artefact records the Engineering evidence for IR-008 and SEC-006 closure. It is a handover to Quality and Security, not a Security closure or release approval.

## Candidate and Provenance

- Candidate commit: `6a0598fefda587ef283129c51d4f56e611534fa`.
- Dependency implementation commit: `3c0e151`.
- Approved MVC parent: `Microsoft.AspNetCore.Mvc.Core 2.1.38`.
- Standard `dotnet pack` for the MVC project remains affected by the known local MSBuild hang. The equivalent evidence package was assembled from the current Release MVC assembly and the project-generated 0.7.0 `.nuspec` dependency metadata, with the candidate commit recorded in the evidence package metadata.
- MVC evidence package SHA-256: `88b250cd14d34a60565489ef78b15b7ec4ef9a60aed97c42ef91a232e14b3d63`.

## MVC Published Dependency Metadata

The inspected `.NETStandard2.0` dependency group contains:

| Dependency | Version |
| --- | --- |
| `Nestgrid.Response.Http` | `0.7.0` |
| `Microsoft.AspNetCore.Http` | `2.1.22` |
| `Microsoft.AspNetCore.Mvc.Core` | `2.1.38` |
| `Newtonsoft.Json` | `13.0.1` |

The resolved MVC graph also selects `System.Text.Encodings.Web 4.7.2` and does not select the reported vulnerable versions `Microsoft.AspNetCore.Http 2.1.1`, `Newtonsoft.Json 9.0.1` or `System.Text.Encodings.Web 4.5.0`.

## Verification Results

| Check | Result |
| --- | --- |
| MVC Release build | Passed; 0 warnings, 0 errors. |
| MVC adapter tests | 28 passed, 0 failed, 0 skipped. |
| MVC project evaluation | `netstandard2.0`; approved direct dependency declarations and project reference confirmed. |
| Resolved MVC graph | Approved patched versions selected; vulnerable reported versions absent. |
| Full solution advisory scan | No vulnerable packages reported across source, test and sample projects. |
| Package consumer restore | Passed from the local MVC evidence package and local Nestgrid dependency feed. |
| Package consumer build | Passed; 0 warnings, 0 errors. |
| Package consumer execution | Passed; success and invalid MVC result conversions executed. |

## Closure Position

Engineering considers the implementation evidence for IR-008 complete. Quality should reconcile the package-consumer and release-readiness records. Security should perform the final SEC-006 closure review under ADR-007. Protected publication, provenance retention in the supported CI environment and Release-stage approval remain outside Engineering authority.
