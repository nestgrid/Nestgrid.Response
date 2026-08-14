# Architecture Handover

```yaml
title: Nestgrid.Response Architecture Handover
version: 1.0
status: Approved
owner: Product Owner
contributors: Knight
produced_by: Product Owner
consumed_by: Solution Architect
date: 2026-08-14
supersedes:
related_decisions:
  - ../../decisions/ADR-004-AspNetCore-Separation.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Product Brief.md
```

## Purpose

Identify the architectural work required after Product Brief approval. Architecture may now consume this handover, subject to the normal Architecture workflow and gates.

## Problem Summary

Developers need a small, explicit result model for expected application outcomes that can be used independently of HTTP, then adapted consistently for modern ASP.NET Core and legacy ASP.NET Core MVC applications.

## Approved Vision

Maintain Nestgrid.Response as a focused, actively supported .NET library serving both Nestgrid teams and public NuGet consumers. Modern ASP.NET Core is the primary user segment; legacy MVC is an equally supported segment. The product remains intentionally smaller than a general functional, validation, workflow or transport framework.

## Approved Scope

- Existing five-package product baseline.
- Core result model and functional helpers.
- Shared HTTP mapping policy.
- Modern ASP.NET Core adapter.
- Actively supported legacy MVC adapter.
- DataAnnotations validation extension.
- Package distribution, documentation, samples and lifecycle evidence.

OpenAPI helpers, `ProblemDetails` support and additional adapters are not approved scope.

## Business Capabilities

| Capability | Description | Priority |
| --- | --- | --- |
| Explicit application outcomes | Represent common successful and non-successful outcomes with semantic status and optional value. | Must |
| Structured result context | Carry human-readable and machine-readable messages without domain-specific coupling. | Must |
| Framework-independent consumption | Use the core product from application, worker and library code without presentation dependencies. | Must |
| Modern HTTP adaptation | Convert results into predictable modern ASP.NET Core responses. | Must |
| Legacy MVC adaptation | Convert results into predictable supported MVC responses for existing applications. | Must |
| Validation integration | Convert DataAnnotations validation outcomes into invalid result messages. | Should |
| Consumer adoption and support | Install, understand, upgrade and validate the packages through NuGet guidance, samples and evidence. | Must |

## Outstanding Architectural Decisions

| Decision Area | Why It Matters | Notes |
| --- | --- | --- |
| Supported framework and compatibility matrix | Defines the real support promise for modern ASP.NET Core and legacy MVC consumers. | Reassess current targets and dependency boundaries; Product Owner has not prescribed a change. |
| MVC support policy and maintenance boundary | Equal support is a product commitment with cost and compatibility consequences. | Define support duration, compatibility expectations and review triggers. |
| Current package and adapter boundaries | Determines whether the five-package baseline remains sustainable and consistent. | Validate existing boundaries; do not assume the implementation is automatically correct. |
| Public API and status compatibility | Statuses and result shapes are consumer-facing contracts. | Identify compatibility risks before any 1.0 or broad API changes. |
| Package distribution and release evidence | Public NuGet and internal consumption require reliable validation and rollback/recovery expectations. | Define proportionate release and package-validation evidence. |
| Future technical possibilities | OpenAPI, `ProblemDetails` and additional adapters may affect boundaries and support cost. | Evaluate only if later product evidence promotes them into scope. |

## Known Risks

| Risk | Impact | Notes |
| --- | --- | --- |
| Legacy MVC support demand is not evidenced | Support cost may exceed value. | Validate usage and support signals at the next product review. |
| ADR-006 is stale | Architecture and support boundaries are misrepresented. | Update or supersede through the decision process. |
| Quality and release evidence is incomplete | Confidence in public package readiness is limited. | Quality and Release stages must define and retain evidence. |
| Product documentation is more solution-focused than problem-focused | Future changes may optimise implementation without validated outcomes. | Preserve the Product Brief as the product baseline. |

## Discovery Assumptions

- Modern ASP.NET Core developers are the primary users.
- Legacy MVC developers are an equally supported user segment.
- Nestgrid internal use and public NuGet adoption are both intended outcomes.
- The current five-package implementation is the baseline for review, not an approved design for future changes.
- Roadmap candidates are technical possibilities only.

## Open Questions

- What exact framework versions and compatibility promises should be supported for each package?
- What evidence is sufficient to review the ongoing cost and value of MVC support?
- Are the current HTTP mappings and success/failure payload behaviours correct for the intended consumers?
- What architecture changes, if any, are required to maintain both modern and legacy adapters without semantic drift?
- What release, package-validation and support evidence should be mandatory before the next public version?

## Recommended Priorities

1. Validate the current package boundaries, framework targets, dependency support and stale ADR-006 against the approved product scope.
2. Define the compatibility, maintenance and operational model for both modern ASP.NET Core and legacy MVC consumers.
3. Confirm that core result semantics and adapter behaviour satisfy the Product Brief without expanding scope.
4. Record architectural decisions, risks and any required implementation initiatives before Engineering begins.
5. Leave OpenAPI, `ProblemDetails` and additional adapters deferred until Product Owner validation creates an approved product need.

## Approval

| Approved By | Date | Decision | Notes |
| --- | --- | --- | --- |
| Knight | 2026-08-14 | Approved | Proceed to Architecture review against the approved Product Brief. |
