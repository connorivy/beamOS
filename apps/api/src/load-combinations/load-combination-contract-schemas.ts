import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

const loadCaseFactorsSchema = z.record(uuidV7Schema, z.number().finite());

export const loadCombinationPropertiesSchema = z
  .object({
    loadCaseFactors: loadCaseFactorsSchema,
  })
  .meta({ id: "LoadCombinationProperties" });

export const createLoadCombinationRequestSchema = loadCombinationPropertiesSchema
  .extend({
    tempId: z.string().trim().min(1).optional(),
  })
  .meta({ id: "CreateLoadCombinationRequest" });

export const putLoadCombinationRequestSchema = loadCombinationPropertiesSchema
  .extend({
    id: uuidV7Schema,
  })
  .meta({ id: "PutLoadCombinationRequest" });

export const loadCombinationResponseSchema = loadCombinationPropertiesSchema
  .extend({
    id: uuidV7Schema,
    revisionId: uuidV7Schema,
  })
  .meta({ id: "LoadCombination" });
