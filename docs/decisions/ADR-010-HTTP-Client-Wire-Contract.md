# ADR-010: Client Wire Contract and Result Construction

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

The server adapters serialize a response representation, not the complete internal `Result` object. `Result.Status` is deliberately ignored by JSON serialization, and `Result<T>` and `ResultMessage` use controlled construction. Directly deserializing the core types would therefore either fail at their private construction boundary or create an incomplete and misleading object.

The client must support both `FullResult` and `ValueOnly` without guessing from arbitrary JSON shape.

## Decision

The client declares the expected payload mode explicitly through a client policy:

```csharp
public enum NestgridResponsePayloadMode
{
    FullResult,
    ValueOnly
}
```

The package uses dedicated internal wire DTOs. The conceptual shapes are:

```csharp
internal sealed class NestgridResponseEnvelope<T>
{
    public T? Value { get; set; }
    public IReadOnlyList<NestgridResponseWireMessage>? Messages { get; set; }
}

internal sealed class NestgridResponseWireMessage
{
    public string? Message { get; set; }
    public string? Code { get; set; }
    public string? Property { get; set; }
    public ResultMessageSeverity Severity { get; set; }
}
```

The non-generic envelope omits `Value`. The actual DTO visibility, serializer attributes and collection implementation remain Engineering details; these types must not become public package contracts.

The client must:

1. determine the HTTP outcome before selecting the body interpretation;
2. use the declared payload mode, never JSON shape inference;
3. deserialize `FullResult` responses through the dedicated envelope;
4. deserialize `ValueOnly` successful generic responses as `T`;
5. deserialize failure envelopes as wire DTOs in either mode;
6. map wire messages through `ResultMessages.Info`, `Warning` or `Error`; and
7. construct the final `Result` or `Result<T>` through the existing public `Results` factories.

The client must not deserialize `Result`, `Result<T>` or `ResultMessage` directly, and must not weaken their constructors or add a serializer-specific construction path.

## Representation Rules

| Client contract | Generic success | Non-generic success | Failure |
| --- | --- | --- | --- |
| `FullResult` | Envelope containing `Value` and `Messages` | Envelope containing `Messages` | Envelope containing `Messages`; a generic envelope may contain a null/default `Value` |
| `ValueOnly` | Body is `T` | No value is expected; an envelope may be used when messages are supplied | Envelope containing `Messages` |

`204 No Content` is bodyless and is converted through the existing `Results.NoContent()` or `Results.NoContent<T>()` factory. The client does not redefine or reinterpret core `NoContent` callback or nullable semantics.

For 200, 201 or 202, a non-generic operation may accept an empty body and construct the mapped non-generic result with no messages. A generic operation requires a value body for those statuses; an empty body is a protocol failure. This keeps missing generic values distinguishable from the approved bodyless `NoContent` result.

FullResult envelopes must contain a valid `messages` array according to the current wire contract. Unknown JSON properties are ignored for forward compatibility. Missing, null or incorrectly typed required envelope content is a protocol failure. A `ValueOnly` success does not manufacture messages that were not present on the wire.

## Result Construction

The client maps only known client-side `ResultStatus` values. A private factory bridge selects the matching existing `Results` overload and passes the reconstructed public `ResultMessage` values. This preserves the core private construction boundary and keeps all message invariants in the existing public factory surface.

No `InternalsVisibleTo`, constructor widening, status setter, serializer constructor or core package change is permitted for this capability.

## Serialization Policy

The first implementation targets the existing `System.Text.Json` wire contract and minimum-compatible dependency policy. Serializer options are explicit client policy, captured at policy/reader construction and not mutated by the package. The package does not add Newtonsoft.Json support or a second serializer abstraction without a new Architecture decision.

The client must use the same property names and enum representation accepted by the current server samples. Golden fixtures must verify `value`, `messages`, `message`, `code`, `property` and `severity`.

## Consequences

- Wire compatibility is tested independently from core object construction.
- A producer using the wrong declared mode fails clearly instead of being heuristically accepted.
- The client can preserve structured messages without exposing a new public DTO model.
- A future wire-version or serializer change can be addressed in the client package without changing core result semantics.

## Alternatives Considered

### Deserialize `Result<T>` directly

Rejected because the core model is not a wire DTO and intentionally controls construction and serialization.

### Add public constructors to core result types

Rejected because it weakens an intentional invariant for the convenience of one adapter.

### Make wire DTOs public

Rejected for the initial capability because it would expose serializer details and create a second public result model to support.

### Infer `FullResult` or `ValueOnly` from JSON shape

Rejected because shapes are ambiguous and inference would make endpoint contracts implicit.

## Related Decisions

- [ADR-001 Result Pattern Philosophy](ADR-001-Result-Pattern-Philosophy.md)
- [ADR-005 Core Object Model](ADR-005-Core-Object-Model.md)
- [ADR-009 HTTP Client Adapter Boundary](ADR-009-HTTP-Client-Adapter-Boundary.md)
- [ADR-011 Client HTTP Outcome Semantics](ADR-011-HTTP-Client-Outcome-Semantics.md)
