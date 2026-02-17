import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

export const element1dPropertiesSchema = z.object({
  startNodeId: uuidV7Schema,
  endNodeId: uuidV7Schema,
  materialId: uuidV7Schema,
  sectionProfileId: uuidV7Schema,
});

export const createElement1dRequestSchema = element1dPropertiesSchema.extend({
  tempId: z.string().trim().min(1).optional(),
});

export const putElement1dRequestSchema = element1dPropertiesSchema.extend({
  id: uuidV7Schema,
});

export const deleteElement1dRequestSchema = z.string().trim().min(1);

export const element1dResponseSchema = element1dPropertiesSchema
  .extend({
    id: uuidV7Schema,
    revisionId: uuidV7Schema,
  })
  .meta({ id: "Element1d" });
