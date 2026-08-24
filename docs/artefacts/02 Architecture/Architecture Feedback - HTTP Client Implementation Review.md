# Architecture Feedback — HTTP Client Implementation Review

```yaml
title: Nestgrid.Response v0.8.0 Architecture Feedback - HTTP Client Implementation Review
version: 1.0
status: Conditional approval for Quality and Security review
owner: Solution Architect
contributors:
  - Knight
produced_by: Solution Architect
consumed_by: Software Engineer, Quality Engineer, Security Engineer, Project Sponsor
date: 2026-08-24
supersedes:
related_decisions:
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Representation.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Policy.md
related_work_items:
  - HTTP client capability
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Pack.md
  - Architecture Recommendation - HTTP Client Capability.md
  - Engineering Handover.md
  - ../03 Implementation/Implementation Plan - HTTP Client Capability.md
  - ../03 Implementation/Implementation Report - HTTP Client Capability.md
```

## Purpose

This feedback records Architecture's review of the Engineering implementation of `Nestgrid.Response.Http.Client`. It confirms the architectural direction, identifies conditions for implementation correction and evidence completion, and provides Mason with the required handover.

This artefact does not replace the Engineering Implementation Report, Quality assessment, Security assessment or release approval.

## Architectural assessment

The implementation remains aligned with ADR-009 through ADR-011 in the following respects:

- the client capability is an additive sibling package;
- the package remains independent of ASP.NET Core, MVC, authentication, DI and consumer-specific proxy infrastructure;
- `FullResult` and `ValueOnly` are explicit contracts;
- internal wire DTOs are used rather than deserialising core `Result` types;
- HTTP status is authoritative on the client;
- expected HTTP outcomes are represented as client-side `Result` values;
- transport and cancellation failures remain standard client exceptions in principle; and
- server-side status mappings are not reversed.

The package boundary and core Result model do not require architectural change.

## Required Engineering corrections

### Serializer isolation

The client options claim immutable serializer behaviour, but the current implementation does not preserve all relevant serializer configuration, including custom converters, and exposes a mutable `JsonSerializerOptions` instance used by the reader.

Engineering must establish a complete supported snapshot or isolation strategy, document the supported settings, and add tests proving that custom converters are preserved and later caller mutation cannot change an already constructed reader's behaviour.

### Cancellation during content reading

The reader checks cancellation before and after body reading, but the current content-read operation does not receive the caller's cancellation token. Cancellation requested during a delayed body read may therefore not be observed promptly.

Engineering must implement cancellable content reading for the supported target framework or document the framework limitation, add a regression test and obtain explicit acceptance of the resulting behaviour.

### Representation and media type contract

The capability is intentionally JSON-based through `System.Text.Json`. XML, plain text and other formats are outside the approved package scope. The implementation currently does not make its media-type behaviour explicit.

Engineering must document and test the accepted JSON media-type policy, including whether a missing media type is accepted. Non-JSON support must not be introduced implicitly as part of this correction.

### Non-generic ValueOnly behaviour

The generic `ValueOnly` success path represents a direct wire value and constructs a client-side `Result<T>`. The non-generic path must have an equally explicit contract. Engineering must either support the approved non-generic `ValueOnly` cases or document the supported restriction and add tests. The implementation must not infer the representation from the JSON shape.

### Consumer-facing documentation

The package README is a public product surface and must be expanded before publication. It should cover installation, explicit payload modes, generic and non-generic usage, the distinction between wire value and returned `Result<T>`, status mappings, structured messages, protocol versus transport failures, JSON/media-type scope, response ownership, and standard `HttpClient`/`IHttpClientFactory` composition.

The convenience API name should communicate that it sends and interprets the response. Architecture recommends `SendAndReadNestgridResponseAsync` for the thin convenience operation, with `NestgridResponseReader.ReadAsync` remaining the composable lower-level boundary. Any rename is an Engineering compatibility decision and must be reflected consistently in tests, samples, XML documentation and the package README.

## Evidence conditions

The current build and test baseline is encouraging, but the implementation evidence should be extended for:

- custom serializer converters and mutation isolation;
- cancellation during body reading;
- missing, null or invalid message collections;
- invalid message severity;
- non-generic `ValueOnly` behaviour;
- explicit media-type handling; and
- convenience-method request and response ownership.

The sample is suitable as a protocol and standard-`HttpClient` proving scenario. It does not constitute live authentication, transport, resilience or production endpoint evidence, which remain consumer responsibilities.

## Decision and gate position

Architecture does not identify a need to alter the approved package boundary, core Result model or existing server packages.

Architecture supports progression to Quality and Security review conditionally. The serializer isolation and cancellation items are implementation corrections unless Mason documents a framework limitation and obtains acceptance. The media-type contract, non-generic ValueOnly contract, README and additional tests are required clarity and evidence work.

This feedback is not final API approval for 1.0 and is not release approval for v0.8.0.

## Handover to Engineering

Mason should resolve or explicitly disposition each condition in the Implementation Report. Architecture should be re-engaged if the changes alter the approved public contract, introduce non-JSON support, change exception semantics, weaken the core Result construction boundary or add a new dependency boundary.
