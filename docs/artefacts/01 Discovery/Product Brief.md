# Product Brief

```yaml
title: Nestgrid.Response Product Brief
version: 1.0
status: Approved
owner: Product Owner
contributors: Knight
produced_by: Product Owner
consumed_by: Solution Architect
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
  - Opportunity Decision.md
  - Architecture Handover.md
```

## Purpose

Nestgrid.Response helps .NET developers represent expected application outcomes explicitly and consistently without coupling application logic to HTTP or relying on exceptions for routine business flow.

## Opportunity Context

The Opportunity Decision recommends pursuing Nestgrid.Response as an existing-product change. The implementation is coherent, the five-package scope is established, and the product has both internal Nestgrid and public NuGet relevance. Broader existing Result or functional libraries were informally considered; the selected position favours a small, direct library aligned with Nestgrid's needs. Discovery evidence is currently stronger for the intended solution and repository baseline than for measured user demand; this brief therefore preserves the existing intent while making that uncertainty explicit.

## Problem Statement

Developers building application services need to communicate successful, invalid, missing, conflicting, cancelled and failed outcomes across layers. Without a shared result model, teams may use inconsistent conventions, lose semantic detail through boolean success values, expose transport concerns in application code, or use exceptions for expected outcomes.

## Goals

- Make common application outcomes explicit, typed where useful and easy to inspect.
- Keep application and domain-facing result usage independent of HTTP and presentation frameworks.
- Provide consistent, well-documented adapters for modern ASP.NET Core and legacy ASP.NET Core MVC applications.
- Support both Nestgrid library consumers and public NuGet developers with a small, predictable product.
- Maintain confidence in public behaviour through tests, documentation and release evidence.

## Non-goals

- Becoming a general functional programming framework.
- Replacing exception handling for unexpected system failures.
- Becoming a validation framework, mediator, workflow engine or transport abstraction.
- Committing to OpenAPI helpers, `ProblemDetails` support or additional adapters without validated user need.
- Redesigning the current architecture as part of this Product Brief.

## Stakeholders

| Stakeholder | Role | Interest |
| --- | --- | --- |
| Nestgrid engineering teams | Internal consumers | Consistent application outcome handling across libraries and applications |
| Public .NET developers | External consumers | A small, understandable Result library available through NuGet |
| Modern ASP.NET Core developers | Primary user segment | Minimal API and controller response integration |
| Legacy ASP.NET Core MVC developers | Equal supported user segment | Continued compatibility and supported MVC response integration |
| Project Sponsor | Investment decision-maker | Product value, scope and sustainable support cost |
| Solution Architect | Downstream lifecycle owner | Clear product intent and architectural questions |

## Users and Personas

### Modern ASP.NET Core developer

Builds APIs using Minimal APIs or current ASP.NET Core controllers. Wants application services to return semantic outcomes and convert them into consistent HTTP responses at the boundary.

### Legacy MVC maintainer

Maintains an existing ASP.NET Core MVC application that cannot immediately move to the modern adapter baseline. Needs an actively supported MVC package with predictable behaviour and upgrade guidance.

### Framework-independent application developer

Builds a worker, library or application service without a presentation framework. Needs the core result model without taking a dependency on ASP.NET Core or HTTP concepts.

## Scope

- Core `Result` and `Result<T>` models, statuses, messages and factories.
- Functional helpers that preserve the documented result semantics.
- Shared HTTP mapping policy.
- Modern ASP.NET Core integration.
- Legacy ASP.NET Core MVC integration.
- DataAnnotations validation extensions.
- NuGet packaging, samples, consumer documentation and support guidance.
- EOS lifecycle artefacts and evidence needed to maintain the product responsibly.

## Out of Scope

- New adapters without validated demand.
- OpenAPI metadata generation or `ProblemDetails` implementation as an assumed feature.
- Persistence, authentication, hosting, workflow orchestration or domain-specific business rules.
- A broad migration of unrelated Nestgrid libraries into this repository.
- Architectural or implementation redesign during Product Definition.

## Functional Requirements

| ID | Requirement | Priority |
| --- | --- | --- |
| FR-001 | Consumers must be able to represent common successful and non-successful application outcomes with explicit semantic statuses. | Must |
| FR-002 | Consumers must be able to represent both outcomes with and without a value. | Must |
| FR-003 | Consumers must be able to attach human-readable and structured messages to outcomes. | Must |
| FR-004 | Consumers must be able to distinguish overall success from failure and inspect the specific status when required. | Must |
| FR-005 | Consumers must be able to transform successful values without losing the documented result outcome and messages. | Should |
| FR-006 | Modern ASP.NET Core consumers must be able to convert results to framework responses. | Must |
| FR-007 | Legacy ASP.NET Core MVC consumers must be able to convert results to framework responses through an actively supported package. | Must |
| FR-008 | Consumers using only application or domain code must be able to use the core package without a presentation-framework dependency. | Must |
| FR-009 | Validation consumers must be able to convert DataAnnotations validation results into invalid result messages. | Should |

