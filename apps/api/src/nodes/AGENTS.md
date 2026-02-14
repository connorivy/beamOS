# AGENTS.md

## Scope
- Applies to `apps/api/src/nodes`.
- Handles node CRUD and batch creation behavior.

## Local File Roles
- `create-node-request-schema.ts` / `update-node-request-schema.ts` / `delete-node-request-schema.ts`: input validation.
- `node-response-schema.ts`: serialized output contract.
- `batch-create-node.ts`: bulk-create behavior and validation flow.
- `patch-node.ts`: patch/update semantics.
- `node-entity.ts` / `node-events.ts`: domain model and event definitions.

## Change Checklist
- Update request/response schemas when handler behavior changes.
- Keep mapper/entity behavior consistent with DB schema and integration test expectations.
- Add or update integration tests in `apps/api/test/integration/node-client.test.ts` when node API behavior changes.

## Client Impact
- Node API surface changes can alter generated OpenAPI client types.
- Regenerate via:
- repo root `bun run build` (or `bun run --cwd packages/openapi-client build`)
- `packages/openapi-client` root `bun run build`
