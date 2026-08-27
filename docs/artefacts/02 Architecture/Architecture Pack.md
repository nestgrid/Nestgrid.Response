# Architecture Pack

```yaml
title: Nestgrid.Response Architecture Pack
eos_version: 1.1.0
version: 1.2
status: Approved for Engineering handover
owner: Solution Architect
contributors: Knight
produced_by: Solution Architect
consumed_by: Software Engineer, Quality Engineer, Security Engineer, Platform Engineer
date: 2026-08-21
supersedes:
related_decisions:
  - ../../decisions/ADR-001-Result-Pattern-Philosophy.md
  - ../../decisions/ADR-002-Status-Driven-Results.md
  - ../../decisions/ADR-003-Immutable-Results.md
  - ../../decisions/ADR-004-AspNetCore-Separation.md
  - ../../decisions/ADR-005-Core-Object-Model.md
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
  - ../../decisions/ADR-012-HTTP-Client-Safety-Boundaries.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Recommendation.md
  - Engineering Handover.md
  - Architecture Feedback - Security.md
  - Architecture Feedback - SEC-006 Dependency Remediation.md
  - Architecture Recommendation - HTTP Client Capability.md
  - Architecture Feedback - HTTP Client Implementation Review.md
  - ../01 Discovery/Product Brief.md
  - ../01 Discovery/Architecture Handover.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Context

Nestgrid.Response is an existing five-package .NET result library at the v0.6.0 implementation baseline. v0.7.0 established the controlled retrofit into the Nestgrid Engineering Operating System. This revision adds the approved architecture for a sixth, additive HTTP client adapter package without changing the existing five-package responsibilities.

The approved product intent is to provide explicit application outcomes without coupling application code to HTTP or using exceptions for routine business flow. Modern ASP.NET Core is the primary segment; legacy ASP.NET Core MVC remains actively supported. OpenAPI and `ProblemDetails` remain deferred. The approved HTTP client capability is a focused adapter for consuming Nestgrid HTTP representations; it is not a generic HTTP-client framework.

## Architecture Goals

- Preserve a small, readable and framework-independent core result model.
- Keep transport policy separate from result semantics and framework execution.
- Support modern ASP.NET Core and legacy MVC without semantic drift.
- Preserve public API and behaviour compatibility by default.
- Make package support, distribution, upgrade and maintenance expectations explicit.
- Enable Engineering to implement conformance fixes without inventing architecture.
- Provide an additive HTTP client adapter that consumes explicit Nestgrid wire representations and constructs client results from authoritative HTTP outcomes.

## Architecture Principles

- Expected application outcomes are represented by results; unexpected failures remain exception and operational concerns.
- The core package must not depend on HTTP or presentation frameworks.
- Dependencies point from adapters and extensions towards the core, never the reverse.
- Client and server HTTP directions are separate policies; server-side status mappings are never reversed to infer client semantics.
- Payload representation is an explicit client contract; JSON shape is never used to infer `FullResult` or `ValueOnly`.
- Wire DTOs are adapter implementation models; core `Result` types remain semantic models and retain their private construction boundaries.
- `ResultStatus` is the authoritative semantic outcome; `IsSuccess` and `IsFailure` are conveniences.
- Results and their message collections are immutable after construction.
- Shared HTTP mapping policy has one owner; adapters execute that policy for their framework.
- Public changes preserve compatibility unless a justified, documented and approved exception exists.
- New capabilities require product evidence and a product decision; technical possibility alone does not create scope.
- Package boundaries should remain easy to test, document, distribute and support.

## Quality Attributes

| Quality Attribute | Architectural Implication | Priority |
| --- | --- | --- |
| Compatibility | Preserve public types, members, statuses, mapping defaults and package support claims. Breaking changes require documented justification and Sponsor approval. | Must |
| Maintainability | Keep five clear package responsibilities and avoid duplicated mapping policy. | Must |
| Portability | Keep core, HTTP policy, validation and MVC packages on their documented portable targets; keep modern ASP.NET Core framework dependencies isolated. | Must |
| Testability | Validate core semantics, validation conversion, shared mappings and both adapters independently and through consumer-facing samples. | Must |
| Security | Do not put authentication, authorisation or secrets in the library; avoid serialising exception implementation details; treat validation messages as potentially sensitive output. | Must |
| Performance | Keep conversions allocation-conscious and predictable; avoid introducing reflection-heavy or distributed mechanisms without evidence. | Should |
| Observability | The library must not assume a logging or telemetry provider; adapters should expose deterministic behaviour that consuming applications can observe and test. | Should |
| Operability | Package publication, installation, version selection, upgrade, rollback and support ownership must be documented for public consumers. | Must |

## Key Decisions

| Decision | Rationale | Related Record |
| --- | --- | --- |
| Retain five-package structure | Matches approved scope and current implementation while preserving dependency isolation. | ADR-004, ADR-006 |
| Keep MVC actively supported | Explicit Sponsor decision and approved Product Brief commitment. | ADR-006 |
| Preserve compatibility by default | Public NuGet consumers require predictable upgrades; breaking changes need justification and approval. | This Pack; future release decisions |
| Centralise HTTP mapping policy | Prevents modern and MVC adapters from acquiring divergent semantics. | ADR-004, ADR-006 |
| Add opt-in detailed validation conversion | Adds member-aware messages without changing existing `.Validation` output. | TDR-001 |
| Make exception conversion safe by default | Prevents raw exception details from crossing the normal result-to-HTTP path while retaining explicit diagnostic conversion. | ADR-008 |
| Add a separate HTTP client adapter | Provides reusable HTTP-to-Result handling without coupling core or changing the server mapper. | ADR-009 |
| Use explicit client payload modes and wire DTOs | Preserves the FullResult/ValueOnly contract and core construction boundary. | ADR-010 |
| Map HTTP outcomes on the client | Makes HTTP authoritative and avoids non-reversible server-status reconstruction. | ADR-011 |
| Defer OpenAPI and `ProblemDetails` | Preserves the separate deferred scope; the client package does not imply metadata generation. | Product Brief; OpenAPI Investigation |

## Architecture Overview

```text
Application / Domain / Worker code
              |
              v
      Nestgrid.Response
              |
       +------+------------------------------+
       |                                     |
       v                                     v
