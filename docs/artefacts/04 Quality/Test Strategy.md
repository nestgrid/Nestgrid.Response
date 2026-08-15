# Test Strategy

```yaml
title: Nestgrid.Response v0.7.0 Test Strategy
version: 1.0
status: Complete with conditions
owner: Quality Engineer
produced_by: Quality Engineer
consumed_by: Software Engineer, Security Engineer, Platform Engineer, Release Owner
date: 2026-08-15
related_artefacts:
  - ../01 Discovery/Product Brief.md
  - ../02 Architecture/Architecture Pack.md
  - ../03 Implementation/Implementation Report.md
  - ../Release/Roadmap.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Quality position

Nestgrid.Response is a public, pre-1.0 .NET library with five packages and compatibility-sensitive result and HTTP contracts. The highest confidence need is regression protection for existing public behaviour, followed by proof that consumers can install and use each supported package in representative target environments.

The implementation is deterministic and has no persistence, migrations, background processing or runtime service dependency. Testing should therefore concentrate on public contracts, package boundaries, framework execution, package consumption and release evidence rather than broad end-to-end infrastructure testing.

## Scope and requirements traceability

| Requirement area | Verification focus | Planned evidence | Risk |
| --- | --- | --- | --- |
| FR-001–FR-004 | Status vocabulary, success/failure flags, typed/untyped results, messages and factories | Core unit tests and API assertions | High |
| FR-005 | Map/match/transform preserves status and messages | Core unit tests and mutation evidence | High |
| FR-006 | Modern ASP.NET Core result execution, status codes, payload modes and registration | Adapter integration/contract tests plus sample endpoint checks | High |
| FR-007 | MVC result execution and package compatibility | MVC adapter tests and consumer installation against the documented baseline | High |
| FR-008 | Core package has no presentation dependency and is consumable independently | Project/package inspection and framework-independent sample | High |
| FR-009 | DataAnnotations conversion, including additive member-aware conversion | Validation unit tests, package sample and mutation evidence | Medium |
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
- Package target frameworks, dependencies, README/XML documentation inclusion and installability.

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

## Proposed verification extensions

No production changes are proposed by Quality. Subject to approval, extend tests or verification in this order:

1. Add adapter contract scenarios that assert status code, headers and body shape for every normative status, both success response modes and representative custom mappings in modern ASP.NET Core and MVC.
2. Add package-consumer smoke projects or equivalent isolated checks for each documented target/framework combination, including MVC package loading.
3. Exercise the ASP.NET Core and MVC samples through representative endpoints, covering success, invalid input and at least one non-success result.
4. Add validation tests for null code, null validation elements and all supported severity values if mutation analysis identifies surviving mutants in those paths.
5. Run the five configured Stryker jobs and retain JSON/HTML reports with the candidate evidence.

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
- The current local test run reports 269 passed, 0 failed and 0 skipped. The Engineering Report records 265 passed; this count discrepancy should be reconciled in the final evidence pack.
- Fresh Cobertura reports exist, but package-level coverage is uneven and is not a substitute for behaviour or mutation evidence.
- No current candidate mutation report, CI run, package-install consumer run or endpoint-level sample run is retained.

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

- Is the documented 100% line and mutation target a hard release gate for v0.7.0, or may exceptions be approved per package with rationale?
- Which exact MVC maintenance duration and review triggers accompany the supported `Microsoft.AspNetCore.Mvc.Core` `2.1.38` baseline?

## Approved execution decisions

- Quality coverage target: over 90% line coverage for each package’s own production source, with branch coverage reported separately.
- Mutation target: restore 100% where feasible; surviving equivalent mutants must be documented rather than hidden.
- MVC compatibility baseline: retain `Microsoft.AspNetCore.Mvc.Core` `2.1.38` unless a demonstrated functionality defect requires a change and Architecture approves it.

## Execution outcome

- Full Release regression: 277 passed, 0 failed, 0 skipped.
- Package-owned line coverage: Core 100%, HTTP 100%, ASP.NET Core 97.7%, MVC 100%, Validation 100%.
- Validation branch coverage: 92.3%.
- All five configured mutation suites reached 100% mutation score: Core, HTTP, ASP.NET Core, MVC and Validation.
- All five 0.7.0 packages and symbol packages were packed successfully.
- A net8.0 consumer restored the five generated packages from the local package feed plus NuGet.org and built successfully.
- Core and validation console samples ran successfully; ASP.NET Core and MVC web samples started successfully.
- MVC verification retained `Microsoft.AspNetCore.Mvc.Core` 2.1.38 as the compatibility baseline.

## Recommendation checkpoint

The strategy execution is complete. Quality recommends handover to Security, Platform and Release with the evidence and limitations recorded in the Release Readiness Report.
