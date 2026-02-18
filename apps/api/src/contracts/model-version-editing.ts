import { z } from "zod";

const uuidSchema = z.string().uuid();

const modelVersionRefSchema = z.object({
  modelId: uuidSchema,
  revisionId: uuidSchema.nullable(),
});

const createModelRequestBodySchema = z
  .object({
    modelId: uuidSchema,
    name: z.string().min(1),
    authorId: uuidSchema,
    message: z.string().min(1),
    branchName: z.string().min(1).optional(),
    revisionId: uuidSchema.optional(),
  })
  .meta({ id: "CreateModelEditingRequestBody" });

export const createModelReqSchema = z
  .object({
    body: createModelRequestBodySchema,
  })
  .meta({ id: "CreateModelEditingEndpointRequest" });

const createNodeRequestBodySchema = z
  .object({
    nodeId: uuidSchema,
    name: z.string().min(1),
    revisionId: uuidSchema,
  })
  .meta({ id: "CreateNodeEditingRequestBody" });

export const createNodeReqSchema = z
  .object({
    params: z.object({
      modelId: uuidSchema,
    }),
    body: createNodeRequestBodySchema,
  })
  .meta({ id: "CreateNodeEditingEndpointRequest" });

export const createNodeResSchema = z
  .object({
    node: z.object({
      id: uuidSchema,
      modelId: uuidSchema,
      name: z.string(),
    }),
    version: modelVersionRefSchema,
  })
  .meta({ id: "CreateNodeEndpointResponse" });

const patchNodeRequestBodySchema = z
  .object({
    name: z.string().min(1),
    revisionId: uuidSchema,
  })
  .meta({ id: "PatchNodeRequest" });

export const patchNodeReqSchema = z
  .object({
    params: z.object({
      modelId: uuidSchema,
      nodeId: uuidSchema,
    }),
    body: patchNodeRequestBodySchema,
  })
  .meta({ id: "PatchNodeEndpointRequest" });

export const patchNodeResSchema = z
  .object({
    node: z.object({
      id: uuidSchema,
      modelId: uuidSchema,
      name: z.string(),
    }),
    version: modelVersionRefSchema,
  })
  .meta({ id: "PatchNodeEditingResponse" });

const patchModelRequestBodySchema = z
  .object({
    name: z.string().min(1),
    revisionId: uuidSchema,
  })
  .meta({ id: "PatchModelRequest" });

export const patchModelReqSchema = z
  .object({
    params: z.object({
      modelId: uuidSchema,
    }),
    body: patchModelRequestBodySchema,
  })
  .meta({ id: "PatchModelEndpointRequest" });

export const patchModelResSchema = z
  .object({
    model: z.object({
      id: uuidSchema,
      name: z.string(),
      description: z.string(),
    }),
    version: modelVersionRefSchema,
  })
  .meta({ id: "PatchModelResponse" });

export type CreateModelReq = z.infer<typeof createModelReqSchema>;
export type CreateNodeReq = z.infer<typeof createNodeReqSchema>;
export type PatchNodeReq = z.infer<typeof patchNodeReqSchema>;
export type PatchModelReq = z.infer<typeof patchModelReqSchema>;
