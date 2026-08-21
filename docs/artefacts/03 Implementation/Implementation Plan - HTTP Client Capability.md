# Nestgrid.Response HTTP Client Capability Implementation Plan

```yaml
title: Nestgrid.Response v0.8.0 HTTP Client Capability Implementation Plan
version: 1.0
status: Complete with conditions — handed to Quality and Security
owner: Software Engineer
contributors:
  - Mason profile
produced_by: Software Engineer
consumed_by: Project Sponsor, Solution Architect, Quality Engineer, Security Engineer, Platform Engineer
date: 2026-08-21
supersedes:
related_decisions:
  - ../../decisions/ADR-009-HTTP-Client-Adapter-Boundary.md
  - ../../decisions/ADR-010-HTTP-Client-Wire-Contract.md
  - ../../decisions/ADR-011-HTTP-Client-Outcome-Semantics.md
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
related_work_items:
  - IR-011
  - IR-012
  - IR-013
  - IR-014
  - IR-015
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../02 Architecture/Architecture Pack.md
  - ../02 Architecture/Engineering Handover.md
  - ../02 Architecture/Architecture Recommendation - HTTP Client Capability.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Purpose and approval boundary

This plan defines the Engineering work for the approved additive `Nestgrid.Response.Http.Client` capability. It is the implementation-stage Recommend checkpoint for the v0.8.0 branch.

The Architecture Recommendation, Architecture Pack, Engineering Handover and ADR-009 through ADR-011 authorise this plan and define the solution boundary. They do not authorise Engineering implementation by themselves. No source, test, package or sample implementation is included in this checkpoint. Implementation begins only after the Project Sponsor or delegated Engineering approver approves this plan.

The already-published v0.7.0 five-package set is unchanged. The sixth package is an additive v0.8.0 candidate and is not a release or publication decision.

## Engineering Readiness Assessment

### Outcome

**Ready with conditions for implementation planning; not yet authorised for implementation.**

The approved architecture is sufficiently specific to plan and implement the capability without inventing product scope. The conditions are:

- approval of this plan before implementation;
- preservation of ADR-009 through ADR-011 and the existing five-package boundary;
- no change to core construction, MVC support, target-framework promises or deferred capabilities;
- completion of the required package, wire-contract, consumer and security evidence; and
- downstream Quality, Security, Platform and Release review after Engineering assurance.

### Inputs reviewed

- Software Engineer role and Mason engineering profile.
- Engineering Handbook workflow, artefact, dependency, testing and gate guidance.
- Approved Product Brief and Discovery-to-Architecture handover.
- Architecture Pack v1.2 and Engineering Handover v1.2.
- HTTP client Architecture Recommendation.
- ADR-009 HTTP Client Adapter Boundary.
- ADR-010 Client Wire Contract and Result Construction.
- ADR-011 Client HTTP Outcome Semantics.
- ADR-007 minimum-compatible dependency policy and ADR-008 safe exception conversion.
- Canonical Independent Review v2.4, including open IR-011 through IR-015.
- Existing source, tests, samples, solution structure, package-management files and v0.7.0 release evidence.

### Confirmed boundaries

- `Nestgrid.Response.Http.Client` is a separate sibling package targeting `netstandard2.0`.
- It depends on core and the centrally managed supported `System.Text.Json` version only for the approved first implementation.
- It has no ASP.NET Core, MVC, authentication, `IHttpClientFactory`, DI, retry, resilience, logging, telemetry or consumer-specific proxy dependency.
- It interprets HTTP outcomes; it never reverses server-side `Nestgrid.Response.Http` mappings.
- Payload mode is explicit: `FullResult` or `ValueOnly`.
- Wire DTOs remain internal and core `Result`, `Result<T>` and `ResultMessage` are not deserialised directly.
- Results are constructed through the existing public `Results` and `ResultMessages` factories.
- Caller-owned `HttpResponseMessage` and `HttpClient` lifetimes remain caller-owned.
- Transport and cancellation exceptions remain standard exceptions; malformed, mismatched and unmapped responses use the safe protocol exception.
- OpenAPI, `ProblemDetails`, authentication, generic HTTP infrastructure and new server adapters remain out of scope.

### Existing review conditions

IR-011 through IR-015 remain open 1.0 API-stability findings. This implementation must not silently resolve or worsen them. The client package will have an explicit additive API inventory and contract tests, while decisions about existing `Result` extensibility, `NoContent<T>` callback semantics, existing server mappings and the public shared mapper invariant remain owned by the appropriate Architecture/Product/Sponsor decision path.

## Scope

### Included

1. Add the sixth client package and its responsibility-mirroring test project.
2. Add the approved public payload mode, immutable client options, stateless reader and safe protocol exception.
3. Add additive `HttpClient` convenience methods only where they are thin, explicit wrappers over the reader and response ownership is documented and tested.
4. Implement the ADR-011 exact status mapping and immutable custom mapping policy.
5. Implement the ADR-010 FullResult and ValueOnly wire rules for generic and non-generic operations.
6. Preserve structured message fields and existing core factory semantics.
7. Add fake-handler, direct-reader, fixture, malformed-input, status-matrix and lifetime tests.
8. Add neutral reusable proving samples/tests for the licence-service and Portal-to-Finance scenarios without embedding consumer-specific infrastructure.
9. Add package README, root documentation, solution visibility, compatibility/API inventory and implementation evidence.
10. Validate package contents, dependencies, target framework, generated metadata, consumer restore/build and security-sensitive exception content.

### Excluded

- Changes to the existing five packages' public behaviour.
- Changes to server-side HTTP mappings or reverse mapping between server and client.
- Public wire DTOs or direct core-type deserialisation.
- Core constructor, status or `InternalsVisibleTo` changes.
- Mandatory DI registration, handler composition or transport policy.
- Authentication, retries, resilience, logging, telemetry or endpoint discovery.
- Newtonsoft.Json support in the client package unless a new ADR authorises it.
- OpenAPI, `ProblemDetails`, persistence, hosting and unrelated validation work.
- Protected publication, package release approval and Release-stage decisions.

## Solution structure and technology baseline

| Area | Planned responsibility | Baseline |
| --- | --- | --- |
| `src/Nestgrid.Response.Http.Client` | HTTP response interpretation and result construction | `netstandard2.0`; core + central `System.Text.Json` |
| `tests/Nestgrid.Response.Http.Client.Tests` | Reader, policy, fixture and fake-handler tests | `net8.0`; existing test toolchain |
| `samples` | Neutral proving scenarios and consumer composition guidance | Existing sample conventions; no consumer-specific proxy package |
| `docs/artefacts/03 Implementation` | This plan, final report and evidence | Solution-visible artefacts |
| `Nestgrid.Response.sln` | IDE visibility for project, tests and relevant artefacts | Existing solution grouping |

The existing `Directory.Packages.props` remains the single version authority. Project files retain package ownership through `PackageReference Include` declarations. No dependency upgrade is implied by this plan.

## Planned public contract

The implementation will use the approved contract direction, subject to source-level review against the ADRs before commit:

```csharp
public enum NestgridResponsePayloadMode
{
    FullResult,
    ValueOnly
}

