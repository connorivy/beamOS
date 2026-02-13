import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import type { createAppAndMigrate as CreateAppAndMigrate } from "../../src/server";
import { GenericContainer, Wait } from "testcontainers";

type AppServer = NonNullable<
  Awaited<ReturnType<typeof CreateAppAndMigrate>>["server"]
>;

let server: AppServer | undefined;
let baseUrl = "";

const postgres = await new GenericContainer("postgres:17-alpine")
  .withEnvironment({ POSTGRES_USER: "beamos" })
  .withEnvironment({ POSTGRES_PASSWORD: "beamos" })
  .withEnvironment({ POSTGRES_DB: "beamos" })
  .withExposedPorts(5432)
  .withWaitStrategy(
    Wait.forLogMessage("database system is ready to accept connections", 2),
  )
  .start();

beforeAll(async () => {
  const dbUri = `postgres://beamos:beamos@${postgres.getHost()}:${postgres.getMappedPort(5432)}/beamos`;
  process.env.DB_URI = dbUri;
  const { createAppAndMigrate } = await import("../../src/server");
  const app = await createAppAndMigrate();
  app.listen(0);

  if (!app.server) {
    throw new Error("Failed to start test API server");
  }

  server = app.server;
  baseUrl = `http://127.0.0.1:${server.port}`;
}, 10_000);

afterAll(async () => {
  server?.stop(true);
  await postgres?.stop();
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
