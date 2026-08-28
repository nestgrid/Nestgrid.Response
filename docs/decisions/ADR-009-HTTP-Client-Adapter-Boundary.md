# ADR-009: HTTP Client Adapter Boundary

> Decision record for **Architecture**.

## Status

Accepted

## Type

Architecture

## Date

2026-08-21

## Owners

- Solution Architect
- Project Sponsor
- Software Engineer

## Context

Nestgrid consumers need reusable handling for the boundary from an HTTP response to `Result` or `Result<T>`. The existing packages intentionally address the opposite direction: result outcomes to HTTP responses. The requirement is shared by more than one independent consumer and must not introduce consumer-specific, authentication-specific or generic HTTP-client infrastructure into Nestgrid.Response.

The existing `Nestgrid.Response.Http` package owns server-side status mapping and response-shape policy. Reversing that mapping would be incorrect because the client observes HTTP and cannot reliably reconstruct the server-side `ResultStatus` that produced it.

## Decision

Create a separate additive HTTP client adapter package with the recommended package identity:

```text
Nestgrid.Response.Http.Client
```

The package is a sibling of `Nestgrid.Response.Http`, not an extension of its server mapper. It depends on `Nestgrid.Response` and the supported JSON serializer, but not on ASP.NET Core, MVC, authentication packages, `IHttpClientFactory` or consumer-specific infrastructure.

The package provides:

- explicit client payload-mode policy;
- HTTP response interpretation;
- wire-envelope deserialisation;
- client-side HTTP-outcome mapping; and
- construction of core results through existing public factories.

It does not provide a generic Nestgrid HTTP client abstraction, authentication, retries, resilience, logging, telemetry, endpoint discovery or a global `DelegatingHandler`.

The package name is the Architecture recommendation and must be treated as the intended public identity unless Product identifies a naming conflict before implementation.

## Rationale

The boundary is sufficiently reusable to belong in the ecosystem, but transport direction and serializer concerns do not belong in core. A separate package limits dependency spread, avoids destabilising the established server mapper and permits client semantics to evolve additively.

Standard `HttpClient` and `IHttpClientFactory` composition remains available to consumers. Authentication and other handlers therefore remain in the normal application-owned pipeline.

## Consequences

- Existing five packages remain unchanged in responsibility and public behaviour.
- A sixth package and its tests, documentation and package evidence are introduced additively.
- The client package must document that it interprets HTTP outcomes rather than reversing server status mappings.
- Package version alignment with the existing ecosystem is a release-governance decision; the package must not be silently added to the already-published 0.7.0 package set.
- No DI integration package is required for the first implementation.

## Alternatives Considered

### Add client behaviour to `Nestgrid.Response.Http`

Rejected because it would mix opposite boundary directions, increase dependency and compatibility pressure, and encourage incorrect reverse mapping.

### Add client behaviour to core

Rejected because it would couple the result model to `HttpClient`, JSON and transport concerns.

### Use a global `DelegatingHandler`

Rejected as the primary API because a handler cannot know the endpoint's target type, payload mode or response contract and would make global response consumption surprising.

### Leave all handling to consumers

Rejected as the preferred direction because repeated implementations would duplicate wire and message handling and produce inconsistent semantics across consumers.

## Related Decisions

- [ADR-001 Result Pattern Philosophy](ADR-001-Result-Pattern-Philosophy.md)
- [ADR-002 Status-Driven Results](ADR-002-Status-Driven-Results.md)
- [ADR-004 ASP.NET Core Separation](ADR-004-AspNetCore-Separation.md)
- [ADR-005 Core Object Model](ADR-005-Core-Object-Model.md)
- [ADR-006 ASP.NET Core and MVC Package Separation](ADR-006-AspNetCore-And-Mvc-Package-Separation.md)
- [ADR-010 Client Wire Contract and Result Construction](ADR-010-HTTP-Client-Wire-Contract.md)
- [ADR-011 Client HTTP Outcome Semantics](ADR-011-HTTP-Client-Outcome-Semantics.md)
