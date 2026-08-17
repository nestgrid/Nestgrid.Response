# Security Feedback

```yaml
title: Nestgrid.Response v0.7.0 Security Feedback - Publication and Dependency Controls
version: 1.3
status: In Review
owner: Platform Engineer
contributors:
  - Security Engineer
produced_by: Security Engineer
consumed_by: Platform Engineer, Solution Architect, Quality Engineer, Project Sponsor
date: 2026-08-17
supersedes:
related_decisions:
related_work_items:
  - SEC-002
  - SEC-003
  - SEC-006
related_repositories:
  - Nestgrid.Response
related_artefacts:
  - Security Assessment.md
  - ../06 Platform/Deployment Guide.md
  - ../06 Platform/Operational Readiness Review.md
  - ../03 Implementation/SEC-006 Dependency Path Matrix.md
  - Security Feedback - SEC-006 Candidate A Approval.md
```

## Context

The Platform publication workflow builds and publishes all five packages through GitHub Actions and NuGet Trusted Publishing. The approved Architecture direction also favours the lowest compatible dependency versions without known vulnerabilities to preserve consumer compatibility.

## Feedback Summary

The publication model is sound in principle. GitHub-side controls are configured, and the current workflows use immutable action SHAs and publication through the protected `nuget` environment. The remaining Platform evidence is a successful protected-environment execution with retained package provenance. ADR-007 records the minimum-compatible dependency policy. Security has separately approved Candidate A for SEC-006 implementation; its final package-closure and consumer evidence remain release conditions.

## Findings

| ID | Severity | Finding | Impact | Recommendation |
| --- | --- | --- | --- | --- |
| SEC-002 | P1 | Publication workflow hardening was previously incomplete. | Action compromise or retagging could alter packages or publish through trusted identity. | **Resolved in repository configuration.** Current workflows pin third-party actions to immutable SHAs and the publish job uses `environment: nuget`; Platform/Release must retain successful execution and package provenance evidence. |
| SEC-003 | P2 | Advisory, restore and package-provenance evidence is not fully retained with the candidate. | Dependency posture is not auditable at release time. | **Governance resolved; evidence open.** Apply ADR-007 and retain the evaluated graph, advisory result, restore result and provenance. Do not upgrade merely for recency. |
| SEC-006 | P1 | Quality reports Critical `System.Text.Encodings.Web` 4.6.0/4.5.0, High `Microsoft.AspNetCore.Http` 2.1.1 and High `Newtonsoft.Json` 9.0.1 advisories in supported/package-consumer graphs. | Known vulnerable dependencies may reach package consumers. | **Implementation direction approved.** Implement Candidate A under the dedicated Security Feedback conditions, then retain final closure evidence or obtain an authorised exception if the reviewed boundary cannot be preserved. |

## Blocking Issues

- SEC-006 must be remediated or explicitly accepted before final release approval. SEC-002 requires retained protected-environment execution evidence.

## Non-blocking Issues

- SEC-003 remains an evidence condition; the current Q-007/SEC-006 advisory result makes dependency disposition release-blocking.

## Questions

- Which protected-environment workflow run and package hashes will be retained as publication evidence?
- Does the final Candidate A evidence demonstrate that Q-007/SEC-006 is fully closed across published and supported consumer graphs?

## Recommendation

Security confirms the workflow hardening in the current repository and requests Platform/Release retain the protected-environment execution evidence. Security approves Candidate A implementation for Q-007/SEC-006; Architecture, Engineering and Security must confirm final closure evidence under ADR-007 before final release approval.
