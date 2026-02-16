# beamOS Scaffold

Bun full-stack scaffold with:
- React + Vite frontend
- Three.js viewer component
- Bun API with Zod-validated DIY endpoint pattern
- Drizzle ORM (SQLite)
- TDD setup: unit and integration tests

## Quick Start

```bash
bun install
bun run db:migrate
bun run dev
```

- Web: `http://127.0.0.1:5173`
- API: `http://127.0.0.1:3001`

## Testing

```bash
bun run test            # unit + integration
```

## TDD Workflow

1. Write or update a failing unit/integration test.
2. Implement code in `apps/api` / `apps/web`.
3. Run targeted tests:
   - `bun run --cwd apps/api test:unit`
   - `bun run --cwd apps/api test:integration`
   - `bun run --cwd apps/web test:unit`
4. Refactor while keeping tests green.

## Drizzle

Schema file is `apps/api/src/db/schema.ts`.

```bash
bun run db:generate
bun run db:migrate
```
