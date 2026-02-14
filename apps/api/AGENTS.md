# AGENTS.md

## Scope
- Applies to `apps/api`.
- Backend is Elysia-based and structured by domain folders under `src/`.

## Structure
- Domain modules live in `src/<domain>/` (schemas, mappers, repositories, handlers).
- Database schema and migration tooling live in `src/db/` and `drizzle/`.
- Integration tests live in `test/integration/`.

## Persistence Model
- Nodes, materials, section profiles, and element1ds are persisted as revision-change domain events.
- Persist those entities via `revision_changes` entries (`entity_type`, `op`, `payload`), not entity-specific tables.
- Read models/revisions by replaying ordered revision changes.

## Commands
- Dev server: `bun run --cwd apps/api dev`
- Build: `bun run --cwd apps/api build`
- Unit tests: `bun run --cwd apps/api test:unit`
- Integration tests: `bun run --cwd apps/api test:integration`

## API Contract + Client Workflow
- API contract changes should be validated with integration tests.
- If route or schema changes affect client types, regenerate the OpenAPI client.
- Client generation paths:
- From repo root: `bun run build` or `bun run --cwd packages/openapi-client build`
- From `packages/openapi-client`: `bun run build`

## Editing Guidance
- Keep request/response schema files and route handlers aligned.
- Prefer explicit validation in `*-request-schema.ts` and `*-response-schema.ts`.
- Avoid introducing implicit behavior across aggregate/repository boundaries.
