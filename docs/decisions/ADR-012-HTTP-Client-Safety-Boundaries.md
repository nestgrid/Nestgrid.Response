# ADR-012: HTTP Client Safety Boundaries

> Decision record for **Architecture**.

## Status

Accepted

## Type

Architecture

## Date

2026-08-26

## Owners

- Solution Architect
- Project Sponsor
- Software Engineer
- Security Engineer

## Context

Security review identified two risks in `Nestgrid.Response.Http.Client`: unbounded buffering of remote response content and disclosure of local serializer or converter diagnostics through public protocol exceptions.

The client is intended to be a delivery and translation channel for API outcomes. Remote HTTP failures and their structured Nestgrid messages must be delivered to consumers as `Result` or `Result<T>` values. Local inability to safely interpret a response is a separate concern and may produce a protocol exception.

## Decision

### Remote outcomes remain results

The client preserves the observed HTTP outcome and API-provided structured messages. Expected API failures are not converted into exceptions merely because their HTTP status is non-2xx. The client must not replace, redact or rewrite API messages as part of normal response interpretation.

Protocol exceptions are reserved for local boundary failures, including malformed content, an explicitly mismatched representation, unsupported media type, unmapped status, resource-limit violation or serializer/converter failure that prevents reliable result construction.

### Bounded response policy

The client options shall include a positive byte limit for response content:

```csharp
public long MaxResponseBodyBytes { get; }
```

The default is `1_048_576` bytes (1 MiB). The limit applies to success and failure responses and is enforced while reading the stream, before unbounded allocation or deserialisation. A consumer may explicitly configure a larger positive limit for known legitimate payloads.

An over-limit response raises `NestgridResponseProtocolException` with a fixed safe message and the observed status code/payload mode. The limit is a local resource-protection policy; it does not assert that larger API payloads are semantically invalid.

### Safe protocol failures

Protocol exceptions produced by the package must not retain inner exceptions or response-derived diagnostic text. Serializer and custom-converter failures are normalised to fixed safe protocol messages. A custom converter must not be able to escape a caller-supplied protocol exception or arbitrary diagnostic chain through the reader.

The public exception type remains catchable by consumers, but its construction surface shall not permit the package’s safe-boundary contract to be undermined. Any public constructor change is an additive-package pre-1.0 compatibility correction and must be reflected in the package README and tests.

Status code and declared payload mode may remain available as safe protocol context. Response bodies, headers, serializer details, converter messages and inner exceptions must not be exposed by package-generated protocol failures.

## Consequences

- API failures remain observable as results with their useful messages.
- Consumers retain control over business interpretation and endpoint policy.
- The package has a bounded-memory default against hostile or compromised endpoints.
- Consumers with larger payloads must make that local resource policy explicit.
- Local processing failures have a stable safe exception contract.
- The new package’s pre-1.0 public exception construction surface may change before 1.0 API stabilisation.

## Implementation constraints

Engineering must:

1. enforce the byte limit during streaming without integer-overflow or allocation bypasses;
2. cover below-limit, exact-limit and over-limit cases for both success and failure responses;
3. normalise serializer and hostile custom-converter failures without inner exceptions;
4. ensure fixed protocol messages contain no response body or diagnostic content;
5. preserve API-provided `ResultMessage` text, code, property and severity in normal failure results; and
6. update package documentation, implementation evidence and Quality/Security tests.

Architecture must be re-engaged if Engineering proposes unlimited defaults, changes the Result-versus-exception boundary, exposes a new wire DTO, adds another serializer or materially changes the public exception contract beyond this decision.

## Alternatives considered

### Return all API failures as exceptions

Rejected. This would discard the delivery-channel purpose and structured API messages.

### Leave response buffering unbounded

Rejected because remote content can exhaust consumer memory and cancellation alone does not impose a resource bound.

### Convert local failures into `Result.Error`

Rejected because it would make malformed or untrusted protocol content appear to be a valid API outcome.

### Preserve serializer/converter inner exceptions publicly

Rejected because consumers may log or expose the exception chain, including implementation details or response-derived secrets.

## Related decisions

- [ADR-008 Safe Exception Result Conversion](ADR-008-Safe-Exception-Result-Conversion.md)
- [ADR-009 HTTP Client Adapter Boundary](ADR-009-HTTP-Client-Adapter-Boundary.md)
- [ADR-010 Client Wire Contract and Result Construction](ADR-010-HTTP-Client-Wire-Contract.md)
- [ADR-011 Client HTTP Outcome Semantics](ADR-011-HTTP-Client-Outcome-Semantics.md)
