import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

// Base material properties schema
export const materialPropertiesSchema = z
    .object({
        name: z.string().trim().min(1),
        modulusOfElasticity: z.number(),
        modulusOfRigidity: z.number(),
        applicationId: z.string().trim().min(1).optional(),
        units: z.object({
            pressure: z.enum(PressureUnits),
        }),
    })
    .meta({ id: "MaterialProperties" });

// Create material request schema
export const createMaterialRequestSchema = materialPropertiesSchema.meta({
    id: "CreateMaterialRequest",
});
export type CreateMaterialRequest = z.infer<typeof createMaterialRequestSchema>;

// Put material request schema (replaces update)
export const putMaterialRequestSchema = materialPropertiesSchema
    .extend({
        newName: z.string().trim().min(1).optional(),
    })
    .meta({ id: "PutMaterialRequest" });
export type PutMaterialRequest = z.infer<typeof putMaterialRequestSchema>;

// Delete material request schema
export const deleteMaterialRequestSchema = z
    .string()
    .trim()
    .min(1)
    .meta({ id: "DeleteMaterialRequest" });

export const materialResponseSchema = materialPropertiesSchema
    .extend({
        id: uuidV7Schema,
        revisionId: uuidV7Schema,
    })
    .meta({ id: "Material" });

export const materialResponseArraySchema = z
    .array(materialResponseSchema)
    .meta({ id: "MaterialArray" });

export const materialPersistenceSchemaV1 = z
    .object({
        name: z.string().min(1),
        modulusOfElasticity: z.number(),
        modulusOfRigidity: z.number(),
    })
    .meta({ id: "MaterialPersistenceV1" });

export type MaterialPersistenceV1 = z.infer<typeof materialPersistenceSchemaV1>;
