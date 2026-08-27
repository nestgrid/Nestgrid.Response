# Security Assessment

```yaml
title: Nestgrid.Response v0.8.0 Security Assessment
version: 1.8
status: In Review
owner: Security Engineer
contributors:
  - Morgan profile
produced_by: Security Engineer
consumed_by: Solution Architect, Software Engineer, Quality Engineer, Platform Engineer, Project Sponsor
date: 2026-08-27
supersedes: Security Assessment v1.6
related_decisions:
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
related_work_items:
  - SEC-001
  - SEC-002
  - SEC-003
  - SEC-004
  - SEC-005
  - SEC-006
  - SEC-007
  - SEC-008
  - SEC-009
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../02 Architecture/Architecture Pack.md
  - ../02 Architecture/Architecture Recommendation - HTTP Client Capability.md
  - ../03 Implementation/Implementation Plan - HTTP Client Capability.md
  - ../03 Implementation/Implementation Report - HTTP Client Capability.md
  - ../04 Quality/Release Readiness Report - HTTP Client Capability.md
  - ../06 Platform/Operational Readiness Review.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Scope

This re-assessment reviews the v0.8.0 candidate, including the additive `Nestgrid.Response.Http.Client` package, source and tests, package metadata, samples, solution visibility, serializer and HTTP response boundaries, GitHub Actions publication controls, dependencies and the current Discovery, Architecture, Engineering, Quality, Platform, Release and Independent Review artefacts.

It covers authentication and authorisation boundaries, untrusted response handling, data disclosure, protocol exceptions, resource exhaustion, serializer configuration, dependency integrity, secrets and operational publication controls. It does not approve publication, accept risk for another role or progress beyond the Recommend stage.

## Threat model

### Assets

- Consumer application data and typed values received over HTTP.
- Structured messages, property names, codes and severity values.
- Exception and protocol-failure diagnostics.
- Package contents, dependency graph, publication identity and provenance.

### Actors and trust boundaries

- A malicious or compromised remote service returning hostile, malformed or oversized content.
- A compromised dependency or GitHub Action attempting to alter the package or obtain publication authority.
- Consumer code, custom converters or logging that forwards sensitive values or exception details.

The principal new boundary is `HttpResponseMessage` content and headers entering the client library. The library is not an authentication, authorisation, transport or endpoint-trust mechanism; consumers own those controls.

## Assessment

The new package stays within the approved additive boundary: it targets `netstandard2.0`, depends on core and the centrally managed `System.Text.Json` baseline, uses internal wire DTOs, constructs results through existing factories and does not introduce authentication, credentials, DI, retries, logging or telemetry. Status interpretation is explicit and does not reverse server mappings.

The current evidence is strong for normal protocol behaviour. The full Release suite passes 366/366, the client suite passes 77/77, client coverage is reported at 98.30% line and 95.76% branch, and mutation testing is reported at 90.08%. Tests cover malformed JSON, invalid messages and severities, media types, cancellation, response ownership, custom converters, status mappings and safe public protocol messages. The generated package metadata declares only `Nestgrid.Response 0.8.0` and `System.Text.Json 4.6.0`.

The normal `Results.Error(Exception)` safe-default control remains effective. The client avoids placing response bodies or headers in its own fixed protocol messages. The previously identified unbounded buffering and exception-chain disclosure conditions have been remediated through a 1 MiB default cumulative limit, safe limit validation and normalised protocol exceptions with no inner exception. The remaining security concern is evidence traceability: the current Engineering report has not yet been synchronised with the latest Quality test counts and package evidence.

## Authentication

Not applicable to the library runtime. The client package does not authenticate endpoints or callers. Consumers own credentials, handler composition, TLS validation, redirect policy and endpoint trust.

## Authorisation

Not applicable to the library runtime. HTTP statuses such as `401` and `403` are interpreted as observed outcomes; the package does not grant or enforce access.

## Data protection

The package does not persist response data or secrets. Values, messages, properties and codes remain consumer-controlled and may be sensitive. Diagnostic logging and external response disclosure remain consumer responsibilities.

## Input and output handling

The reader rejects malformed JSON, invalid message collections, invalid severities, unsupported media types and unmapped statuses with fixed protocol messages. It accepts missing media types for compatibility. The previously unresolved size and exception-chain issues are recorded as remediated SEC-007 and SEC-008. The remaining traceability condition is SEC-009.

## Secrets and configuration

The package stores no runtime secrets. Serializer and status-mapping options are consumer configuration; the reader copies them at construction. NuGet OIDC identity and repository permissions remain GitHub/NuGet operational controls.

## Dependencies

The package metadata declares only `Nestgrid.Response 0.8.0` and `System.Text.Json 4.6.0`. The approved minimum-compatible dependency policy remains in force. Current evaluated candidate graphs report no vulnerable packages; supported-CI provenance remains outstanding.

## Logging and operational security

The package performs no logging, telemetry, retry or resilience policy. Consumers should avoid logging response bodies and exception chains without a data-classification decision. The protected publication workflow is configured with immutable action SHAs and the `nuget` environment, but v0.8.0 execution and provenance are not yet retained.

## Findings

| ID | Severity | Type | Finding | Impact | Mitigation / owner |
| --- | --- | --- | --- | --- | --- |
| SEC-001 | P1 | Vulnerability remediated | Normal exception conversion previously exposed exception details. | Internal diagnostics could cross an API boundary. | Resolved by ADR-008, generic normal output, explicit diagnostic methods and regression tests. |
| SEC-002 | P1 | Risk controlled; evidence open | Publication workflow actions are pinned to immutable SHAs and publication uses the protected `nuget` environment with OIDC. | A failed or unproven protected run could leave package provenance uncertain. | Platform/Release must retain a successful v0.8.0 protected run and package hashes; no publication approval is given here. |
| SEC-003 | P2 | Governance controlled; evidence open | The minimum-compatible dependency policy is recorded and current package evidence is retained, but supported-CI provenance is not yet present for v0.8.0. | Consumers could receive a package graph different from the evaluated graph. | Retain supported-CI advisory, restore, package and provenance evidence under ADR-007. Do not upgrade solely for recency. Platform/Release owner. |
| SEC-004 | P2 | Risk controlled | Client-safe, diagnostic and consumer-controlled output boundaries are documented. | Consumers can still disclose validation or diagnostic content if they deliberately return it to an untrusted caller. | Safe defaults, explicit diagnostic APIs and consumer review remain required. Consumer application owner. |
| SEC-005 | P2 | Risk controlled | Client HTTP status mappings are explicit, immutable by reader construction and independent of server mappings. | Consumer policy misconfiguration may assign misleading semantic outcomes. | Preserve ADR-011 mappings, review custom mappings and keep authentication/authorisation in consumers. Consumer and Architecture owners. |
| SEC-006 | P1 | Vulnerability remediated for evaluated graph | Historical vulnerable dependency paths were remediated; current evaluated graphs contain no vulnerable packages. | A publication mismatch could reintroduce an unverified vulnerable path. | Security closes the evaluated candidate position; Platform/Release must prove protected-CI package identity and provenance. No vulnerability risk is accepted. |
| SEC-007 | P2 | Vulnerability / availability remediated | The reader previously buffered an unbounded response body. | A malicious or compromised endpoint could cause excessive memory and allocation pressure. | **Resolved.** `MaxResponseBodyBytes` defaults to 1 MiB, validates positive limits and enforces the cumulative limit while streaming for successful and failed responses. Boundary, chunked and cancellation tests are present. |
| SEC-008 | P2 | Vulnerability / disclosure condition remediated | Protocol failures previously retained serializer or custom-converter inner exceptions. | Exception inspection or logging could disclose response-derived or converter diagnostics. | **Resolved.** Protocol exception construction is package-internal, package-generated failures contain no inner exception, and hostile converter tests assert safe messages and null inner exceptions. |
| SEC-009 | P2 | Evidence/control gap | The latest Quality evidence records 380 full-solution tests and 91 client tests, while the Engineering report still records 374 and 85 and its package hash points to an earlier evidence commit. | Release reviewers may be unable to identify the exact tested package and evidence baseline. | Engineering must refresh the v0.8.0 Implementation Report with the latest test counts, coverage/mutation results, package hash and current commit, then Quality/Security should re-check cross-artefact consistency. This is release-blocking as a traceability condition, not a confirmed product vulnerability. |

## Improvements, not confirmed vulnerabilities

- `NestgridResponseClientOptions.SerializerOptions` remains a mutable .NET object exposed by the approved API. The reader snapshots it, so later option mutation does not alter an existing reader, but the API should document that options are configuration input rather than a security boundary.
- Missing content type is intentionally accepted for legacy compatibility. Consumers should require `application/json` or an approved `+json` type when endpoint trust and strict protocol enforcement require it.
- The deterministic sample does not prove live authentication, TLS, retry, logging or endpoint trust. Those remain consumer responsibilities and are not product vulnerabilities.
- Existing Independent Review findings IR-011–IR-015 concern 1.0 API stability and are not security vulnerabilities introduced by v0.8.0.
- The dedicated client mutation score is reported locally, but the current GitHub mutation matrix does not include the client configuration. Add it to the CI gate so this security-relevant parser evidence is continuously enforced; this is an evidence/control improvement, not a confirmed vulnerability.

## Accepted risks

No accepted security risks are recorded. In particular, no authority has accepted SEC-009, the v0.8.0 provenance gap or a known dependency vulnerability.

## Assumptions and evidence limitations

- The Security assessment relies on the current Engineering and Quality evidence; the local advisory command did not complete in this environment because registry access did not return promptly.
- The current local Release run passed 380 tests, including 91 HTTP client tests. The dedicated client mutation result is reported locally, but its configuration is not yet included in the GitHub mutation matrix.
- The deterministic sample does not establish live endpoint, authentication, TLS or production logging behaviour.

## Residual risks requiring ownership

| Risk | Owner | Position |
| --- | --- | --- |
| Consumers may return diagnostic exception details, validation content or custom status outcomes to untrusted callers. | Consumer application owner | Documented consumer responsibility; safe-by-default APIs reduce but cannot prevent deliberate disclosure. |
| Endpoint authentication, TLS validation, redirect policy, retries and logging may be misconfigured by a consumer. | Consumer application owner | Outside this package’s approved scope; use standard `HttpClient` controls and review sensitive logging. |

## Release recommendation

Security recommends **conditional progression to Platform and Release review**, but does not recommend final release approval for v0.8.0 at this stage.

Platform and Release may consume this assessment for planning and evidence review. Before final release approval, Engineering must synchronise the Implementation Report and resolve or obtain explicit authorised disposition for SEC-009; Platform/Release must retain the protected-CI publication run, package hashes and supported consumer/provenance evidence; and Release must record the final candidate and decision. SEC-007 and SEC-008 are closed. This assessment remains at Recommend pending approval and does not authorise implementation or publication.
