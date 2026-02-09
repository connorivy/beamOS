import { defineConfig } from "@playwright/test";

export default defineConfig({
  testDir: "./tests/e2e",
  timeout: 30_000,
  use: {
    baseURL: "http://127.0.0.1:5173",
    trace: "on-first-retry",
  },
  webServer: [
    {
      command: "bun run --cwd apps/api dev:test",
      url: "http://127.0.0.1:3001/health",
      reuseExistingServer: true,
      timeout: 60_000,
    },
    {
      command: "bun run --cwd apps/web dev:test",
      url: "http://127.0.0.1:5173",
      reuseExistingServer: true,
      timeout: 60_000,
    },
  ],
});
