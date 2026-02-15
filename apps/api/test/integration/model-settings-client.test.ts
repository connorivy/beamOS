import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("model settings integration", () => {
  it("stores latest model settings in model revision aggregate", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Model Settings Model",
        authorId: randomUUID(),
        message: "Create model for settings test",
      },
    });

    expect(createModelResponse.error).toBeUndefined();
    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const modelId = createModelResponse.data.model.id;
    const branchName = createModelResponse.data.version.branchName;

    const putSettings = async (body: {
      units: {
        pressure: PressureUnits;
        area: AreaUnits;
        areaMomentOfInertia: AreaMomentOfInertiaUnits;
        warpingMomentOfInertia: WarpingMomentOfInertiaUnits;
        volume: VolumeUnits;
      };
      yAxisUp: boolean;
    }) =>
      fetch(
        `${baseUrl}/api/models/${modelId}/branches/${branchName}/model-settings`,
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
        pressure: PressureUnits.Pascals,
        area: AreaUnits.SquareMeters,
        areaMomentOfInertia: AreaMomentOfInertiaUnits.MetersToTheFourth,
        warpingMomentOfInertia:
          WarpingMomentOfInertiaUnits.MetersToTheSixth,
        volume: VolumeUnits.CubicMeters,
      },
      yAxisUp: true,
    });

    expect(firstResponse.status).toBe(200);

    const secondResponse = await putSettings({
      units: {
        pressure: PressureUnits.Bars,
        area: AreaUnits.SquareFeet,
        areaMomentOfInertia: AreaMomentOfInertiaUnits.FootToTheFourth,
        warpingMomentOfInertia: WarpingMomentOfInertiaUnits.FootToTheSixth,
        volume: VolumeUnits.CubicFeet,
      },
      yAxisUp: false,
    });

    expect(secondResponse.status).toBe(200);

    const getRevisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: { modelId, branchName },
        },
      },
    );

    expect(getRevisionResponse.error).toBeUndefined();
    expect(getRevisionResponse.response.status).toBe(200);
    expect(getRevisionResponse.data).toBeDefined();

    if (!getRevisionResponse.data) {
      throw new Error("Expected model revision response");
    }

    const modelRevision = getRevisionResponse.data.modelRevision as any;

    expect(modelRevision.modelSettings).toEqual({
      id: expect.any(String),
      revisionId: expect.any(String),
      units: {
        pressure: PressureUnits.Bars,
        area: AreaUnits.SquareFeet,
        areaMomentOfInertia: AreaMomentOfInertiaUnits.FootToTheFourth,
        warpingMomentOfInertia: WarpingMomentOfInertiaUnits.FootToTheSixth,
        volume: VolumeUnits.CubicFeet,
      },
      yAxisUp: false,
    });
  });
});
