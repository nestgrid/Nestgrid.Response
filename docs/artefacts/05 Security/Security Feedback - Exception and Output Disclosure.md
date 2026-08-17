# Security Feedback

```yaml
title: Nestgrid.Response v0.7.0 Security Feedback - Exception and Output Disclosure
version: 1.1
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

The same review should clarify the consumer boundary for validation property names, validation text, result values and custom HTTP status mappings. Architecture/Engineering now own disposition of SEC-001, SEC-004 and SEC-005.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-001 | P1 | `Results.Error(Exception)` serialises `Exception.Message` and the concrete type name. | Internal diagnostic data can cross the application-to-client trust boundary. | Solution Architect and Software Engineer to define and implement the safe contract, with compatibility impact and migration guidance. |
| SEC-004 | P2 | Output disclosure guidance does not clearly separate client-safe content from internal diagnostics. | Validation and domain details may be exposed in responses or logs. | Software Engineer to update package READMEs and samples; Quality to add representative output-policy assertions. |
| SEC-005 | P2 | Status mapping is configurable without security-specific guidance. | Incorrect mappings may change the security meaning of responses. | Architecture and Engineering to document protected default semantics and test `Unauthorized`, `Forbidden`, `Error` and `NoContent` handling. |

## Blocking Issues

- SEC-001 must be dispositioned by Architecture/Engineering and mitigated or explicitly accepted before final release approval.

## Non-blocking Issues

- SEC-004 and SEC-005 may be completed as follow-up improvements only if the responsible role confirms that the current defaults and release documentation are sufficient for the target consumers.

## Questions

- Should exception conversion remain a convenience API, or should it require an explicit safe public message and separate diagnostic metadata?
- Is preserving the current raw exception message behaviour a deliberate compatibility commitment? If so, who accepts the disclosure risk and what release guidance is mandatory?

## Recommendation

Return SEC-001 to Architecture for a decision and Engineering for implementation planning. Do not silently change the public contract within Security.
