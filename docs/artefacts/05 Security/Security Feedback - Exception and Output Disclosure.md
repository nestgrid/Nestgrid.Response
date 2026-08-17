# Security Feedback

```yaml
title: Nestgrid.Response v0.7.0 Security Feedback - Exception and Output Disclosure
version: 1.2
status: In Review
owner: Solution Architect
contributors:
  - Security Engineer
produced_by: Security Engineer
consumed_by: Solution Architect, Software Engineer, Quality Engineer, Project Sponsor
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
  - SEC-001
  - SEC-004
  - SEC-005
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Security Assessment.md
  - ../02 Architecture/Architecture Pack.md
  - ../03 Implementation/Implementation Report.md
```

## Context

The approved Architecture Pack identifies validation and result messages as potentially sensitive output. The implementation and package README also provide `Results.Error(Exception)` and demonstrate returning the resulting object after catching an exception.

## Feedback Summary

Consumer responsibility remains important, but it is not sufficient as the only control. The public API shape and first-party example establish a normal path that can disclose raw exception details. Architecture and Engineering should decide whether the API should produce a safe generic public message, require an explicit diagnostic opt-in, or otherwise separate logging details from response details.

The same review should clarify the consumer boundary for validation property names, validation text, result values and custom HTTP status mappings. Architecture/Engineering have now dispositioned SEC-001, SEC-004 and SEC-005, and Engineering has implemented the approved changes.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-001 | P1 | `Results.Error(Exception)` previously serialised `Exception.Message` and the concrete type name. | Internal diagnostic data could cross the application-to-client trust boundary. | **Resolved.** ADR-008, safe default implementation, explicit diagnostic methods, migration guidance and tests are present. |
| SEC-004 | P2 | Output disclosure guidance previously did not clearly separate client-safe content from internal diagnostics. | Validation and domain details could be exposed in responses or logs. | **Resolved for the current candidate.** Architecture disposition and updated package/sample guidance distinguish client-safe, diagnostic and consumer-controlled output. |
| SEC-005 | P2 | Status mapping is configurable without security-specific guidance. | Incorrect mappings could change the security meaning of responses. | **Resolved for the current candidate.** Guidance warns about security-sensitive mappings and both adapters test the normative defaults. |

## Blocking Issues

- No current blocking issue remains in this feedback stream. The explicit diagnostic methods remain a residual consumer-controlled risk and must not be returned directly to untrusted clients.

## Non-blocking Issues

- Future changes to exception output, result serialisation or default mappings require Security re-review.

## Questions

- Should exception conversion remain a convenience API, or should it require an explicit safe public message and separate diagnostic metadata?
- Is preserving the current raw exception message behaviour a deliberate compatibility commitment? If so, who accepts the disclosure risk and what release guidance is mandatory?

## Recommendation

Security confirms the Architecture/Engineering disposition and implementation for SEC-001, SEC-004 and SEC-005. Retain the explicit diagnostic boundary and return only future material changes to Security review.
