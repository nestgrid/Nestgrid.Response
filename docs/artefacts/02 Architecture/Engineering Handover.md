# Engineering Handover

```yaml
title: Nestgrid.Response v0.7.0 Engineering Handover
version: 1.1
status: Approved
owner: Solution Architect
contributors: Knight
produced_by: Solution Architect
consumed_by: Software Engineer
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Recommendation.md
  - Architecture Pack.md
  - Architecture Feedback - SEC-006 Dependency Remediation.md
  - ../01 Discovery/Product Brief.md
```

## Purpose

This handover gives Engineering the implementation boundary, priorities, constraints and acceptance expectations for the v0.7.0 EOS retrofit and ongoing product governance.

## Authorised Scope

- Reconcile architecture and product documentation with the existing five-package implementation.
- Correct stale or contradictory architecture records.
- Fix defects and improve implementation where necessary to satisfy the approved architecture, compatibility expectations or maintainability goals.
- Add the opt-in detailed DataAnnotations validation conversion described in TDR-001.
- Preserve existing public behaviour unless a breaking change is justified, documented and approved.
- Add or strengthen tests, samples and package documentation required by the Architecture Pack.

Out of scope:

- OpenAPI support.
- `ProblemDetails` support.
- Additional adapters.
- A general validation framework.
- Persistence, hosting, authentication or workflow features.

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

## Engineering Decisions and Escalation

Engineering may choose internal helper names, allocation details and test organisation. Escalate before changing:

- package boundaries;
- target-framework support promises;
- public status semantics or default mappings;
- existing conversion behaviour;
- actively supported MVC intent;
- deferred product capabilities.

## Handover Completion

Engineering should return an Implementation Report with Engineering Assurance covering:

- implementation changes and deviations from this Pack;
- compatibility assessment;
- tests and evidence;
- package and sample validation;
- SEC-006 dependency-path analysis, remediation or authorised exception evidence;
- unresolved risks and follow-up actions;
- explicit readiness recommendation for Quality and Security.
