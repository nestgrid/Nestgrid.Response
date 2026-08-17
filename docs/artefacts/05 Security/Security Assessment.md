# Security Assessment

```yaml
title: Nestgrid.Response v0.7.0 Security Assessment
version: 1.1
status: Complete with conditions
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
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - ../01 Discovery/Product Brief.md
  - ../02 Architecture/Architecture Pack.md
  - ../03 Implementation/Implementation Report.md
  - ../04 Quality/Release Readiness Report.md
  - ../06 Platform/Operational Readiness Review.md
  - ../../reviews/Nestgrid.Response Independent Review.md
```

## Scope

This assessment reviews the approved Nestgrid.Response v0.7.0 Security stage across the five NuGet packages, source code, tests, package metadata, consumer documentation, GitHub Actions workflows and the approved Discovery, Architecture, Engineering, Quality, Platform and Independent Review artefacts.

The assessment stops at Recommend. It does not modify implementation, approve risk on behalf of another role or approve release.

## Summary

Nestgrid.Response is a small, stateless library. It has no authentication, authorisation, persistence, runtime service identity, runtime secrets or externally hosted trust boundary. Those absences are appropriate to the approved product scope; consumers remain responsible for application access control, exception handling, logging and disclosure policy.

The principal security concern is that the public exception conversion API and its README example make it easy to expose raw exception details through an HTTP response. This is partly a consumer misuse risk, but the library contract and first-party guidance materially enable the unsafe outcome. It requires Architecture and Engineering disposition rather than being assigned solely to consumers.

GitHub-side controls have now been configured: protected release tags, a protected `nuget` publishing environment tied to NuGet Trusted Publishing, and Dependabot alerts/security updates. The remaining SEC-002 action is Platform-owned workflow hardening: pin third-party actions to immutable commit SHAs and wire publication through the `nuget` environment. Dependency policy is intentional and documented as minimum-compatible versions, but current advisory evidence and a reproducible dependency baseline are not retained.

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
3. A mutable GitHub Action reference is changed or compromised in a workflow with `id-token: write`, allowing altered package publication. Protected tags, the protected `nuget` environment and NuGet Trusted Publishing reduce this risk; immutable action pinning and environment wiring remain outstanding.
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

The main output-handling risk is disclosure rather than injection. `Results.Error(Exception)` copies `Exception.Message` and the exception type name into serialisable output. Validation messages and property names are also emitted as supplied. JSON encoding is delegated to the consuming framework and does not provide a policy decision about whether the content is safe to disclose.

## Secrets and Configuration

No runtime secrets or environment-specific application configuration are included. `NestgridResponseOptions` is consumer-owned configuration and contains status mappings and response-shape settings only.

NuGet Trusted Publishing identity and repository permissions are release infrastructure secrets. They must remain managed by GitHub and NuGet controls and must not be copied into source, local configuration or documentation.

## Operational Security

The Platform artefacts provide a coherent package publication, rollback and evidence-retention model. Protected release tags, the protected `nuget` environment, NuGet Trusted Publishing and Dependabot alerts/security updates are now configured. The remaining security conditions are immutable action references, wiring publication through `nuget`, CI reproduction and retention of immutable package provenance.

The package is stateless and has no runtime permissions, service identity, installation script or uninstall operation. Consumers should pin or otherwise control package versions through their own dependency-management policy.

## Dependency Review

The approved Architecture direction intentionally favours the lowest compatible dependency versions that do not carry known vulnerabilities, to preserve the pool of compatible consumers. Security accepts that as the governing policy and does not recommend upgrading merely to the newest major version.

However, the repository does not retain a package lock file or current advisory/restore evidence. The direct package baselines include `System.Text.Json` 4.6.0, `System.ComponentModel.Annotations` 4.1.0 and `Microsoft.AspNetCore.Mvc.Core` 2.1.38. Their security posture must be checked against the approved minimum-version policy before publication; no dependency should be upgraded merely to reach a current major version when the existing version remains the lowest compatible version without a known vulnerability.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-001 | P1 | Exception conversion exposes raw exception message and type through a normal result path. | Consumer APIs may disclose internal paths, connection details, identifiers, vendor details or other sensitive diagnostics. | Architecture and Engineering to define a safe exception-to-result contract, revise first-party guidance and add disclosure regression tests. See [Security Feedback - Exception and Output Disclosure](Security%20Feedback%20-%20Exception%20and%20Output%20Disclosure.md). |
| SEC-002 | P1 | Publication workflow still references third-party actions by mutable tags and has not yet been wired through the protected `nuget` environment. GitHub-side protection and NuGet policy are configured. | A compromised action reference could alter the build or publish attacker-controlled packages. | Platform to pin actions to immutable SHAs and wire the publish job through `environment: nuget`; retain the protected-tag, environment and NuGet policy evidence. See [Security Feedback - Publication and Dependency Controls](Security%20Feedback%20-%20Publication%20and%20Dependency%20Controls.md). |
| SEC-003 | P2 | Minimum-compatible dependency policy is not accompanied by retained current advisory and restore evidence. | A vulnerable dependency could be published without a visible exception or upgrade decision. | Solution Architecture to record the policy explicitly; Security/Platform to retain advisory and restore evidence. Do not upgrade merely to reach current versions. |
| SEC-004 | P2 | Consumer disclosure responsibility is documented, but safe-public versus diagnostic output is not sufficiently distinguished. | Consumers may return validation details, property names or internal messages to untrusted callers or logs. | Architecture/Engineering to disposition the concern; Software Engineering to strengthen package guidance and add representative disclosure-focused tests/examples. |
| SEC-005 | P2 | Consumers can assign security-sensitive semantic statuses to arbitrary HTTP status codes. | Misconfiguration could weaken authentication, authorisation, caching or client error handling. | Architecture/Engineering to disposition the concern, document secure mapping expectations and test the default and security-sensitive mappings. |

## Accepted Risks

| Risk | Owner | Reason | Review Date |
| --- | --- | --- | --- |
| None | — | No role with authority to accept these risks has recorded acceptance. SEC-001 and SEC-002 remain release-blocking conditions; SEC-003 through SEC-005 remain open follow-up risks. Configured GitHub controls reduce SEC-002 exposure but do not complete the Platform action. | Before Release approval |

## Recommendation

Security is **complete with conditions**.

Platform review may proceed using this assessment and the existing Platform artefacts. Release review may proceed as a conditional evidence review, but the product is **not recommended for final release approval** until SEC-001 and SEC-002 are mitigated or explicitly accepted by the appropriate authority, SEC-003 receives current advisory/restore evidence, Architecture/Engineering disposition SEC-004 and SEC-005, and the Release Report records the final dispositions and immutable publication evidence.
