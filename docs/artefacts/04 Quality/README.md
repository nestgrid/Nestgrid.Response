# Quality Artefacts

```yaml
title: Nestgrid.Response v0.7.0 Quality Artefacts
version: 1.0
status: Complete with conditions
owner: Quality Engineer
produced_by: Quality Engineer
consumed_by: Security Engineer, Platform Engineer, Release Owner, Project Sponsor
date: 2026-08-17
```

Quality artefacts for the v0.7.0 EOS retrofit.

## Contents

- [Test Strategy](Test%20Strategy.md)
- [Release Readiness Report](Release%20Readiness%20Report.md)

Quality execution is complete for the post-Engineering handover candidate. Quality recommends proceeding to Security, Platform and Release review, but does not recommend release until the dependency advisory finding and downstream conditions are resolved or formally accepted by the authorised owners.

## v0.8.0 HTTP Client Capability

- [Test Strategy — HTTP Client Capability](Test%20Strategy%20-%20HTTP%20Client%20Capability.md)
- [Release Readiness Report — HTTP Client Capability](Release%20Readiness%20Report%20-%20HTTP%20Client%20Capability.md)

The v0.8.0 additive HTTP client candidate has 353 passing solution tests and 94.16% package-owned line coverage. Quality does not recommend publication yet because its dedicated mutation score is 80.99%, below the configured 90% break threshold; supported sample/consumer and downstream Security/Platform evidence also remain required.
