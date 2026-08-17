# Security Assessment

```yaml
title: Nestgrid.Response v0.7.0 Security Assessment
version: 1.3
status: Approved
owner: Security Engineer
contributors:
  - Morgan profile
produced_by: Security Engineer
consumed_by: Solution Architect, Software Engineer, Platform Engineer, Project Sponsor
date: 2026-08-17
supersedes:
related_decisions:
  - ../../decisions/ADR-006-AspNetCore-And-Mvc-Package-Separation.md
  - ../../decisions/TDR-001-Validation-Result-Conversion-Detail.md
related_work_items:
  - SEC-001
  - SEC-002
  - SEC-003
  - SEC-004
  - SEC-005
  - SEC-006
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../01 Discovery/Product Brief.md
  - ../02 Architecture/Architecture Pack.md
  - ../03 Implementation/Implementation Report.md
  - ../04 Quality/Release Readiness Report.md
  - ../06 Platform/Operational Readiness Review.md
  - ../../reviews/Nestgrid.Response Independent Review.md
  - ../03 Implementation/SEC-006 Dependency Path Matrix.md
  - Security Feedback - SEC-006 Candidate A Approval.md
```

## Scope

This assessment reviews the approved Nestgrid.Response v0.7.0 Security stage across the five NuGet packages, source code, tests, package metadata, consumer documentation, GitHub Actions workflows and the approved Discovery, Architecture, Engineering, Quality, Platform and Independent Review artefacts.

The assessment is a Security re-review of the current candidate. It does not approve risk on behalf of another role or approve release.

## Summary

Nestgrid.Response is a small, stateless library. It has no authentication, authorisation, persistence, runtime service identity, runtime secrets or externally hosted trust boundary. Those absences are appropriate to the approved product scope; consumers remain responsible for application access control, exception handling, logging and disclosure policy.

The prior exception-disclosure concern has been mitigated through the approved ADR-008 design and Engineering implementation: normal exception conversion now returns a generic message, while diagnostic conversion requires explicitly named methods and trusted-output handling. First-party guidance and tests now document the boundary.

GitHub-side controls have been configured: protected release tags, a protected `nuget` publishing environment tied to NuGet Trusted Publishing, and Dependabot alerts/security updates. The current workflow confirms immutable action SHAs and publication through `environment: nuget`; SEC-002 is therefore complete subject to retained protected-environment execution evidence. The approved minimum-compatible dependency policy is recorded, but current Quality evidence reports unresolved Critical/High advisories in supported package graphs.

## Threat Model

### Assets

- Consumer application response data.
- Validation messages, property names and machine-readable codes.
- Result values supplied by consumers.
- Exception messages and exception type names.
- NuGet packages, symbols and publication credentials.
- Package integrity, version provenance and release evidence.

### Threat actors

- An unauthorised external caller attempting to learn internal details through an API response.
- A malicious or compromised dependency or CI action attempting to alter packages or obtain publication authority.
- A maintainer or consumer misconfiguring result mappings or returning internal diagnostics to users.

### Trust boundaries

- Consumer application code to the library.
- Library result objects to HTTP adapter serialisation.
- Repository source and GitHub Actions to NuGet publication.
- Package output to downstream consumer restore and execution.

### Relevant abuse scenarios

1. A consumer catches an exception, calls `Results.Error(exception)` and returns the result through an HTTP adapter. The exception message discloses internal details.
2. A validation result contains sensitive property or message content and is returned using the opt-in member-aware conversion.
3. A mutable GitHub Action reference is changed or compromised in a workflow with `id-token: write`, allowing altered package publication. Protected tags, the protected `nuget` environment, NuGet Trusted Publishing and immutable action pinning reduce this risk; protected-environment execution evidence remains a release condition.
4. A consumer maps an access-control status to an inappropriate HTTP status, weakening client or cache handling.

## Authentication Review

Not applicable to the library runtime. The product does not authenticate callers and correctly leaves authentication to consuming applications. The publication workflow uses NuGet Trusted Publishing through GitHub OIDC; repository tag protection and release-environment approval remain operational controls.

## Authorisation Review

Not applicable to the library runtime. The result statuses `Unauthorized` and `Forbidden` are semantic outcomes only; the library does not enforce access decisions. Consumers must perform authorisation before constructing or returning those results.

## Data Protection Review

Result values and messages are in-memory and not retained by the library. HTTP adapters serialise the result envelope or successful value according to consumer-selected options. `Property`, `Message`, `Code` and `Value` can contain consumer-controlled or domain-sensitive data.

The opt-in validation conversion is appropriately explicit, but the documentation should make safe client-facing versus diagnostic content clearer. Exception-derived messages require a separate safe-public-message pattern.

## Input and Output Handling

The library validates null arguments and preserves immutable result snapshots. It does not interpret message text as markup, SQL, commands or expressions, so no direct injection sink was identified in the reviewed code.

