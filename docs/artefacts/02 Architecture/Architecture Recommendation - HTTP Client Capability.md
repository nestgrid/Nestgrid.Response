# Architecture Assessment and Recommendation — HTTP Client Capability

```yaml
title: Nestgrid.Response HTTP Client Capability Architecture Assessment and Recommendation
eos_version: 1.1.0
version: 1.1
status: Approved direction — detailed Architecture authorised
owner: Solution Architect
contributors:
  - Knight
produced_by: Solution Architect
consumed_by:
  - Project Sponsor
  - Product Owner
  - Software Engineer
  - Quality Engineer
  - Security Engineer
date: 2026-08-21
approval: Project Sponsor approved progression beyond Recommend on 2026-08-21
related_decisions:
  - ../../decisions/ADR-001-Result-Pattern-Philosophy.md
  - ../../decisions/ADR-002-Status-Driven-Results.md
  - ../../decisions/ADR-004-AspNetCore-Separation.md
  - ../../decisions/ADR-005-Core-Object-Model.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
  - ../../decisions/ADR-012-HTTP-Client-Safety-Boundaries.md
related_artefacts:
  - ../01 Discovery/Product Brief.md
  - ../01 Discovery/Architecture Handover.md
  - Architecture Recommendation.md
  - Architecture Pack.md
  - Engineering Handover.md
  - Architecture Investigation - OpenAPI Metadata Integration.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Purpose

This assessment considers whether Nestgrid.Response should provide reusable client-side handling for the boundary:

```text
HTTP response -> Result / Result<T>
```

The working package name `Nestgrid.Response.Http.Client` is only a candidate. It is not an approved package name, API or implementation direction.

The assessment is intentionally separate from the existing server-side adapters, which address:

```text
Result / Result<T> -> HTTP response
```

At the original Recommend stage, no implementation, public API approval, ADR or change to the approved Architecture Pack was authorised by this document. Sponsor approval recorded on 2026-08-21 authorises the detailed Architecture work captured by the related ADRs, Architecture Pack revision and Engineering handover.

## Established requirements

The following requirements are established by the request and the approved product architecture:

- The capability must be reusable across independent Nestgrid consumers, with an independent licence-service consumer and Portal-to-Finance as proving scenarios rather than product-specific dependencies.
- The HTTP boundary is authoritative. A client result represents the observed HTTP outcome and must not reconstruct the originating server-side `ResultStatus`.
- The capability must account for `FullResult` and `ValueOnly` representations, generic and non-generic results, value-bearing and bodyless responses, default mappings and consumer-defined mappings.
- Structured messages must survive the boundary where they are present, including message, code, property and severity.
- Existing `Nestgrid.Response` packages remain lightweight, additive and compatible by default. Core must not acquire `HttpClient`, JSON or presentation dependencies.
- Expected application/HTTP outcomes should be representable as results rather than becoming exceptions solely because the HTTP status is non-2xx.
- Transport, cancellation and protocol failures must remain distinguishable from expected application outcomes.
- The wire representation, rather than the application type alone, is the contract to be consumed and documented.

## Architectural assessment

### Does this belong in the Nestgrid.Response ecosystem?

Yes. There are at least two independent consumers with the same boundary problem, and the capability directly preserves the result and message contract already owned by Nestgrid.Response. It is a natural ecosystem extension provided that it remains an HTTP client adapter rather than becoming part of the core result model.

The requirement does not justify changing the core `Result` model. The current model deliberately controls construction and omits `Status` from JSON; direct deserialisation of `Result<T>` would therefore be an unsafe and incomplete wire contract.

### Recommended package boundary

Create a separate client adapter package, subject to Product approval of the final name. The current candidate, `Nestgrid.Response.Http.Client`, is coherent but should be treated as a working title only.

| Package | Responsibility | Client capability relationship |
| --- | --- | --- |
| `Nestgrid.Response` | Result model, statuses, messages and factories | Remains unchanged and framework-independent |
| `Nestgrid.Response.Http` | Server-side result-to-HTTP mapping and response-shape policy | Remains a sibling policy package; it is not reversed mechanically |
| `Nestgrid.Response.AspNetCore` | ASP.NET Core server execution | Remains unchanged |
| `Nestgrid.Response.Mvc` | Legacy MVC server execution | Remains unchanged |
| Candidate client package | HTTP response interpretation, wire deserialisation and client-result construction | Depends on core; does not depend on ASP.NET Core or MVC |

The client package should not depend on `Nestgrid.Response.AspNetCore` or `Nestgrid.Response.Mvc`. It should also not depend on `Nestgrid.Response.Http` merely to reverse server mappings. A client mapping is a separate policy: an HTTP status code is interpreted as an HTTP outcome, not translated back to the server status that may have produced it.

The reuse question for `SuccessResponseMode` and wire DTOs should be resolved during detailed design. The preferred direction is a client-owned, explicitly named payload contract rather than importing server mapper implementation types into the client package. A new shared wire-contract package is not justified at this stage because it would add another package and lifecycle boundary before evidence demonstrates the need.

### Wire contract and deserialisation boundary

The client should deserialize a private or package-owned wire envelope, then construct a core result through approved factories. It should not deserialize `Result` or `Result<T>` directly.

The envelope should represent the currently supported JSON contract:

- `value`, where applicable;
- `messages`; and
- message fields `message`, `code`, `property` and `severity`.

`status` must not be treated as authoritative wire input. The client status comes from the HTTP outcome. The parser should tolerate normal JSON property casing used by the supported serializer configuration, but it must not silently invent a result when the declared representation is absent or malformed.

The representation must be established explicitly by client configuration or the typed operation contract. The client must not guess between `FullResult` and `ValueOnly` by inspecting arbitrary JSON shape. Shape guessing is ambiguous: a value may itself contain `value` or `messages`, and a failure envelope may be valid JSON without being a valid value model.

### FullResult and ValueOnly

The client contract should declare the expected payload mode for each operation or client policy:

| Declared mode | Successful response with value | Successful response without value | Failure response with envelope |
| --- | --- | --- | --- |
| FullResult | Read the envelope value and messages; construct a client success result | Read an envelope if present; a bodyless status becomes `NoContent` | Read the envelope and preserve messages |
| ValueOnly | Read the body as `T`; messages are available only if the wire representation provides them | Treat a bodyless success as `NoContent`; do not deserialize a missing value | Read the Nestgrid envelope to preserve messages |

For `ValueOnly`, a success body is a value, not a `Result<T>` envelope. For failures, the client should expect the documented Nestgrid failure envelope rather than applying value deserialisation. If a producer emits a different shape, the response is non-conforming and should follow the protocol-failure policy below.

Non-generic operations should return `Result`. Generic operations should return `Result<T>`. A bodyless success should be represented as `NoContent`; a typed `Result<T>` may carry the existing nullable/default value semantics, but this must be explicitly documented and tested rather than inferred from JSON.

### HTTP outcome mapping

The client should have an explicit HTTP-outcome mapping policy, separate from the server `ResultStatus` mapping dictionary. A sensible default contract for known outcomes is:

| HTTP outcome | Client result status |
| --- | --- |
| 200 | `Ok` |
| 201 | `Created` |
| 202 | `Accepted` |
| 204 | `NoContent` |
| 400 | `Invalid` |
| 401 | `Unauthorized` |
| 403 | `Forbidden` |
| 404 | `NotFound` |
| 409 | `Conflict` |
| 422 | `Failed` |
| 5xx, by explicit default policy | `Error` |

This is an initial recommendation, not an approved decision. In particular, 409 must not be interpreted as proof that the server produced `Conflict` rather than `Cancelled`, and 422 must not be treated as a reversible server status. Custom client mappings must be explicit and scoped to the client operation or client policy; they must not mutate the server-side mapping policy.

The detailed design must decide how 3xx and otherwise unmapped statuses behave. The recommended default is to treat an unmapped status as a protocol/policy failure rather than silently assigning a semantically misleading core status. Redirect handling remains the responsibility of `HttpClient` configuration.

### Expected outcomes versus exceptional conditions

The client should return a result for a valid HTTP response whose status and body conform to the declared client contract, including recognised non-2xx application outcomes. It should not call `EnsureSuccessStatusCode()` before interpretation, because that would discard the response body and structured messages.

The recommended default failure split is:

| Condition | Recommended behaviour | Reason |
| --- | --- | --- |
| Recognised HTTP application outcome, including non-2xx | Return `Result`/`Result<T>` | It is an observed application outcome, not an exceptional client failure |
| Network, DNS, TLS or transport failure | Propagate the normal `HttpClient` exception | No HTTP outcome exists to represent |
| Cancellation | Propagate `OperationCanceledException` | Cancellation is control flow and must not be converted into a server/application result |
| Malformed JSON or invalid declared envelope/value | Throw a specific protocol/deserialisation exception | Fabricating an application result would hide a contract failure |
| Non-Nestgrid response where Nestgrid content was declared | Throw a protocol exception | The response cannot safely preserve the promised result contract |
| Unsupported or unmapped HTTP status | Throw a policy/protocol exception by default | Avoid silently assigning the wrong semantic status; explicit mapping can opt in |

The exception types and diagnostic-content policy require detailed design, with ADR-008 applied to any result or diagnostic object that might cross an untrusted boundary. Exceptions must not include response bodies or sensitive headers by default.

### Integration mechanism

The primary abstraction should be a stateless parser/interpreter over `HttpResponseMessage`, with convenient `HttpClient` extensions or a small client facade layered over it. This separates request transport from response interpretation and makes the contract testable with a fake handler.

A `DelegatingHandler` is not the primary mechanism. It does not know the endpoint's `T`, declared payload mode or expected response schema and would make it too easy to consume or reinterpret responses globally. A handler may remain a consumer-owned concern for authentication, retries, logging or tracing.

The package should not own the `HttpClient` lifecycle. Consumers may use a long-lived client with appropriate connection-lifetime configuration or `IHttpClientFactory`/typed clients. Microsoft guidance confirms that the factory manages handler pooling and that factory-created clients and typed clients have lifetime considerations; the Nestgrid package should document these constraints rather than impose a competing lifetime model:

- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- [Use the IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)
- [Troubleshoot IHttpClientFactory issues](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory-troubleshooting)

The base package should be usable without dependency injection. An optional DI registration package may be considered only if proving consumers demonstrate repeated value; it should not add `Microsoft.Extensions.*` dependencies to the core client parser.

### Configuration and lifetimes

Configuration is required because payload mode, target type, custom status mappings, serializer settings and strictness are contract concerns. The preferred scopes are:

- immutable global client defaults;
- named/typed client policy for a service; and
- explicit per-operation overrides only where the endpoint contract genuinely differs.

Mutable process-wide configuration and hidden ambient options should be avoided. Parsers and mapping policies should be immutable and safe to register as singletons. Response objects, buffers and operation-specific state must remain request-scoped.

### Compatibility and release timing

No existing package needs to change to enable the capability. The proposal is additive and should not alter existing server mappings, response modes, Result construction, adapter signatures or package dependencies.

The capability should evolve independently in architectural scope, but release/version alignment with the five-package product remains a Product/Platform decision. It should not be retrofitted into the already-published 0.7.0 packages. A separate package can be introduced before 1.0 if Product prioritises it and the API baseline explicitly includes or excludes it. It need not block stabilisation of the existing five-package core.

The open 1.0 findings in the Independent Review, especially public API baselining, `NoContent` semantics and HTTP mapping rationale, must be resolved before treating a new client package as part of the stable 1.0 public surface.

## Options considered

### Option A — Leave client handling to consumers

No Nestgrid package is added; each typed client implements request, status handling and deserialisation.

Rejected as the recommended direction because two independent consumers already expose the same duplication and risk inconsistent message/error handling. It remains the fallback if the product does not want to own a client wire contract.

### Option B — Add client behaviour to `Nestgrid.Response.Http`

Extend the existing server-side HTTP policy package with `HttpClient` and deserialisation.

Rejected. It would mix opposite boundary directions, introduce transport/serializer concerns into the current shared mapper, encourage incorrect reversal of server mappings and increase compatibility pressure on an established package.

### Option C — Separate `Nestgrid.Response.Http.Client` package

Create a sibling adapter package with an explicit response interpreter and client policy.

Recommended. It preserves existing boundaries, supports the proving scenarios and allows the client contract to evolve additively.

### Option D — Global `DelegatingHandler`

Intercept all responses and convert them to results automatically.

Rejected as the default. A handler lacks the endpoint's declared type and payload mode, and automatic conversion would make malformed/non-Nestgrid responses and streaming scenarios difficult to reason about.

### Option E — Direct deserialisation of `Result<T>`

Use the core result type as the JSON DTO.

Rejected. Core construction is deliberately controlled, `Status` is not the HTTP authority and the serialized envelope is not equivalent to all core state.

## Recommended public-contract direction

The detailed Architecture defines a small set of additive APIs with these properties:

- an explicit operation/policy declaration for payload mode and target type;
- a stateless response interpreter over `HttpResponseMessage`;
- `Result` and `Result<T>` construction through core factories;
- explicit client HTTP-outcome mapping independent of server mapping;
- standard `HttpClient` extensions or a small facade as convenience only;
- no implicit global interception and no direct `Result<T>` JSON deserialisation;
- protocol exceptions that are safe by default and do not include raw response content;
- serializer options that are immutable, explicit and compatible with the existing JSON wire contract.

The detailed API shape, wire contract, status policy, construction strategy and exception boundary are recorded in ADR-009, ADR-010, ADR-011 and the revised Architecture Pack. Engineering may refine internal names and implementation details within that boundary, but must escalate a public-contract or semantic deviation.

## Minimum proving scenarios

Before implementation is accepted, the Engineering and Quality handover should require:

1. FullResult generic success with value and messages.
2. FullResult non-generic success and failure.
3. ValueOnly generic success and envelope failure.
4. 204/no-content success for generic and non-generic operations.
5. Preservation of message, code, property and severity.
6. Default mappings for 200, 201, 202, 204, 400, 401, 403, 404, 409, 422 and 5xx.
7. Explicit custom client mapping, proving it is not a reversal of server status mapping.
8. Malformed JSON, wrong payload mode, empty body where a value is required and non-Nestgrid content.
9. Transport failure and cancellation propagation.
10. A fake-`HttpMessageHandler` integration test proving request/response behaviour without a live network.
11. One sample proving scenario representing an independent licence-service consumer without referencing consumer-specific infrastructure.
12. One independent sample proving scenario representing Portal-to-Finance consumption.

Golden wire fixtures should be cross-checked against the existing ASP.NET Core and MVC samples so that the client does not silently diverge from the server adapters.

## Risks and open questions

- The exact JSON serializer and target-framework support matrix must be selected without weakening the existing minimum-compatible dependency policy.
- The client-side default mapping for 3xx and unknown status codes needs an explicit decision.
- The strictness policy for unknown message severity values and additional envelope fields needs definition.
- Streaming, large responses and non-JSON content are not included in the initial capability unless a proving consumer requires them.
- Authentication, retries, resilience, logging and telemetry must remain consumer or platform concerns, not hidden behaviour of the result parser.
- Version alignment between the new package and the existing five packages needs Product/Platform confirmation.
- The public API baseline and unresolved 1.0 findings in the Independent Review must be reconciled before stable support is claimed.

## Recommendation and approval gate

**Recommend pursuing the capability as a new, separate, additive HTTP client adapter package, with an explicit declared wire representation and a stateless `HttpResponseMessage` interpreter.** Keep `Nestgrid.Response` unchanged, keep `Nestgrid.Response.Http` as the server-side mapping owner, and do not use a handler or reverse mapping as the primary abstraction.

This assessment originally recorded findings and a recommendation only. The Project Sponsor has now approved progression beyond Recommend. The detailed decisions are recorded in ADR-009 through ADR-011; implementation remains governed by the revised Architecture Pack and Engineering handover.

The Sponsor approval authorises:

1. creation of a separate client adapter package in the Nestgrid.Response ecosystem;
2. the explicit FullResult/ValueOnly contract approach;
3. the client-owned HTTP-outcome policy and expected-outcome versus exceptional-condition split; and
4. execution of the detailed Architecture and Engineering handover recorded in the related artefacts.

Engineering implementation must remain within the refined API shape, target frameworks, serializer strategy, status policy and package/version lifecycle recorded in the revised Architecture Pack and handover.

## Closure

Assessment status: **Approved direction — detailed Architecture authorised.**

No source implementation, existing package change or public API decision has been made by this document.
