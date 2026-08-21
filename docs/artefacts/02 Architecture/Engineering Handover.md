# Engineering Handover

```yaml
title: Nestgrid.Response Engineering Handover
version: 1.2
status: Approved for implementation planning
owner: Solution Architect
contributors: Knight
produced_by: Solution Architect
consumed_by: Software Engineer
date: 2026-08-21
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Recommendation.md
  - Architecture Pack.md
  - Architecture Feedback - SEC-006 Dependency Remediation.md
  - Architecture Recommendation - HTTP Client Capability.md
  - ../01 Discovery/Product Brief.md
```

## Purpose

This handover gives Engineering the implementation boundary, priorities, constraints and acceptance expectations for the v0.7.0 EOS retrofit and the approved additive HTTP client capability. It is a handover for implementation planning and does not authorise unrelated product expansion.

## Authorised Scope

- Reconcile architecture and product documentation with the existing five-package implementation.
- Correct stale or contradictory architecture records.
- Fix defects and improve implementation where necessary to satisfy the approved architecture, compatibility expectations or maintainability goals.
- Add the opt-in detailed DataAnnotations validation conversion described in TDR-001.
- Preserve existing public behaviour unless a breaking change is justified, documented and approved.
- Add or strengthen tests, samples and package documentation required by the Architecture Pack.
- Implement the additive `Nestgrid.Response.Http.Client` package within ADR-009 through ADR-011.

Out of scope:

- OpenAPI support.
- `ProblemDetails` support.
- Unapproved additional adapters beyond the approved HTTP client capability.
- A general validation framework.
- Persistence, hosting, authentication or workflow features.
- A generic Nestgrid HTTP-client abstraction, authentication integration, retry/resilience policy or mandatory DI registration package.

## Implementation Priorities

### P0 — Architecture conformance

1. Confirm the five-package dependency graph.
2. Reconcile ADR-006, root README, package READMEs and project files.
3. Confirm the supported target-framework and dependency matrix, especially MVC.
4. Verify shared HTTP mapping behaviour is identical across ASP.NET Core and MVC adapters.

### P1 — Validation enhancement

Implement the additive APIs in TDR-001:

```csharp
IEnumerable<ValidationResult>.ToMessagesWithProperties()
IEnumerable<ValidationResult>.ToInvalidResultWithProperties()
IEnumerable<ValidationResult>.ToInvalidResultWithProperties<T>()
```

The generic overload is required because C# does not infer `T` from the assignment or return type.

### P1 — Evidence and documentation

- Add unit tests for all validation edge cases.
- Add or update the validation sample.
- Document defaults, generated code, property behaviour and compatibility.
- Validate package contents and consumer installation paths.
- Retain build, test, coverage, mutation and package-validation evidence for downstream gates.

### P0 — HTTP client architecture conformance

1. Add `Nestgrid.Response.Http.Client` as a separate `netstandard2.0` package depending on core and the existing centrally managed `System.Text.Json` version.
2. Keep the package independent of ASP.NET Core, MVC, `IHttpClientFactory`, authentication and consumer-specific proxy infrastructure.
3. Implement explicit `NestgridResponsePayloadMode` values `FullResult` and `ValueOnly`; never infer mode from JSON shape.
4. Implement immutable `NestgridResponseClientOptions` containing payload mode, serializer options and client-owned HTTP status mappings.
5. Implement a stateless `NestgridResponseReader` over caller-owned `HttpResponseMessage` instances, plus only additive convenience extensions that do not form a generic HTTP-client abstraction.
6. Do not dispose caller-owned response instances in the reader. If a convenience send method creates the response, it must define and test ownership clearly.
7. Implement `NestgridResponseProtocolException` for malformed, mismatched and unmapped responses without exposing raw response bodies or sensitive headers.
8. Preserve standard `HttpClient` transport and cancellation exceptions; do not convert them into `Result.Error`.

### P0 — Wire and result construction

1. Use internal dedicated wire DTOs for generic and non-generic FullResult envelopes and wire messages.
2. For FullResult, read `Value` where applicable and require a valid `Messages` array according to the established contract.
3. For ValueOnly generic success, deserialize the body as `T`; for ValueOnly failures, deserialize the failure envelope to preserve messages.
4. Treat 204 as bodyless and call existing `Results.NoContent()` or `Results.NoContent<T>()` without redefining core NoContent semantics.
5. Accept empty 200/201/202 only for non-generic operations; treat a missing generic value as a protocol failure.
6. Map wire messages through `ResultMessages.Info`, `Warning` and `Error`, preserving message, code, property and severity.
7. Construct all results through the existing public `Results` factory family. Do not add `InternalsVisibleTo`, public constructors or serializer-specific core APIs.

### P0 — Client HTTP outcome policy

Implement the exact default mappings in ADR-011:

| HTTP status | Client result status |
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

The implementation must treat this as client semantics, never as reversal of `Nestgrid.Response.Http` mappings. Exact custom mappings must be immutable and explicit. 3xx and otherwise unmapped statuses are protocol failures by default.

## Compatibility Constraints

- Existing `.Validation` methods retain their current behaviour.
- Existing public types, factory methods and status values remain stable.
- Existing default HTTP mappings remain stable unless a separately approved decision changes them.
- New APIs should be additive.
- Any breaking change requires a written rationale, migration guidance, evidence and Project Sponsor approval.

## Security Feedback Constraints

