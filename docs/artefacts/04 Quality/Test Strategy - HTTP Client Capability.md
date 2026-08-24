# Test Strategy — HTTP Client Capability

```yaml
title: Nestgrid.Response v0.8.0 HTTP Client Capability Test Strategy
version: 1.0
status: Complete with conditions
owner: Quality Engineer
produced_by: Quality Engineer
consumed_by: Software Engineer, Security Engineer, Platform Engineer, Release Owner, Project Sponsor
date: 2026-08-24
related_decisions:
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
related_work_items:
  - IR-011
  - IR-012
  - IR-013
  - IR-014
  - IR-015
related_artefacts:
  - ../02 Architecture/Architecture Feedback - HTTP Client Implementation Review.md
  - ../03 Implementation/Implementation Report - HTTP Client Capability.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Quality position

The HTTP client capability is a new, compatibility-sensitive `netstandard2.0` package that interprets external HTTP responses into existing `Result` and `Result<T>` contracts. Quality therefore prioritises wire-contract strictness, status semantics, protocol-safety, resource ownership, cancellation and package independence over broad live-network testing.

The existing five-package v0.7.0 product remains the regression baseline. The client package is an additive v0.8.0 candidate and must not alter server-side mappings, MVC support, core construction or existing public behaviour.

## Requirements coverage

| Requirement | Verification focus | Current result | Risk |
| --- | --- | --- | --- |
| ADR-009 package boundary | Separate `netstandard2.0` package with core and `System.Text.Json` only; no ASP.NET Core, MVC, DI, authentication, retry or telemetry dependencies | Covered by project inspection and package metadata | High |
| ADR-010 FullResult wire contract | Generic/non-generic envelopes, value and message preservation, internal DTOs and existing factory construction | Covered by focused tests | High |
| ADR-010 ValueOnly wire contract | Generic direct values, non-generic envelope behaviour, failure envelopes and explicit mode handling | Covered by focused tests | High |
| ADR-011 client status policy | 200, 201, 202, 204, 400, 401, 403, 404, 409, 422, 5xx, custom, 3xx and unmapped statuses | Covered by matrix and custom-mapping tests | High |
| Protocol safety | Malformed JSON, invalid envelopes/messages/severity, missing values, media types and no raw-body disclosure | Covered by focused tests | High |
| Ownership and failure separation | Caller-owned response lifetime, convenience-method response disposal, transport exception and cancellation propagation | Covered by focused tests; sample execution environment limited | High |
| Compatibility | Existing five-package regression and MVC `2.1.38` baseline remain unchanged | Covered; full suite passes | High |
| Package consumption | Package contents, target, README/XML documentation, dependency metadata and consumer installation | Package inspection passed; supported-CI publication remains open | High |
| API stability | Additive public surface and explicit contract inventory | Candidate is additive; IR-011–IR-015 remain open 1.0 work | High |

## Test levels

| Level | Required proof | Result |
| --- | --- | --- |
| Unit/contract | Reader, options, wire conversion, mappings and protocol failures | 73 client tests passed |
| Integration | Fake `HttpMessageHandler` and convenience methods over normal `HttpClient` | Passed in client suite |
| Regression | All existing packages plus new package | 362 passed, 0 failed, 0 skipped |
| Consumer/package | Pack, inspect `.nuspec`, README, XML and local consumer installation | New package pack and metadata inspection passed |
| Sample | Licence-service FullResult and Portal-to-Finance ValueOnly proving scenarios | Engineering reports success; local rerun encountered the known build hang |
| Coverage | Package-owned line coverage above 90% | Client 94.16%; existing packages retain 97.7–100% evidence |
| Mutation | Dedicated client mutation suite and existing package suites | Client 86.62%, below the 90% break threshold; existing five-package evidence remains 100% |

## Verification performed

1. Full Release solution regression: 362 passed, 0 failed, 0 skipped.
2. Focused HTTP client suite: 73 passed, 0 failed, 0 skipped.
3. Package-owned client line coverage: 94.16%; branch coverage 91.4%.
4. Existing package coverage and mutation evidence retained from the current v0.7 baseline.
5. New client package packed successfully as `Nestgrid.Response.Http.Client.0.8.0.nupkg` and `.snupkg`.
6. Package contains the netstandard2.0 assembly, XML documentation, README, icon and dependency metadata for only `Nestgrid.Response 0.8.0` and `System.Text.Json 4.6.0`.
7. Dedicated Stryker configuration was added and executed sequentially. The result was 86.62%, below the configured 90% break threshold.
8. Engineering’s 40-test baseline was extended to 73 tests for result-factory branches, invalid custom status mappings, exact protocol-safe messages, null guards, empty failures, cancellation and generic missing-value behaviour.

## Evidence limitations

- The local client sample build/run encountered the repository’s known local build hang; Engineering’s successful sample evidence remains available, but supported CI should repeat it.
- Live endpoints, authentication, resilience, retry, telemetry and handler composition beyond fake-handler proof are consumer responsibilities and are outside this package’s scope.
- The current client mutation score is not sufficient for release confidence. The suite killed 122 mutants, with 19 surviving and one timeout; survivors remain concentrated in reader control-flow, convenience-method async plumbing and internal exception-detail strings. These should be reviewed for meaningful contract gaps versus equivalent implementation mutations.
- IR-011 through IR-015 remain open 1.0 API-stability findings and are not silently resolved by this additive package.

## Quality exit criteria

Quality can recommend release only when all relevant tests pass, package coverage is proportionate, mutation effectiveness is at or above the approved threshold or explicitly accepted, package/consumer evidence is retained, security review is complete and the outstanding limitations have an owner and disposition.

## Current recommendation

Functional confidence is high, and the package-owned coverage target is met. Quality does not recommend v0.8.0 release yet because the client mutation score is 86.62% against the 90% break threshold and the local proving-sample execution was not independently completed in this environment.