public sealed class NestgridResponseClientOptions
{
    public NestgridResponsePayloadMode PayloadMode { get; }
    public JsonSerializerOptions SerializerOptions { get; }
    public IReadOnlyDictionary<int, ResultStatus> StatusMappings { get; }
}

public sealed class NestgridResponseReader
{
    public Task<Result> ReadAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
    public Task<Result<T>> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default);
}
```

`NestgridResponseProtocolException` will expose only safe protocol context such as observed status and declared mode. It will not expose raw bodies, credentials or sensitive headers. Any convenience overloads must remain additive and must not turn the package into a generic HTTP-client abstraction.

## Implementation approach

1. Establish package/project identity, solution visibility and package metadata without changing existing package identities.
2. Define internal wire DTOs and a deterministic message conversion bridge using existing factories.
3. Implement immutable options and exact client mapping policy, including custom mapping isolation.
4. Implement the stateless reader in HTTP-status-first order.
5. Add thin `HttpClient` convenience methods, if required by the approved contract, with explicit response ownership.
6. Add fixtures and tests in small responsibility-focused blocks.
7. Add neutral proving scenarios and consumer handler-composition guidance.
8. Inspect build output, `.nuspec` dependency metadata, package contents and consumer restore/build.
9. Complete adversarial Engineering Assurance, update this plan with deviations and produce the Implementation Report.

## Test and evidence matrix

| Evidence | Required proof |
| --- | --- |
| Wire conversion | Internal DTO validation, message/code/property/severity preservation and factory construction |
| Payload modes | FullResult and ValueOnly, generic and non-generic, success and failure |
| Body rules | 204 typed/untyped, empty non-generic success, missing generic value, malformed and invalid envelopes |
| Status policy | 200, 201, 202, 204, 400, 401, 403, 404, 409, 422, 5xx, custom mappings, 3xx/unmapped |
| Error separation | Transport and cancellation propagation; protocol exception safety |
| Ownership | Caller response remains usable/not disposed; convenience ownership documented and tested |
| Integration | Fake `HttpMessageHandler` with normal `HttpClient` composition |
| Compatibility | Existing five-package regression suite and unchanged package metadata |
| Fixtures | Golden JSON cross-check against ASP.NET Core and MVC sample output |
| Consumer proof | Reusable licence-service and Portal-to-Finance scenarios without product-specific proxy infrastructure |
| Package proof | Zero-warning build, package content, target framework, dependency graph and local consumer installation |
| API evidence | Public API inventory and deliberate additive baseline for the sixth package |

## Documentation and IDE work

- Add the client project and client test project to `Nestgrid.Response.sln` under `src` and `tests`.
- Add the new package and tests to the appropriate solution-visible documentation groups.
- Add the package README and consumer guidance for explicit payload modes, mappings, authentication/handlers and response lifetime.
- Keep ADR-009 through ADR-011, this plan, the final report and supporting evidence visible in the IDE.
- Do not alter the historical v0.7.0 Release Report to imply that the sixth package was published.

## Risks and mitigations

| Risk | Impact | Mitigation |
| --- | --- | --- |
| Wire contract diverges from server samples | Consumers receive protocol failures or lose messages | Golden fixtures and cross-checks against both existing adapters |
| Client mappings are treated as inverse server mappings | Incorrect client semantics, especially 409/422 | Separate policy implementation and non-reversibility tests |
| Reader owns caller resources | Responses or transports are disposed unexpectedly | Explicit ownership contract and lifetime tests |
| Protocol failures disclose bodies or secrets | Sensitive data reaches logs or callers | Safe exception properties/messages and adversarial tests |
| Package gains generic transport responsibilities | Dependency and support scope expand | Package/dependency inspection and architecture conformance review |
| Public API is difficult to revise after 1.0 | Compatibility burden | API inventory, additive-only design and IR-012 compatibility evidence |
| Empty or malformed bodies become fabricated results | Boundary defects are hidden | Strict generic/body rules and protocol-failure tests |
| Serializer assumptions break netstandard2.0 consumers | Restore/runtime incompatibility | Central version policy, target-matrix build and consumer installation proof |

## Open questions and escalation triggers

- Confirm whether the thin convenience methods are needed in the final public API or whether direct reader usage is sufficient; do not add a generic client abstraction.
- Confirm the exact fixture casing/serializer policy against existing server samples during implementation; escalate if the approved wire contract cannot be represented with the approved serializer baseline.
- Escalate any need for Newtonsoft.Json, a new shared wire package, target-framework change, MVC-boundary change, core construction change or public status/mapping change.
- Escalate if either proving scenario requires consumer-specific proxy or transport infrastructure.
- Treat any unresolved IR-011, IR-013, IR-014 or IR-015 change to existing public semantics as a separate Architecture/Product/Sponsor decision, not an implementation convenience.

## Definition of Done

Engineering is complete when:

- ADR-009, ADR-010 and ADR-011 are implemented without unapproved deviation;
- the sixth package and tests build with zero warnings for the approved target;
- all required reader, mapping, wire, error, ownership, integration and proving tests pass;
- existing five-package tests and package metadata remain compatible;
- package contents and dependency metadata are inspected and recorded;
- consumer installation/build evidence is retained;
- documentation and solution/IDE visibility are complete;
- the API inventory and deviations are recorded;
- Engineering Assurance finds no unresolved implementation defect within scope;
- the Implementation Report records limitations, risks and outstanding downstream work; and
- the evidence is handed to Quality and Security, with Platform/Release decisions explicitly deferred.

## Recommend checkpoint

**Recommendation: approve this plan for implementation, subject to the stated conditions.**

The plan is implementation-ready within the approved Architecture boundary. Approval of this plan authorises the Engineering implementation tasks described above only. It does not approve a release, protected publication, package version, or any change reserved to Architecture, Product, Sponsor, Quality, Security, Platform or Release.

## Execution outcome

The plan was approved and executed. Engineering completed the package, reader, policy, convenience API, tests, proving sample, documentation, solution visibility and package inspection described in this plan. The final implementation report records 319 passing solution tests, package metadata evidence, known limitations and downstream conditions.

Engineering Assurance is **Assured with conditions**. The evidence is handed to Quality and Security for validation. Protected publication, final provenance and Release-stage decisions remain explicitly deferred to Platform, Release and the Project Sponsor.
