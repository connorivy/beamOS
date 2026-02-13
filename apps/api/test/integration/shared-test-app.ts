import { GenericContainer, Wait, type StartedTestContainer } from "testcontainers";
import type { createAppAndMigrate as CreateAppAndMigrate } from "../../src/server";

type AppServer = NonNullable<
  Awaited<ReturnType<typeof CreateAppAndMigrate>>["server"]
>;

let postgres: StartedTestContainer | undefined;
let server: AppServer | undefined;
let baseUrl = "";
let setupPromise: Promise<string> | undefined;
let teardownPromise: Promise<void> | undefined;
let refs = 0;

export const setupIntegrationApp = async (): Promise<string> => {
  refs += 1;

  if (!setupPromise) {
    setupPromise = (async () => {
      postgres = await new GenericContainer("postgres:17-alpine")
        .withEnvironment({ POSTGRES_USER: "beamos" })
        .withEnvironment({ POSTGRES_PASSWORD: "beamos" })
        .withEnvironment({ POSTGRES_DB: "beamos" })
        .withExposedPorts(5432)
        .withWaitStrategy(
          Wait.forLogMessage("database system is ready to accept connections", 2),
        )
        .start();

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
      return baseUrl;
    })();
  }

  return setupPromise;
};

export const teardownIntegrationApp = async (): Promise<void> => {
  refs -= 1;
  if (refs > 0) {
    return;
  }

  if (teardownPromise) {
    return teardownPromise;
  }

  teardownPromise = (async () => {
    server?.stop(true);
    await postgres?.stop();
    server = undefined;
    postgres = undefined;
    baseUrl = "";
    setupPromise = undefined;
    teardownPromise = undefined;
  })();

  return teardownPromise;
};

export const getIntegrationBaseUrl = (): string => baseUrl;
