# Roadmap

> Part of the **[Release Artefacts](README.md)**.

Nestgrid.Response is pre-1.0 and is currently focused on stability, documentation quality and developer experience.

## Purpose

The roadmap records likely direction without promising dates or speculative commitments.

## Current Package Set

- `Nestgrid.Response`
- `Nestgrid.Response.Http`
- `Nestgrid.Response.AspNetCore`
- `Nestgrid.Response.Mvc`
- `Nestgrid.Response.Extensions.Validation`

## Near-Term Focus

- Keep public APIs stable.
- Improve documentation and examples based on real usage.
- Maintain high unit-test and mutation-test confidence.
- Keep HTTP adapters consistent through shared mapping policy.
- Avoid broad abstractions unless repeated usage proves they are needed.

## Next Candidates

- OpenAPI documentation helpers or clearer endpoint metadata guidance.
- `ProblemDetails` mapping guidance for applications that prefer RFC 7807 payloads.
- Additional samples for validation-heavy application services.
- Review whether `Nestgrid.Response.AspNetCore` should expose optional endpoint metadata helpers.

## Candidates for v1.0

- API stability review.
- Documentation freeze for core semantics.
- Package compatibility review.
- Final decision on any pre-1.0 naming or shape adjustments.

## Under Consideration

- Additional adapters only where real application use cases justify them.
- Source generator investigation for documentation or endpoint metadata, if the benefit outweighs the added complexity.

## Navigation

**Artefacts**

- [Release Artefacts](README.md)

**Documentation**

- [Documentation index](../../README.md)

**Repository**

- [Nestgrid.Response](../../../README.md)
