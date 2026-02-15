import { z } from "zod";

const uuidSchema = z.string().uuid();

const modelVersionRefSchema = z.object({
  modelId: uuidSchema,
  revisionId: uuidSchema.nullable(),
});

export const createModelReqSchema = z.object({
  body: z.object({
    modelId: uuidSchema,
    name: z.string().min(1),
    authorId: uuidSchema,
    message: z.string().min(1),
    branchName: z.string().min(1).optional(),
    revisionId: uuidSchema.optional(),
  }),
});

export const createNodeReqSchema = z.object({
  params: z.object({
    modelId: uuidSchema,
  }),
  body: z.object({
    nodeId: uuidSchema,
    name: z.string().min(1),
    revisionId: uuidSchema,
  }),
});

export const createNodeResSchema = z.object({
  node: z.object({
    id: uuidSchema,
    modelId: uuidSchema,
    name: z.string(),
  }),
  version: modelVersionRefSchema,
});

export const patchNodeReqSchema = z.object({
  params: z.object({
    modelId: uuidSchema,
    nodeId: uuidSchema,
  }),
  body: z.object({
    name: z.string().min(1),
    revisionId: uuidSchema,
  }),
});

export const patchNodeResSchema = z.object({
  node: z.object({
    id: uuidSchema,
    modelId: uuidSchema,
    name: z.string(),
  }),
  version: modelVersionRefSchema,
});

export const patchModelReqSchema = z.object({
  params: z.object({
    modelId: uuidSchema,
  }),
  body: z.object({
    name: z.string().min(1),
    revisionId: uuidSchema,
  }),
});

export const patchModelResSchema = z.object({
  model: z.object({
    id: uuidSchema,
    name: z.string(),
  }),
  version: modelVersionRefSchema,
});

export type CreateModelReq = z.infer<typeof createModelReqSchema>;
export type CreateNodeReq = z.infer<typeof createNodeReqSchema>;
export type PatchNodeReq = z.infer<typeof patchNodeReqSchema>;
export type PatchModelReq = z.infer<typeof patchModelReqSchema>;
