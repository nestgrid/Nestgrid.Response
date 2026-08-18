# Opportunity Decision

```yaml
title: Nestgrid.Response Existing Product Opportunity Decision
version: 1.0
status: Approved
owner: Product Owner
contributors: Knight
produced_by: Product Owner
consumed_by: Project Sponsor
date: 2026-08-14
supersedes:
related_decisions:
  - ../../decisions/ADR-001-Result-Pattern-Philosophy.md
  - ../../decisions/ADR-002-Status-Driven-Results.md
  - ../../decisions/ADR-004-AspNetCore-Separation.md
  - ../../decisions/ADR-005-Core-Object-Model.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Product Brief.md
  - Architecture Handover.md
```

## Idea or Observed Problem

Application developers need a small, predictable way to represent expected operation outcomes across application boundaries. Existing approaches can rely on exceptions for routine business outcomes, boolean success flags that lose semantic detail, or transport-specific response types that couple application code to HTTP.

Nestgrid.Response addresses this as an existing .NET library product. It provides explicit result statuses, optional structured messages, typed and untyped outcomes, and separate presentation adapters.

## Conversation Summary

- Modern ASP.NET Core developers are the primary users.
- Developers maintaining legacy ASP.NET Core MVC applications are an equally supported user segment.
- `Nestgrid.Response.Mvc` remains an active, supported capability rather than a compatibility experiment.
- The product serves both Nestgrid's internal library ecosystem and public NuGet consumers.
- OpenAPI helpers, `ProblemDetails` guidance and broader adapter evaluation remain technical possibilities, not approved product commitments.

## Emerging Shared Understanding

The product intent remains valid. The product should remain a focused Result library rather than expand into a general functional programming, validation, mediator, workflow or transport framework.

The current five-package product set is the baseline for Discovery and Architecture review. Existing implementation correctness does not by itself establish product-market need, support obligations or release readiness.

## Users or Stakeholders

- Modern ASP.NET Core application developers.
- Developers maintaining older ASP.NET Core MVC applications.
- Developers of framework-independent application, worker and library code.
- Nestgrid teams consuming shared libraries.
- Public NuGet consumers.
- Product Owner and Project Sponsor, accountable for product direction and investment.
- Solution Architect, responsible for resolving architectural questions after Discovery.

## Desired Outcome

Developers can communicate expected application outcomes consistently and explicitly, while keeping core application code independent of HTTP and avoiding unnecessary adoption of a larger framework.

## Evidence Considered

### Sourced evidence

- The current repository contains five source packages, five test projects and four samples.
- The root README, package READMEs, handbooks and ADRs consistently describe a lightweight Result pattern library.
- The MVC adapter is implemented, packaged, sampled and tested.
- `dotnet test Nestgrid.Response.sln -c Release` passed 265 tests with no failures on 2026-08-14.
- The current canonical Independent Review recommends controlled EOS retrofit discovery and identifies missing product-level lifecycle artefacts.

### Inference

- The product has a coherent technical position and a plausible developer problem.
- The repository's existence, public NuGet packaging and dual modern/legacy support indicate an intention to serve both internal and external consumers.
- User need, adoption level and support demand have not yet been validated with usage evidence or stakeholder research.
- The Product Owner's prior comparison indicated that broader existing Result or functional libraries did not fit the desired small and direct product position as well as a focused Nestgrid library.

## Assumptions and Uncertainties

- Both modern ASP.NET Core and legacy MVC support justify ongoing maintenance.
- Public NuGet adoption and Nestgrid internal reuse are both worthwhile outcomes.
- Existing statuses, package boundaries and public APIs are an adequate starting point for Architecture review.
- No usage, download, support or consumer feedback data was available in the repository review.

## Alternative Framings

- Continue as the existing library product: recommended.
- Reframe as an internal-only Nestgrid capability: rejected because public NuGet use is also an intended outcome.
- Reduce to a modern ASP.NET Core-only library: rejected because legacy MVC support is explicitly retained.
- Expand into a broader application framework: rejected because it conflicts with the stated small and predictable product intent.
- Reuse a broader existing Result or functional library: not selected after a proportionate comparison; the small and direct Nestgrid position remains preferable.
- Defer or stop: not recommended while the product intent remains valid and the implementation baseline is coherent.

## Existing Solutions Considered

The repository records exceptions, boolean success flags, HTTP-coupled models and broader functional abstractions as alternatives or rejected approaches. A proportionate external comparison was recorded on 2026-08-14 using public package documentation:

| Existing solution | Observed position | Trade-off against Nestgrid.Response |
| --- | --- | --- |
| [FluentResults](https://www.nuget.org/packages/FluentResults) | Extensible reason, error and success abstractions with an ASP.NET Core extension ecosystem. | Broader and more extensible, but more abstraction than the intentionally small Nestgrid model requires. |
| [ErrorOr](https://www.nuget.org/packages/ErrorOr) | Typed value-or-error model with error-oriented functional operations and custom error support. | Strong typed error modelling, but a different error-first model from Nestgrid's fixed semantic status model. |
| [CSharpFunctionalExtensions](https://www.nuget.org/packages/CSharpFunctionalExtensions) | Functional library covering Result, Maybe, value objects and related extensions. | Wider functional scope than required for a direct application-outcome library. |
| [Ardalis.Result](https://www.nuget.org/packages/Ardalis.Result) | Result pattern library with validation/error concepts and ASP.NET Core integration. | Relevant overlap, but the existing Nestgrid package baseline favours a smaller, Nestgrid-owned contract. |
| [OneOf](https://www.nuget.org/packages/OneOf) | General discriminated-union type for representing one of several types. | Useful for unions, but not a direct semantic application-result and HTTP-adapter product. |

The comparison was not a feature-for-feature benchmark. The decisive product criterion was a small, direct library aligned with Nestgrid's needs rather than the breadth of a general-purpose framework.

## Opportunity Assessment

The opportunity deserves continued engineering investment as an existing-product change and EOS retrofit. The next work should establish product definition, support boundaries, operational expectations and architectural questions before further capability expansion.

## Decision

**Pursue as an existing-product change.**

## Rationale

The product has a clear, bounded intent, an implemented baseline and a credible developer audience. The principal deficiency is not an absence of a product opportunity, but insufficient Discovery evidence and lifecycle traceability. Continuing with controlled product definition preserves the value already created while preventing roadmap possibilities from becoming unvalidated commitments.

## Next Action or Reconsideration Trigger

Reconsider the product if evidence shows that modern and legacy consumers do not obtain sufficient value to justify maintaining both support segments, or if a better existing solution meets the same need with materially lower cost and risk.

## Approval

| Approved By | Date | Decision | Notes |
| --- | --- | --- | --- |
| Knight — Project Sponsor | 2026-08-14 | Approved | Continue as an existing-product change with the current focused scope; authorise Architecture review and accept the documented stage risks and open questions. |
