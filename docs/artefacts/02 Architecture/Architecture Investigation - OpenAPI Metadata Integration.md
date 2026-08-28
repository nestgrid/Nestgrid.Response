# Architecture Investigation — OpenAPI Metadata Integration

```yaml
title: Nestgrid.Response OpenAPI Metadata Integration Investigation
version: 1.0
status: Closed — findings recorded; no architectural decision made
owner: Solution Architect
contributors:
  - Knight
produced_by: Solution Architect
consumed_by: Project Sponsor, Solution Architect, Software Engineer, Quality Engineer
date: 2026-08-20
supersedes:
related_decisions:
  - ../../decisions/ADR-004-AspNetCore-Separation.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Recommendation.md
  - Architecture Pack.md
  - Engineering Handover.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Purpose

This investigation assessed whether endpoints returning Nestgrid.Response through `ToIResult()` and `ToActionResult()` can accurately expose success and failure response metadata to ASP.NET Core OpenAPI tooling without consumers manually duplicating Nestgrid.Response HTTP mapping knowledge.

The investigation covered:

- `Nestgrid.Response.AspNetCore` and `ToIResult()`;
- `Nestgrid.Response.Mvc` and `ToActionResult()`;
- `FullResult` and `ValueOnly` response modes;
- generic and non-generic results;
- default and custom status mappings;
- success and failure schemas and status codes;
- Minimal APIs and MVC controller actions; and
- Swagger/Scalar rendering considerations.

No product implementation or public API was approved by this investigation.

## Evidence Reviewed

- Approved v0.7.0 Architecture Recommendation and Architecture Pack.
- ADR-004 and ADR-006 package-boundary decisions.
- Existing `Nestgrid.Response.Http` mapping and options implementation.
- Existing ASP.NET Core and MVC adapter implementations.
- Existing ASP.NET Core and MVC samples.
- Native ASP.NET Core OpenAPI metadata guidance:
  - [Include OpenAPI metadata in an ASP.NET Core app](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-10.0)
  - [Create responses in Minimal API applications](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-10.0)
  - [`IEndpointMetadataProvider`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.metadata.iendpointmetadataprovider?view=aspnetcore-10.0)
- A temporary Swagger/Scalar proving harness applied to the existing web samples. Both samples built successfully with zero warnings and errors. The temporary changes were removed after investigation because retaining them would imply unapproved OpenAPI product scope. Runtime HTTP probing could not complete because the local sample host did not bind in the available environment.

## Findings

### Current Minimal API adapter

`ToIResult()` returns `IResult`. The concrete internal `ResponseAspNetCoreResult` performs response mapping and JSON execution at request time, but does not implement `IEndpointMetadataProvider` or provide endpoint response metadata.

Consequently, the endpoint declaration does not statically expose:

- the possible result status codes;
- the response content types;
- whether a success response is a full result envelope or a value-only payload; or
- the value type for a generic success response.

The framework cannot infer those facts from the runtime object that will later be returned by `ExecuteAsync`.

### Current MVC adapter

`ToActionResult()` returns `IActionResult`. The concrete internal `ResponseMvcActionResult` maps and executes the response at request time, but does not provide MVC API response metadata.

MVC therefore requires action-level metadata, conventions or a metadata-bearing action return type to describe response status codes and schemas. The existing `IActionResult` signature does not provide enough information for accurate inference.

### Actual response representation

The OpenAPI contract must describe the serialized response body, not the application’s `Result<T>` type in isolation.

| Mode / result | Successful response body | Failure response body |
| --- | --- | --- |
| `FullResult`, generic | `Result<T>` envelope containing the value and messages | `Result<T>` envelope, normally with no value |
| `FullResult`, non-generic | `Result` envelope containing messages | `Result` envelope containing messages |
| `ValueOnly`, generic success | `T` value only | `Result<T>` envelope |
| `ValueOnly`, non-generic | `Result` envelope; there is no value to unwrap | `Result` envelope |
| Any `NoContent` mapping | No response body | No response body |

This creates different schemas for different statuses in `ValueOnly` mode. A single endpoint may therefore need both a value schema and one or more result-envelope schemas.

### Status mapping

Default mappings are defined in `Nestgrid.Response.Http`, but consumers may change them globally or per call. The adapters resolve effective options during request execution.

This prevents reliable automatic metadata generation for arbitrary custom mappings. OpenAPI generation happens at application startup or document-generation time, while the actual mapping can be supplied later or vary between calls.

### Native framework mechanisms

The native mechanisms are capable of representing the desired information, but they require statically available metadata:

- Minimal APIs can use typed results, `Results<T1,...>` unions, endpoint `.Produces(...)` metadata or a return type implementing `IEndpointMetadataProvider`.
- MVC can use `[ProducesResponseType]`, `ProducesAttribute`, API conventions or other response metadata providers.
- These mechanisms describe the endpoint contract; they do not enforce that runtime behaviour matches the declared contract.

They do not automatically derive the Nestgrid.Response mapping policy from the current `IResult` or `IActionResult` adapters.

## Architectural Constraints

Any future design must preserve the following constraints:

1. `Nestgrid.Response` remains framework-independent.
2. HTTP mapping policy remains owned by `Nestgrid.Response.Http`.
3. ASP.NET Core and MVC adapter boundaries remain separate.
4. Existing `ToIResult()` and `ToActionResult()` behaviour remains compatible by default.
5. `FullResult` and `ValueOnly` remain explicit and accurately represented.
6. Generic and non-generic result contracts remain distinguishable.
7. Default mappings remain stable unless separately approved.
8. Custom mappings must not be documented as defaults or silently ignored.
9. Metadata must describe actual wire representations, including empty-body responses.
10. OpenAPI support remains outside the approved v0.7.0 scope until a new Product and Architecture decision authorises it.
11. Any new public API must be additive unless a justified and approved breaking change is required.
12. Swagger/Scalar and other document-rendering dependencies must remain sample/application concerns unless a package-boundary decision explicitly changes that position.

## Options Identified

### Option A — Documentation-only guidance

Continue requiring consumers to use native `.Produces(...)` and `[ProducesResponseType]` declarations.

**Benefits:** no package changes, no compatibility risk and immediate use of existing framework mechanisms.

**Limitations:** consumers must duplicate status mappings, modes and response schemas. This does not satisfy the original objective.

### Option B — Metadata-bearing adapter result types

Introduce concrete adapter result types that execute the response and expose native metadata, potentially through `IEndpointMetadataProvider` and MVC-compatible metadata contracts.

**Benefits:** metadata can remain close to adapter behaviour and Minimal API declarations could become self-describing.

**Limitations:** existing methods return `IResult`/`IActionResult`; changing their effective public shape may create source, binary or generic-union compatibility concerns. Per-call options and arbitrary custom mappings still require a static metadata input.

### Option C — Explicit endpoint metadata registration

Additive adapter APIs could attach a Nestgrid.Response metadata descriptor to a Minimal API endpoint or MVC action/convention. The descriptor would reference the result value type, response mode and mapping policy rather than making consumers repeat raw status knowledge.

**Benefits:** preserves current conversion methods, makes the static contract explicit, supports custom mappings and keeps metadata generation separate from runtime result execution.

**Limitations:** consumers still perform an explicit registration step. The descriptor must be designed carefully to avoid duplicating or diverging from `Nestgrid.Response.Http` policy.

### Option D — Global conventions or application-wide inference

Register conventions that infer metadata from application configuration, controller signatures or known Nestgrid.Response patterns.

**Benefits:** less endpoint-level ceremony for consistent applications.

**Limitations:** difficult to reconcile with per-call options, custom mappings, mixed response modes and arbitrary application methods. Convention precedence and debugging would also need careful design.

### Option E — Source generation or compile-time endpoint analysis

Generate metadata from endpoint declarations and known result contracts.

**Benefits:** potentially strong compile-time accuracy and reduced runtime reflection.

**Limitations:** highest complexity, toolchain dependency and maintenance cost; cannot infer runtime custom mappings that are not statically declared.

## Current Architectural Position

The investigation establishes that the existing adapters cannot meet the requested OpenAPI accuracy automatically. It also establishes that native ASP.NET Core metadata mechanisms are the correct integration points for any future design.

The investigation does not select Options A–E, approve a public API, change package scope, alter the Architecture Pack or authorise implementation.

The likely design space is a combination of Option B and Option C, but that is an observation for a future Architecture Recommendation, not an architectural decision.

## Closure

Investigation status: **Closed — findings recorded; no architectural decision made.**

Before implementation, a new Architecture Recommendation should compare the viable options against compatibility, package boundaries, custom mapping support, schema accuracy, MVC/Minimal API parity, usability and maintenance cost. Project Sponsor approval is required before Architecture execution or public API design proceeds.