Nestgrid.Response.Http            Extensions.Validation
       |
       +----------------------+----------------------+
       |                      |                      |
       v                      v                      v
Response.AspNetCore     Response.Mvc       Http.Client

HTTP responses from external Nestgrid services
                         |
                         v
                 Http.Client -> Nestgrid.Response
```

### Package Responsibilities

| Package | Responsibility | Dependency Boundary |
| --- | --- | --- |
| `Nestgrid.Response` | `Result`, `Result<T>`, statuses, immutable structured messages, factories and functional helpers. | No ASP.NET Core or HTTP dependency. |
| `Nestgrid.Response.Http` | Status-to-HTTP mapping, response-shape options and framework-neutral mapping metadata. | Depends on core only. |
| `Nestgrid.Response.AspNetCore` | `IResult` and controller `IActionResult` execution for modern ASP.NET Core. | Depends on HTTP policy and `Microsoft.AspNetCore.App`; target `net8.0` unless a later approved support decision changes it. |
| `Nestgrid.Response.Mvc` | `IActionResult` execution for the supported legacy MVC compatibility range. | Depends on HTTP policy and `Microsoft.AspNetCore.Mvc.Core`; current baseline is `netstandard2.0` with 2.1.38 dependency. |
| `Nestgrid.Response.Extensions.Validation` | DataAnnotations conversion to result messages and invalid results, including opt-in member-aware conversion. | Depends on core and `System.ComponentModel.Annotations`; no web dependency. |
| `Nestgrid.Response.Http.Client` | Explicit FullResult/ValueOnly HTTP response interpretation, wire DTO deserialisation, client HTTP-outcome mapping and core-result construction. | Depends on core and the supported JSON serializer; no ASP.NET Core, MVC, authentication or DI dependency. |

## Boundaries and Responsibilities

- Application and domain services own business decisions and return semantic results.
- The core package owns semantic statuses and message structure, not HTTP codes or framework response objects.
- The HTTP package owns default and custom status mappings and success payload mode.
- Framework adapters own response execution and framework-specific registration.
- The validation extension owns translation from DataAnnotations types into core messages; it is not a validation engine.
- Consumers own exception handling, logging, authentication, authorisation, persistence and domain-specific validation policy.
- Consumers own `HttpClient`/`IHttpClientFactory` composition, authentication handlers, retries, resilience, logging and telemetry. The client package interprets a response; it does not own the transport lifecycle.
- Client HTTP mapping is independent of server-side `Nestgrid.Response.Http` mapping. The client never reconstructs the originating server `ResultStatus`.
- Client payload mode is configured explicitly as `FullResult` or `ValueOnly`. The client never infers mode from JSON shape.
- FullResult uses package-owned wire DTOs. Existing core factories reconstruct `Result`/`Result<T>` and messages without weakening core constructors.

## Domain Model

This product has no persistence domain or business aggregate. Its domain is the result vocabulary:

- `ResultStatus` expresses semantic operation outcomes.
- `Result` expresses an outcome without a value.
- `Result<T>` expresses an outcome with an optional value.
- `ResultMessage` expresses human-readable and structured context through message, code, property and severity.
- `Results` and `ResultMessages` provide the preferred construction surface.

## API Strategy

The public API is consumer-facing and compatibility-sensitive.

- Preserve existing public types, factory names, status members and extension-method behaviour by default.
- Additive APIs are preferred for v0.7.0 improvements.
- Breaking changes require a documented rationale, migration guidance, test evidence and Project Sponsor approval.
- `ResultStatus` values and default HTTP mappings are compatibility contracts; changes require explicit review.
- Package READMEs and samples must describe supported target frameworks, dependencies, package selection and adapter usage.

### HTTP Client Public Contract Direction

The approved client contract is intentionally small and additive. The precise source names may be refined by Engineering only where behaviour remains unchanged:

```csharp
public enum NestgridResponsePayloadMode
{
    FullResult,
    ValueOnly
}

