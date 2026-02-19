import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { createApiClient } from "@beamos/openapi-client";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

const defaultModelSettings = {
  units: {
    pressure: "Pascal",
    area: "SquareMeter",
    areaMomentOfInertia: "MeterToTheFourth",
    warpingMomentOfInertia: "MeterToTheSixth",
    volume: "CubicMeter",
  },
  yAxisUp: true,
} as const;

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 30_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 30_000);

describe("model settings integration", () => {
  it("stores latest model settings in model revision aggregate", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/projects", {
      body: {
        name: "Model Settings Model",
        description: "Create model for settings test",
      modelSettings: defaultModelSettings,
      },
    });

    expect(createModelResponse.error).toBeUndefined();
    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const projectId = createModelResponse.data.id;
    const branchName = "main";

    const putSettings = async (body: {
      units: Record<string, string>;
      yAxisUp: boolean;
    }) =>
      fetch(
        `${baseUrl}/api/projects/${projectId}/branches/${branchName}/model-settings`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(body),
        },
      );

    const firstResponse = await putSettings({
      units: {
        pressure: "Pascal",
        area: "SquareMeter",
        areaMomentOfInertia: "MeterToTheFourth",
        warpingMomentOfInertia: "MeterToTheSixth",
        volume: "CubicMeter",
      },
      yAxisUp: true,
    });

    expect(firstResponse.status).toBe(200);

    const secondResponse = await putSettings({
      units: {
        pressure: "Bar",
        area: "SquareFoot",
        areaMomentOfInertia: "FootToTheFourth",
        warpingMomentOfInertia: "FootToTheSixth",
        volume: "CubicFoot",
      },
      yAxisUp: false,
    });

    expect(secondResponse.status).toBe(200);

    const getRevisionResponse = await client.GET(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: {
          path: { projectId, branchName },
        },
      },
    );

    expect(getRevisionResponse.error).toBeUndefined();
    expect(getRevisionResponse.response.status).toBe(200);
    expect(getRevisionResponse.data).toBeDefined();

    if (!getRevisionResponse.data) {
      throw new Error("Expected model revision response");
    }

    const modelRevision = getRevisionResponse.data;

    expect(modelRevision.modelSettings).toEqual({
      id: expect.any(String),
      revisionId: expect.any(String),
      units: {
        pressure: "Bar",
        area: "SquareFoot",
        areaMomentOfInertia: "FootToTheFourth",
        warpingMomentOfInertia: "FootToTheSixth",
        volume: "CubicFoot",
      },
      yAxisUp: false,
    });
  });
});
