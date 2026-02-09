import { describe, expect, it } from "bun:test";
import { formatPostgresUrl } from "../../src/db/postgres-url";

describe("formatPostgresUrl", () => {
  it("builds a PostgreSQL URL and URL-encodes credentials", () => {
    const url = formatPostgresUrl({
      host: "localhost",
      port: 54035,
      username: "postgres",
      password: "D.{3{*E5MMEmckbc!byXNq",
      database: "db",
    });

    expect(url).toBe(
      "postgresql://postgres:D.%7B3%7B*E5MMEmckbc!byXNq@localhost:54035/db",
    );
  });

  it("normalizes database names and appends optional query params", () => {
    const url = formatPostgresUrl({
      host: "localhost",
      port: "5432",
      username: "beamos",
      password: "beamos",
      database: "/beamos",
      params: {
        sslmode: "disable",
        connect_timeout: 5,
      },
    });

    expect(url).toBe(
      "postgresql://beamos:beamos@localhost:5432/beamos?sslmode=disable&connect_timeout=5",
    );
  });
});
