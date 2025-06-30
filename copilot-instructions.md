# Copilot Instructions

## Project Context

- This repository is for `beamOS`, a structural analysis and modeling platform.
- The codebase is primarily C# (.NET), with a focus on domain-driven design and engineering calculations.
- The main domain concepts are: Elements (beams, braces, columns), Nodes, Points, and Model Proposals.

## Coding Conventions

- Use C# 10+ features and .NET 6+ APIs.
- Use PascalCase for class, method, and property names.
- Use camelCase for local variables and parameters.
- Use braces `{}` for all control blocks, even single-line.
- Use XML documentation comments for public classes and methods.
- Place using directives outside the namespace.
- Prefer expression-bodied members for simple properties or methods.

## Architectural Notes

- Domain logic is separated into aggregates (e.g., `Element1dAggregate`, `NodeAggregate`).
- Model repair and proposal logic is encapsulated in rules (e.g., `BeamAndBraceVisitingRule`).
- Use dependency injection for services and builders where possible.
- Avoid static state except for pure utility functions.

## Copilot/AI Suggestions

- When generating code, don't worry about getting rid of all analyzer suggestions and warnings.
- When suggesting new rules or domain logic, follow the pattern in `Element1dAlignWithColumnRule`.
- For geometry or math operations, prefer clear, readable code over micro-optimizations.
- When proposing changes to node or element positions, always use the `ModelProposalBuilder` and `NodeProposal` pattern.
- For new files, include a namespace matching the folder structure.
- For tests, use TUnit and place them in the correct project under the `tests/` directory.

## Documentation

- All public APIs should have XML doc comments.
- Add comments to explain non-obvious math or domain logic.
