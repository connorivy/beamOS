import { defineEndpoint } from "../contracts/endpoint";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { getDb } from "../db/client";
import { uuidV7Schema } from "src/common/uuid";
import {
  modelSettingsPropertiesSchema,
  modelSettingsResponseSchema,
} from "./model-settings-contract-schemas";
import type { ModelSettingsSnapshot } from "./model-settings-entity";
import { createNewRevisionAggregateHandler } from "src/model-revisions/create-model-revision";

export const putModelSettingsReqSchema = z
  .object({
    params: z.object({
      modelId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
    body: modelSettingsPropertiesSchema,
  })
  .meta({ id: "PutModelSettingsRequest" });

export const putModelSettingsResSchema = z
  .object({
    modelSettings: modelSettingsResponseSchema,
  })
  .meta({ id: "PutModelSettingsResponse" });

const toResponseModelSettings = (modelSettings: ModelSettingsSnapshot) => ({
  id: modelSettings.id,
  revisionId: modelSettings.revisionId,
  units: modelSettings.units,
  yAxisUp: modelSettings.yAxisUp,
});

export const putModelSettings = defineEndpoint({
  method: "PUT",
  path: "/api/models/:modelId/branches/:branchName/model-settings",
  req: putModelSettingsReqSchema,
  res: putModelSettingsResSchema,
  async handler(req, ctx: AppContext) {
    const revision = await createNewRevisionAggregateHandler(
      req,
      ctx,
      "Update model settings",
    );

    const snapshot: ModelSettingsSnapshot = {
      id: Bun.randomUUIDv7(),
      revisionId: revision.id,
      units: req.body.units,
      yAxisUp: req.body.yAxisUp,
    };
    revision.setModelSettings(snapshot);

    await getDb().transaction(async (tx) => {
      await ctx.services.modelRevisionRepository.save({
        revision,
        branchName: req.params.branchName,
        tx,
      });
    });

    return {
      modelSettings: toResponseModelSettings(snapshot),
    };
  },
});
