# Security Assessment

```yaml
title: Nestgrid.Response v0.7.0 Security Assessment
version: 1.4
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
  - ../../decisions/ADR-007-Minimum-Compatible-Dependency-Policy.md
  - ../../decisions/ADR-008-Safe-Exception-Result-Conversion.md
related_work_items:
  - SEC-001
  - SEC-002
  - SEC-003
  - SEC-004
  - SEC-005
  - SEC-006
  - Q-007
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
  - Security Feedback - Publication and Dependency Controls.md
```

## Scope

This assessment reviews the approved Nestgrid.Response v0.7.0 Security stage across the five NuGet packages, source code, tests, package metadata, consumer documentation, GitHub Actions workflows and the approved Discovery, Architecture, Engineering, Quality, Platform and Independent Review artefacts.

The assessment is a Security re-review of the current candidate. It does not approve risk on behalf of another role or approve release.

## Summary

Nestgrid.Response is a small, stateless library. It has no authentication, authorisation, persistence, runtime service identity, runtime secrets or externally hosted trust boundary. Those absences are appropriate to the approved product scope; consumers remain responsible for application access control, exception handling, logging and disclosure policy.

The prior exception-disclosure concern has been mitigated through the approved ADR-008 design and Engineering implementation: normal exception conversion now returns a generic message, while diagnostic conversion requires explicitly named methods and trusted-output handling. First-party guidance and tests now document the boundary.

GitHub-side controls have been configured: protected release tags, a protected `nuget` publishing environment tied to NuGet Trusted Publishing, and Dependabot alerts/security updates. The current workflow confirms immutable action SHAs and publication through `environment: nuget`; SEC-002 is therefore complete subject to retained protected-environment execution evidence. The approved minimum-compatible dependency policy is recorded. Candidate A has since been implemented with the approved exact pins, and Engineering reports no vulnerable packages in the evaluated source, test and sample graphs. SEC-006 remains open because authoritative fresh MVC package metadata and supported MVC package-consumer evidence are still outstanding; the current Quality report has not yet recorded the post-remediation reconciliation.

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

The opt-in validation conversion is appropriately explicit. Current guidance distinguishes client-safe, diagnostic and consumer-controlled output. Exception-derived messages require a separate safe-public-message pattern, and diagnostic methods remain appropriate only for trusted workflows.

## Input and Output Handling

The library validates null arguments and preserves immutable result snapshots. It does not interpret message text as markup, SQL, commands or expressions, so no direct injection sink was identified in the reviewed code.

The main output-handling risk is disclosure rather than injection. The normal `Results.Error(Exception)` path now returns `An unexpected error occurred.` without exception-derived details. Explicit `ErrorWithDiagnosticDetails` methods still emit exception messages and type names and must remain within trusted diagnostic workflows. Validation messages and property names are emitted as supplied. JSON encoding is delegated to the consuming framework and does not provide a policy decision about whether the content is safe to disclose.

## Secrets and Configuration

No runtime secrets or environment-specific application configuration are included. `NestgridResponseOptions` is consumer-owned configuration and contains status mappings and response-shape settings only.

NuGet Trusted Publishing identity and repository permissions are release infrastructure secrets. They must remain managed by GitHub and NuGet controls and must not be copied into source, local configuration or documentation.

## Operational Security

The Platform artefacts provide a coherent package publication, rollback and evidence-retention model. Protected release tags, the protected `nuget` environment, NuGet Trusted Publishing, Dependabot alerts/security updates, immutable action SHAs and `environment: nuget` wiring are now recorded. Remaining conditions are a successful protected-environment publication run, CI reproduction, package provenance retention, post-remediation Quality reconciliation and SEC-006 package-closure evidence.

The package is stateless and has no runtime permissions, service identity, installation script or uninstall operation. Consumers should pin or otherwise control package versions through their own dependency-management policy.

## Dependency Review

The approved Architecture direction intentionally favours the lowest compatible dependency versions that do not carry known vulnerabilities, to preserve the pool of compatible consumers. Security accepts that as the governing policy and does not recommend upgrading merely to the newest major version.

