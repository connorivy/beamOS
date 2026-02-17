import { describe, expect, it } from "bun:test";
import { getUserResSchema } from "../../api/src/contracts/user";
import { getModelRouteParams } from "./pages/model-route-utils";
import { routePathMatches } from "./plugins/types";

describe("contracts usable in web", () => {
  it("parses user schema", () => {
    const parsed = getUserResSchema.parse({ id: "1", name: "Ada" });
    expect(parsed.name).toBe("Ada");
  });
});

describe("route helpers", () => {
  it("matches dynamic model revision route path", () => {
    expect(
      routePathMatches("/models/:modelId/:branchName", "/models/model-123/main"),
    ).toBe(true);
  });

  it("extracts route params from model revision path", () => {
    expect(getModelRouteParams("/models/model-123/main")).toEqual({
      modelId: "model-123",
      branchName: "main",
    });
  });
});