- Implement ADR-008: `Results.Error(Exception)` is safe by default; use `ErrorWithDiagnosticDetails` only for deliberate trusted diagnostic workflows.
- Treat exception-derived messages and type names as diagnostic content, never automatically client-safe content.
- Document that validation messages, property names, result values and custom codes are consumer-controlled output.
- Preserve normative default mappings for `Unauthorized`, `Forbidden`, `Error` and `NoContent`; custom mapping remains an explicit consumer responsibility.
- Retain dependency advisory, restore and package-provenance evidence according to ADR-007.
- Complete the SEC-006 dependency-path and compatibility analysis described in [Architecture Feedback — SEC-006 Dependency Remediation](Architecture%20Feedback%20-%20SEC-006%20Dependency%20Remediation.md).

### SEC-006 Dependency Remediation Acceptance Criteria

- Treat SEC-006 as a P1 release blocker.
- Produce the required dependency-path matrix for every reported advisory.
- Distinguish published package closure, supported consumer graphs and repository-only test/sample dependencies.
- Prefer the lowest compatible patched versions under ADR-007.
- Escalate any change to the actively supported MVC boundary before implementation.
- Do not rely on a blanket exception; any exception must be explicit, scoped, time-limited and reviewed by Security.
- Retain restore, advisory, package-content and consumer-installation evidence.

## Security Mitigation Acceptance Criteria

- Non-generic and generic `Error(Exception)` overloads return `An unexpected error occurred.` without an exception-derived code.
- Non-generic and generic `ErrorWithDiagnosticDetails(Exception)` overloads preserve the previous message/type behaviour.
- Diagnostic method documentation warns against direct untrusted publication.
- Core tests cover both safe-default and explicit-diagnostic paths, including null exceptions.
- Package README, samples and release notes describe the behavioural correction and migration path.

## Validation Enhancement Acceptance Criteria

- A validation result with multiple non-blank member names produces one message per member in source order.
- Blank or whitespace-only member names are ignored.
- A validation result with no usable member name produces one message with `Property == null`.
- The default message code is `validation_failed`.
- A caller may provide a custom code.
- A null `ErrorMessage` uses `The entity is invalid.`.
- Default severity remains `Warning`; caller-selected severity is preserved.
- Empty input produces an invalid result with no messages, matching existing behaviour.
- Existing conversion methods remain unchanged and continue to pass their current tests.

## HTTP Client Acceptance Criteria

- The new package builds for its approved target framework with zero warnings and has no ASP.NET Core, MVC, authentication or DI package dependency.
- FullResult generic success preserves value and all supported message fields.
- FullResult non-generic success and failure preserve all supported message fields.
- ValueOnly generic success deserializes only the declared value representation.
- ValueOnly failure deserializes the Nestgrid envelope and preserves structured messages.
- Generic and non-generic results are both supported without direct core-type deserialisation.
- 204 produces the existing typed and non-typed NoContent result behaviour.
- Empty 200/201/202 non-generic responses produce the mapped non-generic result; empty generic responses fail as protocol errors.
- Default mappings for 200, 201, 202, 204, 400, 401, 403, 404, 409, 422 and 5xx are covered by contract tests.
- Custom client mappings are covered and proven independent from server-side mapping configuration.
- 3xx and unmapped statuses fail with the protocol exception by default.
- Malformed JSON, missing/invalid FullResult messages, wrong payload mode, invalid ValueOnly values and non-Nestgrid content fail safely without returning a fabricated application result.
- Transport failures and cancellation preserve standard `HttpClient` exception behaviour.
- Protocol exception messages and public properties contain no raw response body, credentials or sensitive headers.
- Caller-owned `HttpResponseMessage` lifetime is documented and tested.
- Fake-handler tests prove standard `HttpClient` composition without authentication or retry assumptions.

## Required Proving Evidence

Engineering and Quality must provide:

1. Unit tests for wire DTO validation and message conversion.
2. HTTP status mapping matrix tests, including custom mappings and 409/422 non-reversibility.
3. `HttpResponseMessage` reader tests for every payload mode and result shape.
4. Fake `HttpMessageHandler` integration tests for `HttpClient` convenience methods.
5. Golden JSON fixtures cross-checked against the existing ASP.NET Core and MVC server samples.
6. One reusable licence-service consumer proving scenario and one Portal-to-Finance proving scenario, without consumer-specific proxy infrastructure in the package.
7. Package-content, dependency-graph and consumer-installation evidence for the sixth package.
8. Documentation showing how consumers compose authentication and other handlers through normal `HttpClient`/`IHttpClientFactory` configuration.

## Engineering Decisions and Escalation

Engineering may choose internal helper names, allocation details and test organisation. Escalate before changing:

- package boundaries;
- target-framework support promises;
- public status semantics or default mappings;
- existing conversion behaviour;
- actively supported MVC intent;
- deferred product capabilities.
- the client package boundary, public payload modes, client status mappings, protocol exception behaviour or core construction strategy.

## Handover Completion

Engineering should return an Implementation Report with Engineering Assurance covering:

- implementation changes and deviations from this Pack;
- compatibility assessment;
- tests and evidence;
- package and sample validation;
- SEC-006 dependency-path analysis, remediation or authorised exception evidence;
- unresolved risks and follow-up actions;
- explicit readiness recommendation for Quality and Security.

For the client package, the Implementation Report must explicitly confirm conformance with ADR-009, ADR-010 and ADR-011, record any public API deviation and identify the final package/version publication decision.
