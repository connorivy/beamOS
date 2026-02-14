# AGENTS.md

## Scope
- Applies to `packages/openapi-client`.
- This package contains generated OpenAPI client artifacts and exports.

## Generation Rules
- Primary generation script: `bun run generate`
- Build pipeline (cleans + regenerates + typechecks): `bun run build`
- Build command can be run:
- From repo root: `bun run --cwd packages/openapi-client build` or `bun run build` (workspace build)
- From this package root: `bun run build`

## Source of Truth
- Generated files are in `src/generated/`.
- `src/generated/*` is generated output; do not hand-edit.
- If generated output is stale or incorrect, rerun generation/build from one of the commands above.

## Editing Guidance
- Manual edits should generally be limited to non-generated entrypoints like `src/index.ts` and scripts.
- Keep script changes in `scripts/generate-openapi-client.mjs` deterministic and CI-safe.
