import { defineEndpoint } from "../contracts/endpoint";
import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { materialResponseSchema } from "./material-response-schema";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getMaterialReqSchema = z.object({
  params: z.object({
    materialId: uuidV7Schema,
  }),
});

export const getMaterialResSchema = z.object({
  material: materialResponseSchema,
});

export const getMaterial = defineEndpoint({
  method: "GET",
  path: "/api/materials/:materialId",
  req: getMaterialReqSchema,
  res: getMaterialResSchema,
  async handler(req, ctx: AppContext) {
    const material = await ctx.services.materialRepository.getById(
      req.params.materialId,
    );

    if (!material) {
      throw httpError("Material not found", 404);
    }

    return {
      material: {
        id: material.id,
        revisionId: material.revisionId,
        modulusOfElasticity: material.pressureE.Pascals,
        modulusOfRigidity: material.pressureG.Pascals,
        units: {
          pressure: PressureUnits.Pascals as const,
        },
      },
    };
  },
});
