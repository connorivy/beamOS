import { defineEndpoint } from "@beamos/contracts";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { Element1dEntity } from "./element1d-entity";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

const element1dInputSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  startNodeId: uuidV7Schema,
  endNodeId: uuidV7Schema,
  materialId: uuidV7Schema,
  sectionProfileId: uuidV7Schema,
});

const element1dResSchema = z.object({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
  startNodeId: uuidV7Schema,
  endNodeId: uuidV7Schema,
  materialId: uuidV7Schema,
  sectionProfileId: uuidV7Schema,
});

export const batchCreateElement1dReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    element1ds: z.array(element1dInputSchema).min(1),
  }),
});

export const batchCreateElement1dResSchema = z.object({
  element1ds: z.array(element1dResSchema),
  tempIdToId: z.record(z.string(), uuidV7Schema),
});

const toResponseElement1d = (element1d: Element1dEntity) => ({
  id: element1d.id,
  revisionId: element1d.revisionId,
  startNodeId: element1d.startNodeId,
  endNodeId: element1d.endNodeId,
  materialId: element1d.materialId,
  sectionProfileId: element1d.sectionProfileId,
});

export const batchCreateElement1d = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/element1ds/batch",
  req: batchCreateElement1dReqSchema,
  res: batchCreateElement1dResSchema,
  async handler(req, ctx: AppContext) {
    const seenTempIds = new Set<string>();

    for (const element1d of req.body.element1ds) {
      if (!element1d.tempId) {
        continue;
      }

      if (seenTempIds.has(element1d.tempId)) {
        throw httpError(`Duplicate tempId \"${element1d.tempId}\"`, 400);
      }

      seenTempIds.add(element1d.tempId);
    }

    const tempIdToId: Record<string, string> = {};
    const { modelId, branchName } = req.params;
    
    // Create a new revision and update branch head
    const newRevisionId = await ctx.services.modelRevisionRepository.createRevisionAndUpdateBranchHead({
      modelId,
      branchName,
      authorId: "system", // TODO: get from auth context
      message: "Add element1ds",
    });

    const entities = req.body.element1ds.map((element1d) => {
      const id = Bun.randomUUIDv7();

      if (element1d.tempId) {
        tempIdToId[element1d.tempId] = id;
      }

      return Element1dEntity.create({
        id,
        revisionId: newRevisionId,
        startNodeId: element1d.startNodeId,
        endNodeId: element1d.endNodeId,
        materialId: element1d.materialId,
        sectionProfileId: element1d.sectionProfileId,
      });
    });

    const saved = await ctx.services.element1dRepository.batchCreate(entities);

    return {
      element1ds: saved.map((element1d) => toResponseElement1d(element1d)),
      tempIdToId,
    };
  },
});
