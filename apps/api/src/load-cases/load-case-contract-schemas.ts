import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

export const loadCasePropertiesSchema = z
  .object({
    name: z.string().trim().min(1),
  })
  .meta({ id: "LoadCaseProperties" });

export const createLoadCaseRequestSchema = loadCasePropertiesSchema
  .extend({
    tempId: z.string().trim().min(1).optional(),
  })
  .meta({ id: "CreateLoadCaseRequest" });

export const putLoadCaseRequestSchema = loadCasePropertiesSchema
  .extend({
    id: uuidV7Schema,
  })
  .meta({ id: "PutLoadCaseRequest" });

export const loadCaseResponseSchema = loadCasePropertiesSchema
  .extend({
    id: uuidV7Schema,
    revisionId: uuidV7Schema,
  })
  .meta({ id: "LoadCase" });