ADR-007 now records the policy and review trigger. The direct package baselines include `System.Text.Json` 4.6.0, `System.ComponentModel.Annotations` 4.1.0 and `Microsoft.AspNetCore.Mvc.Core` 2.1.38. The pre-remediation Quality audit reported Critical `System.Text.Encodings.Web` 4.6.0/4.5.0, High `Microsoft.AspNetCore.Http` 2.1.1 and High `Newtonsoft.Json` 9.0.1 advisories in supported/package-consumer graphs. Candidate A now resolves the evaluated graphs to `System.Text.Encodings.Web 4.7.2`, `Microsoft.AspNetCore.Http 2.1.22` and `Newtonsoft.Json 13.0.1` without changing the MVC `2.1.38` parent boundary. The approved minimum-compatible policy does not justify retaining a known vulnerable graph without an explicit Architecture/Security/Sponsor decision and review date; no such exception is recorded.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-001 | P1 | Exception conversion previously exposed raw exception message and type through a normal result path. | Consumer APIs could disclose internal diagnostics. | **Resolved.** ADR-008 is accepted; normal overloads now return a generic message, diagnostic methods are explicit, guidance is updated and tests cover safe/diagnostic paths. |
| SEC-002 | P1 | Publication workflow previously referenced mutable action tags and lacked protected-environment wiring. | A compromised action reference could alter the build or publish attacker-controlled packages. | **Resolved in repository configuration.** All reviewed workflow actions use immutable SHAs and publication uses `environment: nuget`; retain a successful protected-environment run and package provenance before release. |
| SEC-003 | P2 | Minimum-compatible dependency policy previously lacked recorded governance and evidence. | Dependency posture was not auditable at release time. | **Governance resolved; evidence remains open.** ADR-007 records the policy. Security/Platform must retain advisory, restore and package-provenance evidence. |
| SEC-004 | P2 | Client-safe, diagnostic and consumer-controlled output boundaries were previously insufficiently distinct. | Consumers could expose validation or diagnostic details to untrusted callers. | **Resolved for the current candidate.** Architecture disposition, package guidance, sample guidance and implementation assurance now distinguish the three output categories. |
| SEC-005 | P2 | Consumers can assign security-sensitive semantic statuses to arbitrary HTTP status codes. | Misconfiguration could weaken authentication, authorisation, caching or client error handling. | **Resolved for the current candidate.** Consumer responsibility is documented and normative mappings are covered by ASP.NET Core and MVC regression tests. |
| SEC-006 | P1 | The pre-remediation supported/package-consumer graphs contained Critical/High advisories. Candidate A has been implemented and its evaluated graphs report no vulnerable packages, but final MVC package metadata and supported MVC consumer evidence are not yet retained. | Without final published-closure and consumer evidence, Security cannot confirm that the release artefact is free of the reported vulnerable paths. | **Open evidence blocker.** Reconcile the post-remediation Quality result, retain authoritative MVC `.nuspec`/package closure and supported MVC package-consumer evidence, then perform final Security closure review. Do not release or accept residual vulnerability risk by implication. |

## Accepted Risks

| Risk | Owner | Reason | Review Date |
| --- | --- | --- | --- |
| None | — | No role with authority to accept SEC-003 evidence or SEC-006 dependency vulnerability risk has recorded acceptance. | Before Release approval |

## Residual Risks Requiring Ownership

| Risk | Owner | Mitigation | Release position |
| --- | --- | --- | --- |
| A consumer may return explicit diagnostic exception details, validation properties/messages or custom status mappings to an untrusted caller. | Consumer application owner | Use safe exception conversion by default, keep diagnostic methods inside trusted workflows, review validation output and preserve normative status mappings. | Documented residual consumer risk; not an accepted SEC-006 or release waiver. |

## Recommendation

Security re-review is **approved with conditions**.

Platform review may proceed using this assessment and the existing Platform artefacts. SEC-001, SEC-002, SEC-004 and SEC-005 are resolved for the current candidate, subject to the evidence conditions stated above. Candidate A implementation is complete; its implementation approval is recorded separately and is not a closure decision. Release review may proceed as a conditional evidence/disposition review, but the product is **not recommended for final release approval** until SEC-003 evidence, the post-remediation Quality reconciliation, SEC-006 closure evidence, protected-environment publication evidence and the Release Report’s final dispositions/package provenance are complete.
