import { defineEndpoint } from "@beamos/contracts";
import { Pressure, PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { MaterialEntity } from "./material-entity";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

const pressureDtoSchema = z.object({
  value: z.number().finite(),
  unit: z.enum(PressureUnits),
});

const materialInputSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  pressureE: pressureDtoSchema,
  pressureG: pressureDtoSchema,
});

const materialResSchema = z.object({
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
});

export const batchCreateMaterialReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    materials: z.array(materialInputSchema).min(1),
  }),
});

export const batchCreateMaterialResSchema = z.object({
  materials: z.array(materialResSchema),
  tempIdToId: z.record(z.string(), uuidV7Schema),
});

const toResponseMaterial = (material: MaterialEntity) => ({
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
});

export const batchCreateMaterial = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/materials/batch",
  req: batchCreateMaterialReqSchema,
  res: batchCreateMaterialResSchema,
  async handler(req, ctx: AppContext) {
    const seenTempIds = new Set<string>();
    for (const material of req.body.materials) {
      if (!material.tempId) {
        continue;
      }
      if (seenTempIds.has(material.tempId)) {
        throw httpError(`Duplicate tempId "${material.tempId}"`, 400);
      }
      seenTempIds.add(material.tempId);
    }

    const tempIdToId: Record<string, string> = {};
    const { modelId, branchName } = req.params;
    
    // Create a new revision and update branch head
    const newRevisionId = await ctx.services.modelRevisionRepository.createRevisionAndUpdateBranchHead({
      modelId,
      branchName,
      authorId: Bun.randomUUIDv7(),
      message: "Add materials",
    });

    const entities = req.body.materials.map((material) => {
      const id = Bun.randomUUIDv7();
      if (material.tempId) {
        tempIdToId[material.tempId] = id;
      }

      return MaterialEntity.create({
        id,
        revisionId: newRevisionId,
        pressureE: new Pressure(
          material.pressureE.value,
          material.pressureE.unit,
        ),
        pressureG: new Pressure(
          material.pressureG.value,
          material.pressureG.unit,
        ),
      });
    });

    const saved = await ctx.services.materialRepository.batchCreate(entities);
    return {
      materials: saved.map((material) => toResponseMaterial(material)),
      tempIdToId,
    };
  },
});