## Non-functional Requirements

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-001 | Public behaviour and terminology must remain small, predictable and understandable without adopting a larger framework. | Must |
| NFR-002 | The core package must remain independent of HTTP and presentation frameworks. | Must |
| NFR-003 | Public result objects must preserve the documented immutability and status semantics. | Must |
| NFR-004 | Supported target frameworks and package dependencies must be explicit and maintained as a support policy. | Must |
| NFR-005 | Public API or behaviour changes must have proportionate compatibility, test and documentation evidence. | Must |
| NFR-006 | Product documentation must distinguish supported capabilities from technical possibilities and future candidates. | Should |

## Operational Requirements

| ID | Requirement | Priority |
| --- | --- | --- |
| OR-001 | Packages must be consumable through the intended NuGet distribution model with installation and package-selection guidance. | Must |
| OR-002 | Each supported package must document its target runtime/framework, dependencies, intended consumers and support boundary. | Must |
| OR-003 | Versioning, compatibility expectations and upgrade guidance must be documented for the pre-1.0 and eventual stable-release phases. | Must |
| OR-004 | The product must retain release evidence appropriate to a public library, including build, test, coverage, mutation and package validation evidence where applicable. | Should |
| OR-005 | Samples must provide runnable validation of the primary modern, legacy and framework-independent consumption paths. | Should |
| OR-006 | Support ownership and the process for handling consumer issues must be explicit for all active packages, including MVC. | Should |

## Acceptance Criteria

- The Product Brief is approved by the Project Sponsor as the current product definition.
- The five-package baseline and the equal support commitment for modern ASP.NET Core and legacy MVC are explicitly accepted.
- Product scope excludes unvalidated roadmap possibilities unless a later Product Decision promotes them.
- Functional, quality and operational requirements are clear enough for Architecture to identify decisions without inventing product intent.
- Known uncertainties and technical questions are handed over rather than resolved through Product Definition.
- The Architecture Handover is approved as the next lifecycle input.

## Assumptions

- Modern ASP.NET Core is the primary user segment.
- Legacy ASP.NET Core MVC remains a material supported segment with ongoing maintenance value.
- Nestgrid internal reuse and public NuGet adoption are both intended outcomes.
- The current implementation is a baseline to assess, not proof that every requirement is complete or correct.
- OpenAPI, `ProblemDetails` and additional adapters are not commitments until validated.

## Constraints

- The core product must remain small and framework-independent.
- Public APIs and status semantics are compatibility-sensitive.
- The product is distributed as .NET packages with multiple target-framework and dependency considerations.
- Product Discovery must not make architectural or implementation decisions.

## Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| Legacy MVC support costs more than its validated value | Ongoing maintenance burden or reduced modern focus | Establish support evidence, compatibility policy and review triggers with Architecture and Sponsor |
| Product intent is inferred from implementation rather than user evidence | Investment may continue without sufficient adoption or value | Gather consumer, usage and support evidence during the next product review |
| Technical possibilities become roadmap commitments | Scope expands without outcome evidence | Keep OpenAPI, `ProblemDetails` and new adapters explicitly uncommitted |
| Stale ADR or package documentation misleads consumers | Incorrect support or architecture assumptions | Reconcile ADR-006 and run documentation/link review as follow-up work |
| Release evidence remains incomplete | Public package confidence is weaker than stated targets | Retain quality and release evidence before future release decisions |

## Open Questions

- What support duration and compatibility policy should apply to the MVC package?
- Which consumer and adoption signals should determine whether both support segments continue at the next review?
- What minimum package compatibility matrix should be published for each supported package?
- Which operational evidence is required for each future public release?
- Which existing libraries were included in the informal comparison, and what specific trade-offs led to the small-and-direct position?

## Recommendation

Approve this Product Brief as the baseline for Architecture review, subject to recording the support policy and compatibility evidence as architectural follow-up. Keep the current product scope stable while validating usage and support demand. Do not promote roadmap candidates into approved capability without a new product decision.

## Approval

| Approved By | Date | Decision | Notes |
| --- | --- | --- | --- |
| Knight | 2026-08-14 | Approved | Current product intent and five-package baseline approved for Architecture review. |
