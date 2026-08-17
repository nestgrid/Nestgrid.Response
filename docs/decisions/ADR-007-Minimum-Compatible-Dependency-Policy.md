# ADR-007: Minimum-Compatible Dependency Policy

> Decision record for **Architecture**.

## Status

Accepted

## Type

Architecture

## Date

2026-08-17

## Owners

- Solution Architect
- Security Engineer
- Platform Engineer

## Context

Nestgrid.Response supports a compatibility-sensitive public NuGet package set, including a legacy MVC adapter. Security review identified that the repository did not retain current advisory and restore evidence for its deliberately old, minimum-compatible dependencies.

The product must balance vulnerability response with compatibility. Upgrading every dependency to the newest major version would undermine the approved MVC and portable-package support model without proving a security benefit.

## Decision

Nestgrid.Response will use the lowest compatible direct dependency versions that have no known unresolved vulnerability relevant to the supported package baseline.

The policy requires:

- dependency versions to remain compatible with the approved target-framework matrix;
- no upgrade solely for recency or major-version currency;
- advisory and restore review before each release and whenever a dependency changes;
- retained evidence of the evaluated dependency graph, advisory result, restore result and package provenance;
- escalation to Architecture, Security and the Project Sponsor when a vulnerable minimum version conflicts with compatibility;
- an explicit exception and review date when a vulnerable dependency cannot be upgraded immediately.

The current approved direct dependency baselines include:

- `System.Text.Json` 4.6.0;
- `System.ComponentModel.Annotations` 4.1.0;
- `Microsoft.AspNetCore.Mvc.Core` 2.1.38;
- `System.Text.Encodings.Web` 4.7.2;
- `Microsoft.AspNetCore.Http` 2.1.22; and
- `Newtonsoft.Json` 13.0.1.

The final three pins were approved through the SEC-006 Candidate A remediation to remove known vulnerable resolved graphs while preserving package identity, target frameworks and the active MVC support boundary. All versions remain subject to advisory, restore, package-provenance and consumer verification; this record does not declare a dependency vulnerability-free without current evidence.

## Rationale

The policy preserves the product’s approved compatibility promise while making security evidence and escalation mandatory. It avoids both silent dependency drift and indiscriminate upgrades that could break the supported MVC consumer segment.

## Alternatives Considered

### Always upgrade to the newest available version

Rejected because it can introduce major compatibility changes without a demonstrated security requirement.

### Never upgrade legacy dependencies

Rejected because a known vulnerability may require a security-driven compatibility decision.

### Leave dependency decisions to individual releases

Rejected because it provides no consistent review trigger or retained evidence standard.

## Consequences

- Release evidence must include current advisory and restore results.
- A vulnerable minimum version becomes an explicit risk decision rather than an invisible dependency fact.
- Security and Architecture must participate when compatibility and vulnerability remediation conflict.
- Package compatibility remains a first-class architectural constraint.

## Related Decisions

- [ADR-006 ASP.NET Core and MVC Package Separation](ADR-006-AspNetCore-And-Mvc-Package-Separation.md)

## Related Documentation

- [Architecture Feedback - Security](../artefacts/02%20Architecture/Architecture%20Feedback%20-%20Security.md)
- [Architecture Feedback - SEC-006 Dependency Remediation](../artefacts/02%20Architecture/Architecture%20Feedback%20-%20SEC-006%20Dependency%20Remediation.md)
- [SEC-006 Dependency Path Matrix](../artefacts/03%20Implementation/SEC-006%20Dependency%20Path%20Matrix.md)
- [Implementation Report](../artefacts/03%20Implementation/Implementation%20Report.md)
- [Security Assessment](../artefacts/05%20Security/Security%20Assessment.md)
- [Deployment Guide](../artefacts/06%20Platform/Deployment%20Guide.md)
