import { afterAll, beforeAll } from "bun:test";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

beforeAll(async () => {
  await setupIntegrationApp();
}, 30_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 30_000);
