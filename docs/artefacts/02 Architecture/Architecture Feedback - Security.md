# Architecture Feedback

```yaml
title: Nestgrid.Response v0.7.0 Architecture Feedback - Security
version: 1.2
status: Approved with Engineering action
owner: Solution Architect
contributors:
  - Morgan profile
produced_by: Solution Architect
consumed_by: Project Sponsor, Software Engineer, Security Engineer, Quality Engineer, Platform Engineer
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
  - SEC-001
  - SEC-003
  - SEC-004
  - SEC-005
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Architecture Pack.md
  - Engineering Handover.md
  - ../05 Security/Security Assessment.md
  - ../05 Security/Security Feedback - Exception and Output Disclosure.md
  - ../05 Security/Security Feedback - Publication and Dependency Controls.md
```

## Context

Security completed its v0.7.0 assessment with conditions and returned three findings to Architecture/Engineering. The product is a stateless library with no runtime authentication, authorisation, persistence or secrets. The relevant security boundary is therefore the boundary between result content and consumer-controlled HTTP or logging output, plus the package publication and dependency supply chain.

## Feedback Summary

The Architecture Pack already identifies result values, validation messages and exception-derived content as consumer-controlled output. Security has correctly identified that the current exception convenience API and first-party example make unsafe disclosure easy, and that the current guidance does not sufficiently distinguish client-safe output from diagnostics.

Architecture can disposition SEC-003, SEC-004 and SEC-005 without changing product scope. The Sponsor has approved the safe-default correction for SEC-001 as a justified pre-1.0 behavioural change.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-001 | P1 | `Results.Error(Exception)` copied the exception message and type name into a serialisable result. | A consumer could return internal diagnostics through an HTTP adapter. | Implement ADR-008 and obtain Security re-review. |
| SEC-003 | P2 | Minimum-compatible dependency policy lacks retained advisory and restore evidence. | Dependency posture is not auditable at release time. | Record the policy and review triggers in ADR-007; Security/Platform retain per-release evidence. |
| SEC-004 | P2 | Client-safe and diagnostic output guidance is not sufficiently distinct. | Consumers may expose validation properties, messages, result values or exception details to untrusted callers. | Update package and sample guidance; make the diagnostic/client boundary explicit and add documentation-focused tests or review evidence. |
| SEC-005 | P2 | Consumers can override status mappings, including security-sensitive outcomes. | An application can unintentionally change authentication, authorisation, error or caching semantics. | Keep custom mappings as an intentional consumer capability, document the security responsibility, preserve normative defaults and test the default mappings across both adapters. |

## Dispositions

### SEC-003 — Architecture disposition

The minimum-compatible dependency policy is accepted as an architectural compatibility constraint:

- retain the lowest compatible direct dependency versions unless advisory evidence identifies a vulnerability or functional defect;
- do not upgrade solely to reach the newest major version;
- review direct and transitive advisories before each release and when dependencies change;
- retain restore, advisory and package provenance evidence with the Release Report;
- escalate a vulnerable minimum version to Architecture and Security for a compatibility-versus-risk decision.

This is recorded in [ADR-007](../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md).

### SEC-004 — Architecture disposition

The library will distinguish three categories of output:

1. **Client-safe output** — content deliberately designed for external callers.
2. **Diagnostic output** — internal exception, implementation or infrastructure detail that should remain in logs or internal telemetry.
3. **Consumer-controlled domain output** — result values, validation messages, property names and custom codes whose safety depends on the consuming application.

The package does not enforce a universal redaction policy because it does not own the consumer’s trust boundary. It must, however, make the distinction explicit in first-party documentation and samples.

### SEC-005 — Architecture disposition

`NestgridResponseOptions.StatusMappings` remains configurable for legitimate consumer integration needs. The default mapping is the normative baseline and must remain covered by regression tests.

Documentation must warn that remapping `Unauthorized`, `Forbidden`, `Error`, `NoContent` and other statuses can alter security, caching and client-control semantics. Consumers own the consequences of custom mappings. No API-breaking restriction is proposed.

## Blocking Issues

- SEC-001 remains blocking for final release approval until Engineering implements ADR-008 and Security re-review confirms the resulting contract and guidance.
- SEC-002 remains Platform-owned and is not an Architecture action.

## Non-blocking Issues

- SEC-003 requires evidence and governance before release; it does not require dependency upgrades by itself.
- SEC-004 and SEC-005 require documentation, test and evidence updates and should be completed before final release where practical.

## Sponsor Decision

For `Results.Error(Exception)`, the Sponsor approved Option B on 2026-08-17.

### Option A — Preserve compatibility

Retain the current raw message/type behaviour, classify the overload as diagnostic-oriented, add a clearly named safe-public conversion path, revise all first-party examples and require explicit residual-risk acceptance before release.

### Option B — Safe default

Change the existing exception overload to return a generic client-safe message by default, add a separate explicit diagnostic conversion path, and provide migration guidance. This is a justified but breaking behavioural change and requires Sponsor approval.

### Decision

Option B is approved. The existing method signatures remain available with safe output, and the previous diagnostic behaviour is available only through explicitly named `ErrorWithDiagnosticDetails` methods. The behavioural change is justified by SEC-001 and the product’s pre-1.0 status.

## Recommendation

Proceed with the SEC-001 design and the SEC-003, SEC-004 and SEC-005 dispositions. Engineering must implement ADR-008 and return the implementation evidence through the normal Engineering handover. Security should then re-review the resulting exception contract and output guidance before final Release approval.
