# TDR-001: Detailed Validation Result Conversion

> Decision record for **Technical** implementation detail.

## Status

Accepted

## Type

Technical

## Date

2026-08-14

## Owners

- Solution Architect
- Software Engineer

## Context

The existing `.Validation` package converts each DataAnnotations `ValidationResult` into one message and intentionally does not infer message codes or properties. A consumer has demonstrated a useful alternative that expands member names into structured `ResultMessage.Property` values and supplies a stable validation code.

Changing the existing methods would risk breaking consumers that rely on their current output. The capability must therefore be additive and opt-in.

## Decision

Add detailed conversion methods to `Nestgrid.Response.Extensions.Validation`:

```csharp
IEnumerable<ValidationResult>.ToMessagesWithProperties()
IEnumerable<ValidationResult>.ToInvalidResultWithProperties()
IEnumerable<ValidationResult>.ToInvalidResultWithProperties<T>()
```

The detailed conversion shall:

- preserve source order;
- emit one message for each non-null, non-empty and non-whitespace member name;
- emit one memberless message when no usable member exists;
- set `Property` to the member name or `null` for memberless results;
- use `validation_failed` as the default code;
- permit an explicit custom code;
- use `The entity is invalid.` when `ErrorMessage` is null;
- preserve the existing default `Warning` severity and severity override behaviour;
- return an invalid result and preserve the existing empty-input behaviour.

Existing `ToMessage`, `ToMessages`, `ToInvalidResult` and generic `ToInvalidResult<T>` methods remain unchanged.

## Rationale

This provides structured field-level validation information without adding a validation abstraction to the core package or changing existing public behaviour. The member-aware conversion is explicitly opt-in because property names and validation text may be inappropriate for every external response or log.

## Alternatives Considered

### Change existing conversion methods

Rejected because it changes observable message code, property and message-count behaviour for existing consumers.

### Add a validation-specific object model

Rejected because `ResultMessage` already supports code, property and severity without requiring a new core abstraction.

### Always infer codes and properties

Rejected because consumers may not want field-level output or a generated code in every response.

### Add only a caller-side helper

Rejected because the behaviour is sufficiently reusable and belongs naturally in the optional validation package.

## Consequences

Benefits:

- richer structured validation output;
- no core-package change;
- additive public API;
- consistent generic and non-generic invalid-result creation.

Costs and risks:

- additional public API and documentation;
- potential disclosure of property names or validation details;
- one validation result may produce multiple messages;
- custom code policy must remain documented.

## Related Decisions

- [ADR-005 Core Object Model](ADR-005-Core-Object-Model.md)
- [ADR-006 ASP.NET Core and MVC Package Separation](ADR-006-AspNetCore-And-Mvc-Package-Separation.md)

## Related Documentation

- [Architecture Pack](../artefacts/02%20Architecture/Architecture%20Pack.md)
- [Engineering Handover](../artefacts/02%20Architecture/Engineering%20Handover.md)
