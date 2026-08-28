# Security Feedback

```yaml
title: Nestgrid.Response v0.8.0 Security Feedback - HTTP Client Capability
version: 1.4
status: Complete with conditions
owner: Security Engineer
contributors:
  - Morgan profile
produced_by: Security Engineer
consumed_by: Software Engineer, Solution Architect, Quality Engineer, Platform Engineer, Project Sponsor
date: 2026-08-27
related_decisions:
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
related_work_items:
  - SEC-007
  - SEC-008
  - SEC-009
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
| SEC-007 | P2 | The reader previously buffered an unbounded response body in memory. | **Resolved.** A 1 MiB default cumulative limit, positive-limit validation and boundary/chunked/cancellation tests are in place. | Software Engineer / Quality Engineer |
| SEC-008 | P2 | Protocol exceptions previously retained inner exception details from serializer or custom-converter failures. | **Resolved.** Package-generated protocol exceptions now have fixed safe messages and no inner exception; hostile-converter tests cover the boundary. | Software Engineer / Quality Engineer |
| SEC-009 | P2 | The latest Quality and Engineering reports previously recorded different test counts and package evidence baselines. | **Resolved.** Both reports now record 91 client tests, 380 total tests, matching quality metrics and package hash `94dd1dbe24f0f1d08ca2783eee488ceb4e05892ab585150560696e71f0aa5484` built at evidence commit `8e91306`. The later `79ef468` commit is documentation-only. | Software Engineer / Quality Engineer |

## Controls confirmed

- The package has no authentication, authorisation, credential storage, logging, retry or endpoint-discovery responsibility.
- The package does not deserialize core result types directly and does not place response bodies or sensitive headers in its fixed protocol messages.
- Normal exception conversion remains safe by default under ADR-008.
- Media types, malformed payloads, invalid messages, invalid severities, cancellation, response ownership and status mappings have focused tests.
- All reviewed workflow actions use immutable commit SHAs; protected publication and package provenance remain Platform/Release evidence.
- The dependency evidence declares the approved `System.Text.Json 4.6.0` baseline and reports no current vulnerable package path in the evaluated candidate graphs.

## Recommendation

SEC-007, SEC-008 and SEC-009 are closed from Security’s perspective. Do not approve final publication while protected-CI provenance and final Release evidence are absent. Platform and Release review may proceed in parallel for evidence planning. This feedback is a Security recommendation, not implementation or release approval.
