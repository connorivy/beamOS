import { defineConfig } from "@playwright/test";

export default defineConfig({
  testDir: "./tests",
  timeout: 30_000,
  retries: 1,
  projects: [
    {
      name: "setup env",
      testMatch: /setup\/global\.setup\.ts/,
      teardown: "cleanup env",
    },
    {
      name: "cleanup env",
      testMatch: /setup\/global\.teardown\.ts/,
    },
    {
      name: "oss e2e",
      testMatch: /e2e\/.*\.e2e\.ts/,
      dependencies: ["setup env"],
      use: {
        baseURL: "http://127.0.0.1:5173",
        trace: "on-first-retry",
      },
    },
  ],
});
