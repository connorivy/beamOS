# AGENTS.md

## Repo Overview
- Monorepo with Bun workspaces: `apps/*` and `packages/*`.
- Main projects:
- `apps/api`: Elysia + Drizzle backend.
- `apps/web`: React + Vite frontend.
- `packages/openapi-client`: generated API client consumed by API/web code.

## Core Commands
- Install deps: `bun install`
- Run both apps: `bun run dev`
- Build all workspaces: `bun run build`
- Run tests: `bun run test`

## Important: OpenAPI Client Generation
- The API client is generated as part of build.
- Generate via repo root: `bun run build` (includes `packages/openapi-client` build).
- Or generate from client package root: `bun run build` inside `packages/openapi-client`.
- Equivalent from repo root: `bun run --cwd packages/openapi-client build`.
- Generate-only (without TypeScript build): `bun run openapi:generate-client` or `bun run --cwd packages/openapi-client generate`.
- Do not hand-edit files under `packages/openapi-client/src/generated/`; regenerate instead.

## Working Norms For Agents
- Prefer small, scoped changes.
- Keep API schema/request/response updates synchronized with integration tests.
- Entity persistence for nodes/materials/section profiles/element1ds is event-sourced: write/read through `revision_changes` payloads instead of entity-specific tables.
- When API routes change, regenerate the OpenAPI client and verify TypeScript usage in dependent packages.
