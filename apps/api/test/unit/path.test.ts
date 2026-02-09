import { describe, expect, it } from "bun:test";
import { matchPath } from "../../src/lib/path";

describe("matchPath", () => {
  it("extracts route params", () => {
    expect(matchPath("/api/users/:id", "/api/users/42")).toEqual({ id: "42" });
  });

  it("returns null when segments do not match", () => {
    expect(matchPath("/api/users/:id", "/api/projects/42")).toBeNull();
  });
});
