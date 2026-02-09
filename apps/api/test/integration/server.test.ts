import { beforeAll, describe, expect, it } from "bun:test";
import { db } from "../../src/db/client";
import { bootstrapDb } from "../../src/db/bootstrap";
import { users } from "../../src/db/schema";
import { createServer } from "../../src/server";

beforeAll(async () => {
  await bootstrapDb();
  await db.insert(users).values({ id: "integration-user", name: "Integration Ada" }).onConflictDoNothing();
});

describe("API integration", () => {
  it("returns user payload from endpoint runtime", async () => {
    const app = createServer();
    const response = await fetch(`http://127.0.0.1:${app.port}/api/users/integration-user`);
    const body = await response.json();

    expect(response.status).toBe(200);
    expect(body).toEqual({ id: "integration-user", name: "Integration Ada" });
    app.stop();
  });
});
