# AGENTS.md

## Scope
- Applies to `apps/web`.
- React + Vite frontend consuming shared contracts and API client types.

## Commands
- Dev server: `bun run --cwd apps/web dev`
- Build: `bun run --cwd apps/web build`
- Unit tests: `bun run --cwd apps/web test:unit`

## API Client Dependency
- Web code may depend on generated client types/behavior from `packages/openapi-client`.
- If API routes or schemas changed and typing/runtime behavior seems stale, regenerate client:
- repo root: `bun run build` or `bun run --cwd packages/openapi-client build`
- openapi-client package root: `bun run build`

## Editing Guidance
- Keep API usage centralized (see `src/lib/api.ts`).
- When changing API data shapes in UI, confirm backend schemas and generated client types are aligned.
