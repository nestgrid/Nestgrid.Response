# Architecture Recommendation

```yaml
title: Nestgrid.Response v0.7.0 Architecture Recommendation
version: 1.1
status: Approved
owner: Solution Architect
contributors: Knight
produced_by: Solution Architect
consumed_by: Project Sponsor, Software Engineer
date: 2026-08-14
supersedes:
related_decisions:
  - ../../decisions/ADR-001-Result-Pattern-Philosophy.md
  - ../../decisions/ADR-002-Status-Driven-Results.md
  - ../../decisions/ADR-003-Immutable-Results.md
  - ../../decisions/ADR-004-AspNetCore-Separation.md
  - ../../decisions/ADR-005-Core-Object-Model.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../01 Discovery/Product Brief.md
  - ../01 Discovery/Architecture Handover.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Purpose

This recommendation defines the architectural direction for retrofitting the existing `v0.6.0` Nestgrid.Response implementation into the Nestgrid Engineering Operating System during `v0.7.0`.

The existing implementation is the baseline for assessment. It is not treated as an automatically approved Architecture Pack. The recommendation therefore separates confirmed architectural intent from decisions that must be made explicit before Engineering begins.

## Clarification Summary

No new product-intent clarification was required. The approved Product Brief and Architecture Handover establish the product scope, five-package baseline, equal modern/MVC support intent and deferred roadmap candidates.

Repository inspection confirms that the current implementation contains five packages and a shared HTTP mapping layer. The main architectural uncertainty is not the broad package shape; it is the durability and precision of the support, compatibility, operational and public-contract decisions around that shape.

| Question | Current clarification | Source |
| --- | --- | --- |
| Is the existing five-package product in scope? | Yes. It is the approved baseline for Architecture review, not proof that every current decision remains correct. | [Product Brief](../01%20Discovery/Product%20Brief.md), [Architecture Handover](../01%20Discovery/Architecture%20Handover.md) |
| Are OpenAPI, `ProblemDetails` and additional adapters in scope? | No. They remain deferred until a later Product decision. | [Product Brief](../01%20Discovery/Product%20Brief.md) |
| Is MVC support intended to continue? | Yes. It is an approved, equally supported segment, subject to Architecture defining the support boundary and compatibility policy. | [Architecture Handover](../01%20Discovery/Architecture%20Handover.md) |

## Proposed Architectural Direction

Retain Nestgrid.Response as a small, framework-independent result library with presentation concerns isolated behind adapter packages.

The recommended v0.7.0 direction is a controlled architectural retrofit:

1. Preserve the existing core result model, immutable result objects, status-driven semantics and lightweight functional extensions.
2. Preserve the shared HTTP mapping package as the single owner of transport mapping policy.
3. Preserve separate modern ASP.NET Core and legacy MVC adapters, subject to an explicit compatibility and maintenance policy.
4. Preserve the validation extension as an optional package with no expansion into a general validation framework.
5. Formalise the public API, status mappings, target-framework matrix, package dependencies, support boundaries and operational distribution model.
6. Avoid broad implementation redesign or new product capabilities during the retrofit.

The Architecture Pack should document the architecture as it is intended to be maintained, while identifying any implementation changes required to bring the current baseline into conformance.

## Key Architectural Decisions

| Decision Area | Recommendation | Rationale |
| --- | --- | --- |
| Core boundary | Keep `Nestgrid.Response` framework-independent and responsible for result semantics only. | Preserves portability and the approved separation of application outcomes from HTTP. |
| Package structure | Retain five packages: core, HTTP policy, ASP.NET Core adapter, MVC adapter and validation extension. | Matches the approved scope and current implementation while limiting dependency spread. |
| HTTP policy | Keep status-to-response mapping in `Nestgrid.Response.Http`; adapters execute framework-specific responses. | Prevents semantic drift between adapters and keeps transport concerns out of core. |
| Result semantics | Treat `ResultStatus` as the authoritative public outcome contract; retain immutable results and structured messages. | These are established public behaviours and are central to consumer compatibility. |
| MVC support | Continue support, but define supported framework versions, maintenance duration, compatibility expectations and review triggers. | Equal support is an approved product intent with material cost and compatibility consequences. |
| Public compatibility | Establish an API and behaviour compatibility policy before further broad changes or 1.0 planning. | Statuses, result shapes, mappings and package targets are consumer-facing contracts. |
| ADR-006 | Update or supersede ADR-006 to record the implemented MVC package, actual dependency and support boundary. | The current record is stale and conflicts with the implementation and approved Discovery artefacts. |
| Deferred capabilities | Do not add OpenAPI, `ProblemDetails` or additional adapters in this retrofit. | These capabilities are explicitly outside the approved scope. |

## Principal Risks and Trade-offs

| Risk or Trade-off | Impact | Proposed Response |
| --- | --- | --- |
| MVC support cost is not evidenced | Ongoing maintenance may exceed validated value. | Define support boundaries now and establish usage/support review signals for the next Product review. |
| Existing documentation and implementation have drifted | Engineers or consumers may rely on contradictory package and compatibility claims. | Reconcile ADR-006, package READMEs, root README and Architecture Pack. |
| Shared mappings simplify consistency but constrain adapter-specific behaviour | A mapping change can affect both supported adapter families. | Treat mapping behaviour as a versioned public contract and test both adapters against it. |
| Pre-1.0 public APIs remain compatibility-sensitive | Necessary improvements may cause consumer disruption. | Record compatibility rules, change classification and upgrade guidance. |
| Release evidence is incomplete | Architecture may be sound while public package confidence remains unproven. | Hand explicit build, package, test, coverage, mutation and release-evidence obligations to later Quality and Release stages. |
| Retrofit scope expands into redesign | Delivery becomes disconnected from the approved product intent. | Constrain v0.7.0 to architecture reconciliation and required conformance changes. |

## Open Questions

- Which exact target frameworks and dependency versions are supported for each package?
- What is the supported MVC compatibility range, maintenance duration and end-of-support policy?
- Which status-to-HTTP mappings are normative, and what is the compatibility policy for changing them?
- What response payload shapes are supported for full-result and value-only modes across both adapters?
- What minimum package, installation, upgrade, rollback and support guidance is required for a public release?
- Which current implementation discrepancies require Engineering changes rather than documentation correction?

These questions should be resolved or explicitly accepted in the Architecture Pack and related decisions. They must not be silently answered through implementation assumptions.

## Proposed Architecture Artefacts

The Execute stage should produce:

- Architecture Pack.
- Updated or superseding ADR-006.
- TDRs only where a technical choice needs more detailed, time-bounded traceability than an ADR provides.
- Architecture Feedback if the current baseline cannot satisfy the approved Product Brief without a Product decision.
- Engineering handover guidance identifying conformance changes, priorities, constraints and deferred work.

The Architecture Pack must explicitly cover:

- architecture principles and package boundaries;
- quality attributes and compatibility expectations;
- API and status contracts;
- HTTP mapping and adapter strategy;
- security and data considerations appropriate to a library product;
- packaging, distribution, installation, configuration, upgrade, rollback and support model;
- risks, trade-offs, open questions and Engineering guidance.

## Approval Gate

Architecture is not approved to Execute until the Project Sponsor accepts this direction.

| Approved By | Date | Decision | Notes |
| --- | --- | --- | --- |
| Knight — Project Sponsor | 2026-08-14 | Approved | MVC remains actively supported; compatibility is the default; breaking changes require justification and approval; the retrofit may include necessary defect fixes and implementation improvements; operational guidance should follow proportionate industry practice. |

## Recommendation

**Approved to proceed to Architecture Execute.**

The existing v0.6.0 implementation is a credible baseline with a coherent broad package architecture. This approval authorises the Architecture Pack, decision-record reconciliation and necessary conformance improvements. Engineering readiness remains a later Architecture Gate decision.