public sealed class NestgridResponseClientOptions
{
    public NestgridResponsePayloadMode PayloadMode { get; }
    public JsonSerializerOptions SerializerOptions { get; }
    public IReadOnlyDictionary<int, ResultStatus> StatusMappings { get; }
}

public sealed class NestgridResponseReader
{
    public Task<Result> ReadAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
    public Task<Result<T>> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default);
}
```

The package may provide `HttpClient` and `HttpResponseMessage` extension conveniences over the reader, but it must not introduce a generic Nestgrid HTTP-client abstraction. The reader is stateless after construction, does not own or dispose caller-owned `HttpResponseMessage` instances, and does not own `HttpClient` instances.

`NestgridResponseProtocolException` is the focused public exception for malformed, mismatched or unmapped responses. It may expose status and declared payload mode, but never raw response content or sensitive headers. Transport and cancellation exceptions remain standard `HttpClient` exceptions.

The public options must copy or otherwise freeze caller-provided status mappings at construction. Serializer options are captured at construction and must not be mutated by the package. The default payload mode is `FullResult`, matching the established server default, but the operation contract must still explicitly select the mode through the client policy.

### HTTP Client Wire and Outcome Rules

The client interprets the HTTP status before the body:

| HTTP status | Client `ResultStatus` |
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
| 500–599 | `Error` |

These are client-side semantics, not the inverse of server mappings. Exact consumer mappings may extend or replace the client policy. 3xx and otherwise unmapped statuses are protocol failures by default.

The body rules are:

- `FullResult` generic success: deserialize the dedicated envelope containing `Value` and `Messages`.
- `FullResult` non-generic success/failure: deserialize the dedicated envelope containing `Messages`.
- `ValueOnly` generic success: deserialize the body as `T`.
- `ValueOnly` failure: deserialize the failure envelope to preserve structured messages.
- `204`: return the existing `Results.NoContent()` or `Results.NoContent<T>()` outcome without reading a body.
- Empty 200/201/202 non-generic success: return the mapped non-generic result with no messages.
- Empty 200/201/202 generic success: protocol failure because a declared value is absent.

Malformed JSON, an invalid envelope, a wrong payload mode, non-Nestgrid content or an unmapped status is not converted into an application result. Network/transport failures and cancellation are propagated unchanged.

## Integration Strategy

There are no runtime external integrations. Integration occurs through package references and framework adapter boundaries.

- Core consumers reference only the core package.
- HTTP consumers reference the shared HTTP package and the adapter appropriate to their hosting framework.
- Adapters consume shared mapping metadata and must not independently redefine default semantics.
- Validation consumers opt into DataAnnotations conversion and remain responsible for invoking validation.
- HTTP client consumers reference `Nestgrid.Response.Http.Client` and compose it with their normal `HttpClient` or `IHttpClientFactory` pipeline.
- Authentication, authorisation handlers, retries, resilience, logging and telemetry remain outside the client package.
- The client package does not depend on ASP.NET Core, MVC or the server-side `Nestgrid.Response.Http` mapper.

## Data Strategy

Nestgrid.Response has no persistence, migrations, data ownership or long-lived storage. Result values and messages are in-memory objects passed across application boundaries.

Consumers are responsible for deciding whether result values or messages may be logged, returned externally or retained. The library should not store exceptions or secrets in result messages.

The client package uses internal wire DTOs for the serialized envelope. It maps wire messages through existing public `ResultMessages` factories and constructs results through existing public `Results` factories. `Result.Status` is not read from JSON; HTTP status is the client authority.

## Security Considerations

- The library provides no authentication, authorisation, secret storage or trust boundary.
- Validation and result messages may contain user or domain data; consuming applications must apply output, logging and privacy policies.
- Exception objects must not be introduced into serialisable result payloads.
- Package publication must use the repository’s controlled CI and NuGet credentials; credentials are operational secrets, not product configuration.
- Dependencies should remain minimal and reviewed when changed.
- Known vulnerable dependencies in a supported or published graph are release blockers until remediated or explicitly accepted under the SEC-006 feedback and ADR-007.
- Client-safe output, diagnostic output and consumer-controlled domain output must be treated as distinct categories.
- Default mappings for `Unauthorized`, `Forbidden`, `Error` and `NoContent` are normative; custom mappings remain consumer-owned configuration with security implications.
- Exception conversion follows ADR-008: safe generic output by default; diagnostic details require an explicitly named method and trusted output boundary.
- The client must not include raw response bodies, secrets or sensitive headers in protocol exception messages or properties.
- Client authentication and handler configuration remain consumer-owned; the package must not log or persist credentials.

## Operational Considerations

- Build, test, package and publish through the repository’s CI workflows.
- Keep package README files inside packages so installed consumers receive usage and support guidance.
- Validate package contents, dependency graphs and sample consumption before release consideration.
- Maintain current version, target framework and dependency documentation across project files, package READMEs, root README and decision records.
- Do not add runtime telemetry or service infrastructure to the library.

## Operational Model

### Distribution and Installation

Packages are distributed through NuGet. Consumers install only the package required by their layer:

- core for framework-independent application code;
- HTTP plus an adapter for web responses;
- HTTP.Client for consumers that need HTTP responses converted to Nestgrid results;
- validation extension for DataAnnotations translation.

### Configuration

HTTP mappings and success payload shape are configured through `NestgridResponseOptions`. Configuration is application-owned and must not require secrets.

Client payload mode, serializer options and client-side HTTP mappings are configured through immutable `NestgridResponseClientOptions`; they are distinct from server-side `NestgridResponseOptions`. The client reader is stateless and may be registered as a singleton. It does not own the lifetime of `HttpClient` or caller-owned response objects.

### Upgrade and Compatibility

Use normal NuGet version selection and review release notes before upgrade. Preserve compatibility by default. Any approved breaking change must include migration notes, affected packages, replacement APIs and a versioning rationale.

### Rollback and Recovery

Consumers should be able to pin the previous known-good package version and redeploy through their normal dependency and release process. Releases must retain package artifacts and validation evidence sufficient to identify the previous version. Unpublishing packages is not the normal rollback mechanism.

### Support and Ownership

Nestgrid owns the package architecture and support policy. MVC remains actively supported as part of the full Nestgrid.Response library. It follows the common library maintenance, versioning, support and review lifecycle, with no separate end-of-support policy. The `2.1.38` baseline, dependency advisories, target-framework changes, public contract changes and release evidence are reviewed through the same library governance as the other packages. Consumer issues should be handled through the repository’s documented contribution and issue process.

## Trade-offs

| Trade-off | Decision | Consequence |
| --- | --- | --- |
| Five packages versus one package | Retain package separation. | More packages to publish and document, but smaller dependency footprints and clearer boundaries. |
| MVC support versus modern-only focus | Retain active MVC support. | Higher maintenance and compatibility cost, accepted as product value. |
| Shared mapping versus adapter freedom | Centralise mapping policy. | Adapter-specific exceptions require explicit extension points or decisions. |
| Compatibility versus rapid API evolution | Compatibility first. | Some improvements require additive APIs or approved migration work. |
| Detailed validation output versus minimal conversion | Add it opt-in. | Richer consumer diagnostics without changing existing output contracts. |
| Shared server/client HTTP policy versus separate direction-specific policies | Keep policies separate. | A small amount of duplicated contract knowledge avoids incorrect reverse mapping and keeps package boundaries honest. |
| Automatic response interception versus explicit operation policy | Use explicit reader/options contracts. | Callers must declare payload mode, but response interpretation remains predictable and testable. |
| Core constructor access versus public factory construction | Use existing public factories. | The client preserves the core private boundary and accepts the small status/message bridge. |

## Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| MVC support evidence drifts from the documented baseline | Consumers may receive an inaccurate support promise. | Keep the `2.1.38` baseline and common library maintenance policy current with project files, package documentation and release evidence. |
| ADR-006 or package documentation drifts again | Architecture traceability is weakened. | Keep decision, project, README and Pack changes in the same review and run link/documentation checks. |
| Mapping changes break consumers silently | HTTP clients may observe changed status or payload behaviour. | Treat mappings as compatibility-sensitive, test both adapters and document changes. |
| Validation detail leaks sensitive fields | API responses or logs may expose internal property names or messages. | Document consumer responsibility and make detailed conversion opt-in. |
| Retrofit becomes feature expansion | v0.7.0 delivery loses focus. | Defer unapproved roadmap candidates and require Product decisions for new capabilities. |
| Client wire contract diverges from server samples | Consumers receive protocol failures or lose messages. | Use dedicated golden fixtures and cross-check FullResult/ValueOnly output against both server adapters. |
| Client status mapping is mistaken for server status reversal | 409/422 and custom mappings acquire misleading semantics. | Keep a separate client policy, document rationale and test custom mappings independently. |
| Protocol failures are hidden as application results | Transport or contract defects become difficult to diagnose. | Propagate transport/cancellation and throw safe protocol exceptions for malformed or unmapped responses. |
| Client package acquires generic HTTP infrastructure | Dependency and support scope expand beyond the product. | Keep authentication, retries, resilience, DI registration and handler composition consumer-owned. |

## Open Questions and Follow-up Decisions

- Keep the documented MVC `2.1.38` baseline and common library review triggers current with project files, package documentation and release evidence.
- Confirm the package compatibility matrix through package validation in Engineering and Quality.
- Define the release-specific support review date and adoption signals for MVC.
- Reassess deferred OpenAPI, `ProblemDetails` and adapter candidates only through a new Product decision.
- Confirm the final public package/API names during Engineering design without changing the approved boundary.
- Confirm the package version alignment and release sequencing for the additive client package before publication.

These are governance and evidence follow-ups, not permission to invent new product scope during Engineering.

## Engineering Guidance

Engineering should implement the following in priority order:

1. Reconcile documentation and ADR-006 with the five-package implementation and current dependency versions.
2. Add the detailed validation conversion described in [TDR-001](../../decisions/TDR-001-Validation-Result-Conversion-Detail.md) without changing existing conversion defaults.
3. Confirm or correct package boundaries, target frameworks, dependency versions and adapter mapping consistency.
4. Add tests for compatibility-sensitive result semantics, mappings, validation conversion and both adapters.
5. Update package and consumer documentation, samples and compatibility guidance.
6. Produce an Implementation Report with Engineering Assurance, explicitly recording any deviation from this Pack.

For `Nestgrid.Response.Http.Client`, Engineering must additionally:

1. Create the new package targeting `netstandard2.0`, using the existing centrally managed `System.Text.Json` dependency unless a compatibility finding is escalated.
2. Implement the public options, payload-mode enum, reader/extensions and safe protocol exception within ADR-009 through ADR-011.
3. Keep wire envelope and message DTOs internal; do not deserialize core `Result`, `Result<T>` or `ResultMessage` directly.
4. Construct results through the existing public `Results` and `ResultMessages` factories; do not widen core constructors or add `InternalsVisibleTo`.
5. Implement exact client HTTP mappings from ADR-011, with explicit immutable custom mappings and protocol failure for 3xx/unmapped statuses by default.
6. Preserve structured message fields and validate FullResult envelopes without inferring payload mode.
7. Preserve normal transport and cancellation exceptions, and ensure protocol exception messages do not expose response bodies or sensitive headers.
8. Provide direct `HttpResponseMessage` reader tests and `HttpClient` fake-handler tests; do not introduce a mandatory DI or authentication dependency.
9. Add proving samples/tests for FullResult and ValueOnly, generic and non-generic results, no content, non-2xx envelopes and custom client mappings.

For SEC-006, Engineering must follow [Architecture Feedback — SEC-006 Dependency Remediation](Architecture%20Feedback%20-%20SEC-006%20Dependency%20Remediation.md) and return the dependency-path, compatibility and package-closure evidence to Security and Quality.

Engineering may fix defects and improve implementation where necessary for conformance, correctness, maintainability or compatibility. It must escalate a change that alters approved product scope, public compatibility policy or a reserved decision.

## Recommendation

**Architecture complete; proceed to Engineering with conditions.**

The conditions are that Engineering works within this Pack and ADR-009 through ADR-011, records implementation deviations, preserves compatibility by default, resolves SEC-006 or obtains an explicit authorised exception, and hands forward explicit evidence gaps to Quality, Security and Platform. The client package is additive and is not part of the already-published 0.7.0 package set. This Pack does not constitute release approval.
