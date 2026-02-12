import { z } from "zod";

const uuidSchema = z.string().uuid();

const targetRevisionOrDraftSchema = z
  .object({
    revisionId: uuidSchema.optional(),
    draftId: uuidSchema.optional(),
  })
  .refine(
    (value) =>
      (Boolean(value.revisionId) && !value.draftId) ||
      (Boolean(value.draftId) && !value.revisionId),
    {
      message: "Exactly one of revisionId or draftId is required",
      path: ["revisionId"],
    },
  );

const modelVersionRefSchema = z.object({
  modelId: uuidSchema,
  revisionId: uuidSchema.nullable(),
  draftId: uuidSchema.nullable(),
});

export const createModelReqSchema = z.object({
  body: z.object({
    modelId: uuidSchema,
    name: z.string().min(1),
    authorId: uuidSchema,
    message: z.string().min(1),
    branchName: z.string().min(1).optional(),
    target: z.enum(["revision", "draft"]).default("draft"),
    revisionId: uuidSchema.optional(),
    draftId: uuidSchema.optional(),
  }).superRefine((value, ctx) => {
    if (value.target === "revision" && value.draftId) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "draftId is not allowed when target is revision",
        path: ["draftId"],
      });
    }

    if (value.target === "draft" && value.revisionId) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "revisionId is not allowed when target is draft",
        path: ["revisionId"],
      });
    }
  }),
});

export const createNodeReqSchema = z.object({
  params: z.object({
    modelId: uuidSchema,
  }),
  body: z.object({
    nodeId: uuidSchema,
    name: z.string().min(1),
    target: targetRevisionOrDraftSchema,
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
    target: targetRevisionOrDraftSchema,
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
    target: targetRevisionOrDraftSchema,
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
