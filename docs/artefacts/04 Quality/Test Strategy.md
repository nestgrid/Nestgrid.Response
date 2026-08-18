# Test Strategy

```yaml
title: Nestgrid.Response v0.7.0 Test Strategy
version: 1.3
status: Complete with conditions
owner: Quality Engineer
produced_by: Quality Engineer
consumed_by: Software Engineer, Security Engineer, Platform Engineer, Release Owner
date: 2026-08-18
related_artefacts:
  - ../01 Discovery/Product Brief.md
  - ../02 Architecture/Architecture Pack.md
  - ../03 Implementation/Implementation Report.md
  - ../03 Implementation/SEC-006 Closure Evidence.md
  - ../07 Release/Roadmap.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Quality position

Nestgrid.Response is a public, pre-1.0 .NET library with five packages and compatibility-sensitive result and HTTP contracts. The highest confidence need is regression protection for existing public behaviour, followed by proof that consumers can install and use each supported package in representative target environments.

The implementation is deterministic and has no persistence, migrations, background processing or runtime service dependency. The current candidate nevertheless contains a security-sensitive behavioural correction (`ADR-008`) and a dependency-management change (`ADR-007`), so prior Quality evidence must be treated as historical until rerun against this source state.

## Scope and requirements traceability

| Requirement area | Verification focus | Planned evidence | Risk |
| --- | --- | --- | --- |
| FR-001–FR-004 | Status vocabulary, success/failure flags, typed/untyped results, messages and factories | Core unit tests and API assertions | High |
| FR-005 | Map/match/transform preserves status and messages | Core unit tests and mutation evidence | High |
| FR-006 | Modern ASP.NET Core result execution, status codes, payload modes and registration | Adapter integration/contract tests plus sample endpoint checks | High |
| FR-007 | MVC result execution and package compatibility | MVC adapter tests and consumer installation against the documented baseline | High |
| FR-008 | Core package has no presentation dependency and is consumable independently | Project/package inspection and framework-independent sample | High |
| FR-009 | DataAnnotations conversion, including additive member-aware conversion | Validation unit tests, package sample and mutation evidence | Medium |
| ADR-008 contract | Safe generic exception output by default; explicit diagnostic output only through named methods; typed and untyped overloads | Core tests, adapter serialisation checks and mutation evidence | High |
| ADR-007 policy | Central package versions preserve the approved dependency baselines, including MVC `2.1.38` | Restore graph, package verification and current advisory evidence | High |
| NFR-001–NFR-006 | Predictability, immutability, dependency boundaries, compatibility and documentation alignment | Regression suite, API/package inspection, link check and review | High |
| OR-001–OR-006 | Pack contents, installation, targets, dependencies, upgrade/support guidance and samples | CI-equivalent pack/install checks and retained package evidence | High |

## Risk-based test model

### Highest-risk behaviours

- Default and custom status-to-HTTP mappings shared by both adapter families.
- Full-result versus value-only response payload modes.
- Typed and untyped result conversions, including null values and non-success outcomes.
- Immutable result and message behaviour.
- MVC package loading and execution against the documented `Microsoft.AspNetCore.Mvc.Core` baseline.
- Additive validation conversion: member expansion, source ordering, blank-member filtering, defaults, severity and null handling.
- Safe versus diagnostic exception conversion, including null exceptions, message/code absence, typed results and HTTP response boundaries.
- Package target frameworks, dependencies, README/XML documentation inclusion and installability.
- Central package-management evaluation and minimum-compatible dependency evidence.

### Test levels

| Level | Use in this product | Required outcome |
| --- | --- | --- |
| Unit | Core semantics, messages, mappings and validation rules | Fast, deterministic protection of public behaviour and edge cases |
| Integration/contract | HTTP policy with each adapter and framework response execution | Proves boundaries unit tests cannot prove, including response shape and status code |
| Consumer/package | Pack, install and compile/run representative consumers for core, modern ASP.NET Core, MVC and validation | Proves the distributed product rather than only project references |
| End-to-end/sample | One meaningful success and failure/validation flow per web sample; framework-independent sample execution | Proves assembled consumer journeys without duplicating every unit case |
| Mutation | Each configured package/test pair | Demonstrates that important tests detect meaningful behavioural changes |
| Static/documentation | API/package metadata, links, README claims and target/dependency matrix | Prevents support and installation claims drifting from implementation |

Performance, persistence, recovery and service availability testing are not required for this change because the product has no such runtime responsibilities. Security-sensitive output handling is handed to Security; Quality will verify only the observable contract and documentation warning that consumers own disclosure policy.

## Verification performed

No production changes were required by Quality. The current candidate was verified as follows:

1. Full Release regression: 289 passed, 0 failed, 0 skipped.
2. Package-owned line coverage: Core 100%, HTTP 100%, ASP.NET Core 97.7%, MVC 100%, Validation 100%.
3. Sequential Stryker runs: all five configured package suites reached 100%; parallel runs were discarded because shared Debug outputs interfered.
4. Current core and adapter tests cover safe and diagnostic exception paths, typed and untyped results, null exceptions and output-sensitive mappings.
5. Core, HTTP and Validation standard pack plus `scripts/verify-packages.sh` evidence is retained. ASP.NET Core metadata is cross-checked, and the MVC package metadata, hash and supported consumer restore/build/execute evidence are retained in the Engineering SEC-006 Closure Evidence artefact.
6. Dependency restore and central-version resolution completed; current advisory/provenance evidence remains a Security/Platform release condition.
7. Web samples were startup-checked; endpoint-level assertions remain a non-blocking follow-up.

These are verification activities and test-only changes. Any production defect found must return to Engineering for remediation.

## Entry criteria

- Approved Product Brief, Architecture Pack and Engineering Handover are available.
- Engineering reports implementation complete with conditions.
- Candidate source and tests are stable enough to execute.
- The exact MVC compatibility/support policy is confirmed or explicitly marked as a release blocker.

## Exit criteria for Quality recommendation

- All relevant automated tests pass with no unexplained failures or skips.
- Requirements and high risks trace to observable evidence.
- Mutation and coverage results are retained and exceptions are explicit.
- Package build, contents, installation and representative consumer execution succeed.
- Adapter response contracts and web sample flows are verified.
- Defects are separated from improvements, assigned, and either resolved or formally accepted by the appropriate authority.
- Security and Platform receive the latest evidence and limitations.

## Evidence limitations

- The first solution-level test command was blocked by the sandbox's MSBuild named-pipe permission restriction. Re-running with isolated compilation succeeded, so the environment limitation is not evidence of a product failure.
- The previous Quality run covered a pre-ADR-008 candidate; its 277-test and 100%-mutation results are not current-candidate evidence.
- Current Quality-owned regression, coverage and mutation evidence is now recorded below; final Security closure, protected publication and supported-CI provenance remain downstream conditions.
- Security identifies SEC-002 as a Platform release condition and SEC-003 as a dependency-evidence condition; Quality will track their effect on the release recommendation but does not own their remediation.

## Minimum package-consumer compatibility matrix

The minimum matrix should prove the packages consumers actually install, while avoiding unsupported claims about frameworks that are not part of the approved product baseline.

| Consumer scenario | Package(s) | Minimum environment | Required check | Support interpretation |
| --- | --- | --- | --- | --- |
| Framework-independent application | `Nestgrid.Response` | `net8.0` application referencing the `netstandard2.0` package | Restore, compile, create typed/untyped results, serialise and run the core sample | Supported baseline; the package target remains `netstandard2.0` |
| Shared HTTP policy consumer | `Nestgrid.Response.Http` + core | `net8.0` application | Restore, compile and exercise default/custom mappings and response modes | Supported as the shared policy used by both adapters |
| Modern ASP.NET Core consumer | `Nestgrid.Response.AspNetCore` + dependencies | ASP.NET Core `net8.0` application | Restore, start host, call representative success, validation and non-success endpoints, assert status and response shape | Supported modern adapter baseline |
| MVC consumer | `Nestgrid.Response.Mvc` + dependencies | `Microsoft.AspNetCore.Mvc.Core` `2.1.38`, hosted by a `net8.0` test/sample application | Restore, load the package, execute representative controller results and assert status/body contracts | Supported MVC baseline; do not imply support for another MVC dependency version |
| DataAnnotations consumer | `Nestgrid.Response.Extensions.Validation` + core | `net8.0` application referencing the `netstandard2.0` package | Restore, run ordinary and member-aware validation conversion, assert messages, codes, properties and severity | Supported validation extension baseline |

The matrix should also include one packaging check per row: consume the generated `.nupkg` from a local package source rather than only a project reference. A `netstandard2.0` class-library compile check is useful for the three portable packages, but it is an additional compatibility signal rather than a claim that every `netstandard2.0` consumer runtime is supported. `net8.0` is the minimum executable environment because it is the repository’s maintained SDK and modern adapter target. Other runtimes or MVC package versions should be added only after an explicit support decision and available representative tooling.

## Questions for confirmation

- Is the documented 100% line and mutation target a hard release gate for the current candidate, or may exceptions be approved per package with rationale?
- MVC follows the common library maintenance, versioning, support and review lifecycle; it has no separate maintenance lifecycle or independent end-of-support policy. The baseline remains `Microsoft.AspNetCore.Mvc.Core` `2.1.38`.

## Current Quality recommendation

- Quality retained the over-90% package-owned line target, achieved 100% mutation across all five configured suites, and retained `Microsoft.AspNetCore.Mvc.Core` `2.1.38` as the MVC baseline.
- Quality does not recommend release until Q-007's Critical/High dependency advisories are remediated or formally accepted by the authorised owners; this is a release risk disposition, not a test shortfall.

## Recommendation checkpoint

The strategy execution is complete. Quality recommends proceeding to Security, Platform and Release review, subject to the downstream conditions recorded in the Release Readiness Report.
