# Security Artefacts

```yaml
title: Nestgrid.Response v0.7.0 Security Artefacts
version: 1.5
status: In Review
owner: Security Engineer
produced_by: Security Engineer
consumed_by: Solution Architect, Software Engineer, Platform Engineer, Project Sponsor
date: 2026-08-26
```

Security artefacts for the v0.7.0 EOS retrofit.

## Contents

- [Security Assessment](Security%20Assessment.md)
- [Security Feedback - Exception and Output Disclosure](Security%20Feedback%20-%20Exception%20and%20Output%20Disclosure.md)
- [Security Feedback - Publication and Dependency Controls](Security%20Feedback%20-%20Publication%20and%20Dependency%20Controls.md)
- [Security Feedback - SEC-006 Candidate A Approval](Security%20Feedback%20-%20SEC-006%20Candidate%20A%20Approval.md)
- [Security Feedback - HTTP Client Capability](Security%20Feedback%20-%20HTTP%20Client%20Capability.md)

The v0.8.0 Security re-assessment recommends conditional progression to Platform and Release review. SEC-001 through SEC-006 are resolved or dispositioned for the evaluated candidate. SEC-007 (unbounded response buffering) and SEC-008 (protocol-exception inner-detail disclosure) are open P2 security findings. No security risk is accepted; final publication is not recommended until these findings are resolved or explicitly authorised, and protected-CI provenance and Release evidence are complete.
