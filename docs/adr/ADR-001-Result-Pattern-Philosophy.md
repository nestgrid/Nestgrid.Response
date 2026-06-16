# ADR-001: Result Pattern Philosophy

## Status

Accepted

## Context

Nestgrid.Response is intended to replace and improve on lessons learnt from Knight.Response.

The library should provide a lightweight, explicit, and developer-friendly way for application and service layers to communicate operation outcomes to presentation layers.

It should make success, failure, validation, authorization, missing resources, and unexpected errors easy to represent without relying on exceptions for expected business outcomes.

Nestgrid.Response is not intended to be a full functional programming framework, workflow engine, mediator pipeline, or HTTP abstraction.

## Decision

Nestgrid.Response will be designed as an application/service result abstraction.

The core package will focus on:

- `Result`
- `Result<T>`
- Clear operation statuses
- Readable factory methods
- Validation-friendly outcomes
- Simple message handling
- Predictable presentation-layer mapping

The library will prioritize:

- Readability
- Explicit intent
- Immutability
- Low ceremony
- Minimal dependencies
- Clean integration with ASP.NET Core through separate packages

## Consequences

Application and service layers can return structured outcomes without depending on ASP.NET Core.

Presentation layers can convert results into API responses using adapter packages.

The library will avoid excessive functional abstractions, mutation chains, and complex nested error graphs unless a clear need emerges later.

## Principles

- Results represent expected operation outcomes.
- Exceptions represent unexpected system failures.
- Core must remain framework-agnostic.
- Public APIs must be simple and predictable.
- Result objects should be easy to serialise, inspect, test, and map.