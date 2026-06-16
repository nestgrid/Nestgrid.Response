# Coding Standards

## General

- Prefer readability over clever abstractions.
- Prefer explicit code over magic.
- Keep public APIs small and predictable.
- Avoid unnecessary dependencies.
- Use immutable models wherever possible.

## Naming

- Use Microsoft naming conventions.
- Use US spelling in public APIs and documentation.
- Use descriptive method names.

## Results

- Results are immutable.
- Status is the primary outcome indicator.
- Factory methods should be preferred over constructors.
- Do not expose mutable collections.

## Testing

- Use xUnit v2.
- Use NSubstitute for mocking.
- Use Shouldly for assertions.
- Follow Arrange / Act / Assert structure.
- Create separate test files per feature where practical.

## Documentation

- Public APIs should have XML documentation.
- Architectural decisions should be captured in ADRs.
- Breaking changes require ADR review.
- Use consistent formatting and style across documentation.
- Ensure clarity and conciseness in documentation.
- Use headings and subheadings to organise content.
- Provide examples and code snippets where applicable.
- Keep documentation up-to-date with code changes.
- Use consistent terminology and avoid ambiguity.
- New public features should include documentation updates.
- Documentation should explain the reasoning behind decisions, not just the implementation.