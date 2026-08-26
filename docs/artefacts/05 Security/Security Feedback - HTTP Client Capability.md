# Security Feedback

```yaml
title: Nestgrid.Response v0.8.0 Security Feedback - HTTP Client Capability
version: 1.0
status: In Review
owner: Security Engineer
contributors:
  - Morgan profile
produced_by: Security Engineer
consumed_by: Software Engineer, Solution Architect, Quality Engineer, Platform Engineer, Project Sponsor
date: 2026-08-26
related_decisions:
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
related_work_items:
  - SEC-007
  - SEC-008
related_artefacts:
  - Security Assessment.md
  - ../03 Implementation/Implementation Report - HTTP Client Capability.md
  - ../04 Quality/Release Readiness Report - HTTP Client Capability.md
```

## Context

The v0.8.0 candidate adds a client adapter that reads HTTP response content from remote services and constructs Nestgrid results. The new boundary is security-sensitive because response content, serializer failures and response size are controlled by a remote system or intermediary.

## Findings and required actions

| ID | Severity | Finding | Required action | Owner |
| --- | --- | --- | --- | --- |
| SEC-007 | P2 | The reader buffers an unbounded response body in memory. | Add a bounded response-size policy with a safe default, an explicit documented opt-in for larger payloads, enforcement during reading and over-limit tests. Escalate the public option shape to Architecture. | Software Engineer / Solution Architect / Quality Engineer |
| SEC-008 | P2 | Public protocol exceptions can expose retained inner exception details from serializer or custom-converter failures. | Remove or sanitise inner exceptions from public protocol failures, decide the safe public-constructor contract and add hostile-converter tests covering exception message and inner exception. Escalate public API changes to Architecture. | Software Engineer / Solution Architect / Quality Engineer |

## Controls confirmed

- The package has no authentication, authorisation, credential storage, logging, retry or endpoint-discovery responsibility.
- The package does not deserialize core result types directly and does not place response bodies or sensitive headers in its fixed protocol messages.
- Normal exception conversion remains safe by default under ADR-008.
- Media types, malformed payloads, invalid messages, invalid severities, cancellation, response ownership and status mappings have focused tests.
- All reviewed workflow actions use immutable commit SHAs; protected publication and package provenance remain Platform/Release evidence.
- The dependency evidence declares the approved `System.Text.Json 4.6.0` baseline and reports no current vulnerable package path in the evaluated candidate graphs.

## Recommendation

Do not approve final publication while SEC-007 or SEC-008 remains unresolved or explicitly accepted by the authorised risk owner. Platform and Release review may proceed in parallel for evidence planning. This feedback is a Security recommendation, not implementation or release approval.
