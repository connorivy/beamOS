import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import {
  setupIntegrationApp,
  teardownIntegrationApp,
} from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("typed openapi client integration", () => {
  it("creates a model", async () => {
    const client = createApiClient(baseUrl);

    const requestBody = {
      name: "Integration Test Model",
      authorId: randomUUID(),
      message: "Create a model through typed OpenAPI client",
    };

    const { data, error, response } = await client.POST("/api/models", {
      body: requestBody,
    });

    expect(error).toBeUndefined();
    expect(response.status).toBe(200);
    expect(data).toBeDefined();

    if (!data) {
      throw new Error("Expected response body from create model API");
    }

    expect(data.model.name).toBe(requestBody.name);
    expect(data.model.id).toMatch(/^[0-9a-f-]{36}$/i);

    // const storedModel = await drizzleModelRepository.getById({
    //   modelId: data.model.id,
    // });

    // expect(storedModel).toBeDefined();
    // expect(storedModel?.name).toBe(requestBody.name);
  });
});
