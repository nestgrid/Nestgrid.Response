# ADR-011: Client HTTP Outcome Semantics

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

The client observes an HTTP response after the server-side mapping decision has already occurred. Server mappings are configurable and are not reversible. Treating the client as an inverse mapper would therefore lose the authoritative HTTP meaning and could reconstruct the wrong server status, particularly for HTTP 409 and other shared mappings.

Expected non-2xx application outcomes must be available as results. Transport and protocol failures must not be disguised as application outcomes merely to avoid exceptions.

## Decision

The client uses an explicit, client-owned HTTP outcome policy. The initial default exact mappings are:

| HTTP status | Client `ResultStatus` | Rationale |
| --- | --- | --- |
| 200 | `Ok` | Successful request with a normal representation |
| 201 | `Created` | Successful resource creation |
| 202 | `Accepted` | Accepted for asynchronous processing |
| 204 | `NoContent` | HTTP explicitly carries no response body |
| 400 | `Invalid` | The request was not acceptable as supplied |
| 401 | `Unauthorized` | Authentication is absent or unsuccessful |
| 403 | `Forbidden` | The caller is authenticated or identified but not permitted |
| 404 | `NotFound` | The requested resource or operation was not found |
| 409 | `Conflict` | The observed HTTP outcome is a conflict; it is not evidence of the originating server status |
| 422 | `Failed` | The observed HTTP outcome is an expected, semantically unprocessable failure |
| 500–599 | `Error` | The HTTP boundary reports an unexpected server-side failure category |

The exact-status map is consumer-configurable through immutable client policy. Custom mappings replace or extend client interpretation only; they never modify or reverse `Nestgrid.Response.Http` server mappings.

HTTP 3xx statuses and all otherwise unmapped statuses are protocol/policy failures by default. `HttpClient` redirect behaviour remains consumer-controlled. The package must not silently map an unknown status to a convenient core status.

## Expected and Exceptional Conditions

| Condition | Behaviour |
| --- | --- |
| Recognised HTTP status and conforming declared body | Return `Result` or `Result<T>` |
| Recognised non-2xx application outcome with a valid failure envelope | Return failure `Result` or `Result<T>` with preserved messages |
| 200/201/202 non-generic response with no body | Return mapped non-generic result with no messages |
| 200/201/202 generic response with no body | Throw protocol exception |
| 204 response | Return existing `NoContent` result factory output |
| Network, DNS, TLS, timeout or transport failure | Propagate the normal `HttpClient` exception |
| Cancellation | Propagate `OperationCanceledException` |
| Malformed JSON, wrong mode or invalid envelope/value | Throw protocol exception |
| Non-Nestgrid content where a Nestgrid representation was declared | Throw protocol exception |
| 3xx or unmapped status | Throw protocol exception by default |

The client must not call `EnsureSuccessStatusCode()` before reading a response because that would turn expected non-2xx outcomes into exceptions and discard the opportunity to preserve structured messages.

## Protocol Exception

The package should expose one safe, focused protocol exception for malformed, mismatched or unmapped response conditions, provisionally named `NestgridResponseProtocolException`. It may expose the observed status code and declared payload mode, but must not include raw response bodies, secrets or sensitive headers in its message or serializable properties. An underlying serializer exception may be retained as an inner exception according to normal .NET practice.

Transport exceptions and cancellation remain the standard `HttpClient` exceptions. They must not be wrapped as a Nestgrid application result or protocol exception.

## Rationale

This split preserves the Result pattern for expected application outcomes while respecting the distinction between an application failure and the absence of a trustworthy HTTP contract. It also follows the established Nestgrid principle that exceptions represent unexpected operational or boundary failures, not normal business outcomes.

## Alternatives Considered

### Reverse the server mapping

Rejected because mappings are configurable and not bijective. HTTP 409, for example, cannot prove whether the server used `Conflict` or `Cancelled`.

### Convert every non-2xx response to `Error`

Rejected because it loses useful client semantics and structured application outcomes.

### Convert transport and protocol failures to `Result.Error`

Rejected because no valid HTTP application outcome exists and the conversion would hide outages or contract regressions.

### Use `EnsureSuccessStatusCode()` first

Rejected because it prevents expected failure envelopes from being interpreted and preserved.

## Related Decisions

- [ADR-001 Result Pattern Philosophy](ADR-001-Result-Pattern-Philosophy.md)
- [ADR-002 Status-Driven Results](ADR-002-Status-Driven-Results.md)
- [ADR-007 Minimum-Compatible Dependency Policy](ADR-007-Minimum-Compatible-Dependency-Policy.md)
- [ADR-009 HTTP Client Adapter Boundary](ADR-009-HTTP-Client-Adapter-Boundary.md)
- [ADR-010 Client Wire Contract and Result Construction](ADR-010-HTTP-Client-Wire-Contract.md)
