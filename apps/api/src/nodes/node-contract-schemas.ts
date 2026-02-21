import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

const restraintSchema = z
    .object({
        canTranslateAlongX: z.boolean(),
        canTranslateAlongY: z.boolean(),
        canTranslateAlongZ: z.boolean(),
        canRotateAboutX: z.boolean(),
        canRotateAboutY: z.boolean(),
        canRotateAboutZ: z.boolean(),
    })
    .optional();

const spatialNodeLocationSchema = z.object({
    type: z.literal("spatial"),
    point: z.object({
        x: z.number(),
        y: z.number(),
        z: z.number(),
    }),
});

const internalNodeLocationSchema = z.object({
    type: z.literal("internal"),
    element1dId: uuidV7Schema,
    ratioAlongElement1d: z.number().min(0).max(1),
});

export const nodeLocationSchema = z.discriminatedUnion("type", [
    spatialNodeLocationSchema,
    internalNodeLocationSchema,
]);
export type NodeLocation = z.infer<typeof nodeLocationSchema>;

export const nodePropertiesSchema = z.object({
    restraint: restraintSchema,
    location: nodeLocationSchema,
    applicationId: z.string().trim().min(1).optional(),
});

export const createNodeRequestSchema = nodePropertiesSchema
    .extend({
        tempId: z.string().trim().min(1).optional(),
    })
    .meta({ id: "CreateNodeRequest" });
export type CreateNodeRequest = z.infer<typeof createNodeRequestSchema>;

export const putNodeRequestSchema = nodePropertiesSchema
    .extend({
        id: uuidV7Schema.optional(),
    })
    .meta({ id: "PutNodeRequest" });
export type PutNodeRequest = z.infer<typeof putNodeRequestSchema>;

export const deleteNodeRequestSchema = z.string().trim().min(1).meta({ id: "DeleteNodeRequest" });

export const nodeResponseSchema = nodePropertiesSchema
    .extend({
        id: uuidV7Schema,
        projectId: uuidV7Schema,
    })
    .meta({ id: "Node" });
