# ADR-008: Safe Exception Result Conversion

> Decision record for **Architecture**.

## Status

Accepted

## Type

Architecture

## Date

2026-08-17

## Owners

- Solution Architect
- Software Engineer
- Security Engineer

## Context

Security identified that `Results.Error(Exception)` copied an exception message and concrete type name into a serialisable result. A consumer could return that result through an HTTP adapter and disclose internal diagnostics.

The existing conversion is useful for trusted internal diagnostic workflows, but it is unsafe as the default public result contract. The product is pre-1.0, so a justified behavioural correction is permitted when documented and approved.

## Decision

`Results.Error(Exception)` and `Results.Error<T>(Exception)` now produce a safe generic error result:

- status remains `Error`;
- message is `An unexpected error occurred.`;
- no exception message or type name is copied into the result;
- the exception is not retained;
- callers remain responsible for logging the exception separately.

The previous diagnostic conversion is available through explicitly named methods:

```csharp
Results.ErrorWithDiagnosticDetails(exception);
Results.ErrorWithDiagnosticDetails<T>(exception);
```

These methods preserve the exception message and type name for trusted internal or diagnostic workflows. Their results must not be returned directly to untrusted clients or serialised without an output policy.

## Rationale

The safe behaviour is the least surprising default for a result type that is routinely passed to HTTP adapters. The explicit diagnostic name makes the disclosure trade-off visible at the call site while preserving the useful internal capability.

The change preserves the existing method signatures for source and binary compatibility, but intentionally changes observable message content. This is an approved pre-1.0 compatibility exception addressing a P1 security finding.

## Alternatives Considered

### Preserve raw details in `Error(Exception)`

Rejected as the default because it keeps the normal path capable of disclosing internal diagnostics.

### Remove exception conversion entirely

Rejected because trusted internal workflows still benefit from a standard conversion and the capability can be made explicit.

### Require callers to construct a safe message manually

Rejected as the sole approach because it loses the convenience of exception-to-result conversion and makes internal diagnostic use less consistent.

## Consequences

- Existing callers using `Error(Exception)` receive safe generic output and must not rely on the previous raw message/type content.
- Consumers that require diagnostic details must adopt the explicitly named methods and apply an output policy.
- Documentation and samples must distinguish logging from client response content.
- Security must re-review SEC-001 and the related output guidance.
- Release notes must call out the behavioural correction and migration path.

## Related Decisions

- [ADR-001 Result Pattern Philosophy](ADR-001-Result-Pattern-Philosophy.md)
- [ADR-007 Minimum-Compatible Dependency Policy](ADR-007-Minimum-Compatible-Dependency-Policy.md)

## Related Documentation

- [Architecture Feedback - Security](../artefacts/02%20Architecture/Architecture%20Feedback%20-%20Security.md)
- [Security Assessment](../artefacts/05%20Security/Security%20Assessment.md)
- [Core package README](../../src/Nestgrid.Response/README.md)
