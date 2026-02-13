import { defineEndpoint } from "@beamos/contracts";
import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getMaterialReqSchema = z.object({
  params: z.object({
    materialId: uuidV7Schema,
  }),
});

export const getMaterialResSchema = z.object({
  material: z.object({
    id: uuidV7Schema,
    revisionId: uuidV7Schema,
    pressureE: z.object({
      value: z.number().finite(),
      unit: z.literal(PressureUnits.Pascals),
    }),
    pressureG: z.object({
      value: z.number().finite(),
      unit: z.literal(PressureUnits.Pascals),
    }),
  }),
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
        pressureE: {
          value: material.pressureE.Pascals,
          unit: PressureUnits.Pascals as const,
        },
        pressureG: {
          value: material.pressureG.Pascals,
          unit: PressureUnits.Pascals as const,
        },
      },
    };
  },
});
