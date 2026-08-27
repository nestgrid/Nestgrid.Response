# Architecture Feedback — HTTP Client Implementation Review

```yaml
title: Nestgrid.Response v0.8.0 Architecture Feedback - HTTP Client Implementation Review
eos_version: 1.1.0
version: 1.2
status: Reviewed against Independent Review v2.5; downstream release conditions remain
owner: Solution Architect
contributors:
  - Knight
produced_by: Solution Architect
consumed_by: Software Engineer, Quality Engineer, Security Engineer, Project Sponsor
date: 2026-08-27
supersedes:
related_decisions:
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
  - ../../decisions/ADR-012-HTTP-Client-Safety-Boundaries.md
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

This review also records Architecture's response to the current canonical
[Independent Review](../../reviews/Nestgrid.Response%20Independent%20Review.md)
v2.5. The Independent Review remains the authoritative independent findings
register; this document records only the responsible-role architectural
disposition and handover.

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

The latest implementation and downstream reviews do not identify a change to
the approved client boundary. SEC-007, SEC-008 and SEC-009 are recorded as
resolved by Engineering and Security for the evaluated candidate. This does
not establish final release evidence: IR-016 remains open until supported-CI,
protected-publication and immutable provenance evidence are recorded by the
responsible roles.

The Independent Review's IR-011 through IR-015 are legitimate pre-1.0 API
governance findings, not implementation authorisation. They remain open for
the authorised Architecture/Product/Sponsor decision path and must not be
silently resolved by the HTTP client package. IR-018 is likewise a contract
decision: the current implementation should not be described as preserving a
complete `JsonSerializerOptions` contract until the supported subset or full
snapshot behaviour is explicitly decided and evidenced.

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

## Independent Review dispositions and handover

| Finding | Architecture disposition | Responsible next owner | Gate position |
| --- | --- | --- | --- |
| IR-011 | Keep open. Decide whether public `Result` derivation is supported and define deterministic handling for any supported extension before the 1.0 baseline. | Solution Architect / Software Engineer / Project Sponsor | 1.0 API baseline; not a v0.8 implementation blocker unless the client relies on the decision. |
| IR-012 | Keep open. Require a six-package public API inventory and repeatable compatibility gate before 1.0 freeze. | Solution Architect / Software Engineer / Quality Engineer | 1.0 readiness. |
| IR-013 | Keep open. Freeze the existing nullable and `NoContent<T>` callback behaviour through an explicit decision and contract tests, or approve a separately governed change. | Solution Architect / Software Engineer / Project Sponsor | 1.0 API decision. |
| IR-014 | Keep open. Obtain Product/Sponsor confirmation of the normative default HTTP semantics, especially `Cancelled` and `Failed`, before freezing them. | Product Owner / Solution Architect / Project Sponsor | 1.0 API decision. |
| IR-015 | Keep open. Assess the public mapper invariant and choose validation or a compatible replacement before 1.0. | Solution Architect / Software Engineer | 1.0 API decision. |
| IR-016 | Keep open. No release approval is implied by this feedback; Quality, Platform and Release must complete the protected evidence chain. | Quality / Platform / Release / Project Sponsor | v0.8 release blocker. |
| IR-017 | Accepted as a documentation correction. The roadmap must distinguish the released v0.7 five-package baseline from the v0.8 six-package candidate. | Solution Architect / Release Owner | Traceability correction. |
| IR-018 | Keep open. The serializer-options API must explicitly define its supported snapshot contract before 1.0; Engineering must not claim complete preservation from the current selective copy. | Solution Architect / Software Engineer / Quality Engineer | 1.0 API/behaviour decision; current claim requires correction. |
| IR-019 | Resolved by this revision and the linked implementation-report correction: canonical ADR filenames are used. | Solution Architect | Traceability complete after link check. |

These dispositions do not accept risk, approve publication or close findings in
another role's artefact. They provide the handover required by the EOS review
gate.

## Security remediation architecture

Sponsor approval was granted on 2026-08-26 for the delivery-channel boundary and the security remediation direction recorded in [ADR-012](../../decisions/ADR-012-HTTP-Client-Safety-Boundaries.md).

Mason may proceed with SEC-007 and SEC-008 implementation subject to that decision:

- preserve API-provided failure statuses and structured messages as client-side results;
- add the bounded response policy with a 1 MiB default and explicit larger positive limits;
- enforce the limit during streaming for success and failure responses;
- keep protocol exceptions for local processing failures only;
- remove public inner-exception and diagnostic disclosure from serializer/converter failures; and
- preserve safe status code and payload-mode context where applicable.

The response-size option and any exception-constructor change are part of the additive pre-1.0 client package and must be recorded in the Implementation Report and reviewed by Quality and Security. Sponsor approval for this direction is recorded in the current Architecture workflow.

## Handover to Engineering

Mason should resolve or explicitly disposition each condition in the Implementation Report. Architecture should be re-engaged if the changes alter the approved public contract, introduce non-JSON support, change exception semantics, weaken the core Result construction boundary or add a new dependency boundary.

## Current handover outcome

Architecture is complete for the current implementation review and supports
conditional progression to downstream validation and Release review. The next
required evidence is the supported-CI/provenance chain for v0.8.0 and the
separately governed pre-1.0 API decisions identified above. A subsequent
Architecture review is required if IR-011, IR-013, IR-014, IR-015 or IR-018 is
resolved through a public or semantic contract change.
