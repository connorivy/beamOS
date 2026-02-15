import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

// Base material properties schema
export const materialPropertiesSchema = z.object({
  modulusOfElasticity: z.number().finite(),
  modulusOfRigidity: z.number().finite(),
  units: z.object({
    pressure: z.enum(PressureUnits),
  }),
});

// Create material request schema
export const createMaterialRequestSchema = materialPropertiesSchema.extend({
  tempId: z.string().trim().min(1).optional(),
});

// Put material request schema (replaces update)
export const putMaterialRequestSchema = materialPropertiesSchema.extend({
  id: uuidV7Schema,
});

// Delete material request schema
export const deleteMaterialRequestSchema = z.string().trim().min(1);

// Material response schemas
export const materialPropertiesResponseSchema = z.object({
  modulusOfElasticity: z.number().finite(),
  modulusOfRigidity: z.number().finite(),
  units: z.object({
    pressure: z.literal(PressureUnits.Pascals),
  }),
});

export const materialResponseSchema = materialPropertiesResponseSchema.extend({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
});