The main output-handling risk is disclosure rather than injection. The normal `Results.Error(Exception)` path now returns `An unexpected error occurred.` without exception-derived details. Explicit `ErrorWithDiagnosticDetails` methods still emit exception messages and type names and must remain within trusted diagnostic workflows. Validation messages and property names are emitted as supplied. JSON encoding is delegated to the consuming framework and does not provide a policy decision about whether the content is safe to disclose.

## Secrets and Configuration

No runtime secrets or environment-specific application configuration are included. `NestgridResponseOptions` is consumer-owned configuration and contains status mappings and response-shape settings only.

NuGet Trusted Publishing identity and repository permissions are release infrastructure secrets. They must remain managed by GitHub and NuGet controls and must not be copied into source, local configuration or documentation.

## Operational Security

The Platform artefacts provide a coherent package publication, rollback and evidence-retention model. Protected release tags, the protected `nuget` environment, NuGet Trusted Publishing, Dependabot alerts/security updates, immutable action SHAs and `environment: nuget` wiring are now recorded. Remaining conditions are a successful protected-environment publication run, CI reproduction, package provenance retention and dependency advisory disposition.

The package is stateless and has no runtime permissions, service identity, installation script or uninstall operation. Consumers should pin or otherwise control package versions through their own dependency-management policy.

## Dependency Review

The approved Architecture direction intentionally favours the lowest compatible dependency versions that do not carry known vulnerabilities, to preserve the pool of compatible consumers. Security accepts that as the governing policy and does not recommend upgrading merely to the newest major version.

ADR-007 now records the policy and review trigger. The direct package baselines include `System.Text.Json` 4.6.0, `System.ComponentModel.Annotations` 4.1.0 and `Microsoft.AspNetCore.Mvc.Core` 2.1.38. Quality’s current candidate audit reports Critical `System.Text.Encodings.Web` 4.6.0/4.5.0, High `Microsoft.AspNetCore.Http` 2.1.1 and High `Newtonsoft.Json` 9.0.1 advisories in supported/package-consumer graphs. The approved minimum-compatible policy does not justify retaining a known vulnerable graph without an explicit Architecture/Security/Sponsor decision and review date.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-001 | P1 | Exception conversion previously exposed raw exception message and type through a normal result path. | Consumer APIs could disclose internal diagnostics. | **Resolved.** ADR-008 is accepted; normal overloads now return a generic message, diagnostic methods are explicit, guidance is updated and tests cover safe/diagnostic paths. |
| SEC-002 | P1 | Publication workflow previously referenced mutable action tags and lacked protected-environment wiring. | A compromised action reference could alter the build or publish attacker-controlled packages. | **Resolved in repository configuration.** All reviewed workflow actions use immutable SHAs and publication uses `environment: nuget`; retain a successful protected-environment run and package provenance before release. |
| SEC-003 | P2 | Minimum-compatible dependency policy previously lacked recorded governance and evidence. | Dependency posture was not auditable at release time. | **Governance resolved; evidence remains open.** ADR-007 records the policy. Security/Platform must retain advisory, restore and package-provenance evidence. |
| SEC-004 | P2 | Client-safe, diagnostic and consumer-controlled output boundaries were previously insufficiently distinct. | Consumers could expose validation or diagnostic details to untrusted callers. | **Resolved for the current candidate.** Architecture disposition, package guidance, sample guidance and implementation assurance now distinguish the three output categories. |
| SEC-005 | P2 | Consumers can assign security-sensitive semantic statuses to arbitrary HTTP status codes. | Misconfiguration could weaken authentication, authorisation, caching or client error handling. | **Resolved for the current candidate.** Consumer responsibility is documented and normative mappings are covered by ASP.NET Core and MVC regression tests. |
| SEC-006 | P1 | Current supported/package-consumer dependency graphs contain unresolved Critical/High advisories reported by Quality. | Publishing the current graph may expose consumers to known vulnerable dependencies. | **Implementation approved with conditions.** Security approves Candidate A’s exact lowest-compatible patched pins while preserving the MVC `2.1.38` boundary. Final package metadata, consumer, restore and advisory evidence remain required; do not upgrade solely for recency or treat a residual vulnerable path as accepted without an authorised decision. |

## Accepted Risks

| Risk | Owner | Reason | Review Date |
| --- | --- | --- | --- |
| None | — | No role with authority to accept SEC-003 evidence or SEC-006 dependency vulnerability risk has recorded acceptance. SEC-001, SEC-002, SEC-004 and SEC-005 are resolved for this candidate; protected-environment execution evidence remains outstanding. | Before Release approval |

## Recommendation

Security re-review is **approved with conditions**.

Platform review may proceed using this assessment and the existing Platform artefacts. SEC-001, SEC-002, SEC-004 and SEC-005 are resolved for the current candidate, subject to the evidence conditions stated above. Security approves Mason to implement SEC-006 Candidate A under the dedicated Security Feedback conditions. Release review may proceed as a conditional evidence review, but the product is **not recommended for final release approval** until SEC-003 evidence and SEC-006 closure evidence are complete, protected-environment publication evidence is retained, and the Release Report records final dispositions and package provenance.
