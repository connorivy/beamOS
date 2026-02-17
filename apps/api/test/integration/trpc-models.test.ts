import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("trpc models endpoint", () => {
  it("returns models list from trpc procedure", async () => {
    const response = await fetch(`${baseUrl}/trpc/models.list`, {
      method: "POST",
    });

    expect(response.status).toBe(200);

    const body = (await response.json()) as {
      models: Array<{
        id: string;
        name: string;
        description: string;
        lastModified: string | null;
        role: "Owner" | "Contributor" | "Reviewer";
      }>;
    };

    expect(Array.isArray(body.models)).toBe(true);
  });
});
