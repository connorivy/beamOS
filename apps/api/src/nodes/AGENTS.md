# AGENTS.md

## Scope
- Applies to `apps/api/src/nodes`.
- Handles node CRUD and batch creation behavior.

## Local File Roles
- `node-contract-schemas.ts`: request/response validation contracts.
- `batch-create-node.ts`: bulk-create behavior and validation flow.
- `patch-node.ts`: patch/update semantics.
- `node-entity.ts` / `node-events.ts`: domain model and event definitions.

## Persistence Rules
- Node changes are persisted as domain events in `revision_changes`.
- Do not add direct persistence to a `nodes` table.
- `NodeEntity.create(...)` must emit a create event and repositories/handlers should persist from pulled domain events.

## Change Checklist
- Update request/response schemas when handler behavior changes.
- Keep entity/event payload behavior consistent with revision-change replay expectations.
- Add or update integration tests in `apps/api/test/integration/node-client.test.ts` when node API behavior changes.

## Client Impact
- Node API surface changes can alter generated OpenAPI client types.
- Regenerate via:
- repo root `bun run build` (or `bun run --cwd packages/openapi-client build`)
- `packages/openapi-client` root `bun run build`
