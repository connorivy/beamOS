import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import { createElement1dRequestSchema } from "../element1ds/create-element1d-request-schema";
import { deleteElement1dRequestSchema } from "../element1ds/delete-element1d-request-schema";
import { updateElement1dRequestSchema } from "../element1ds/update-element1d-request-schema";
import { createMaterialRequestSchema } from "../materials/create-material-request-schema";
import { deleteMaterialRequestSchema } from "../materials/delete-material-request-schema";
import { updateMaterialRequestSchema } from "../materials/update-material-request-schema";
import { createNodeRequestSchema } from "../nodes/create-node-request-schema";
import { deleteNodeRequestSchema } from "../nodes/delete-node-request-schema";
import { updateNodeRequestSchema } from "../nodes/update-node-request-schema";
import { createSectionProfileRequestSchema } from "../section-profiles/create-section-profile-request-schema";
import { deleteSectionProfileRequestSchema } from "../section-profiles/delete-section-profile-request-schema";
import { updateSectionProfileRequestSchema } from "../section-profiles/update-section-profile-request-schema";

const element1dOperationsRequestSchema = z.object({
  create: z.array(createElement1dRequestSchema).default([]),
  update: z.array(updateElement1dRequestSchema).default([]),
  delete: z.array(deleteElement1dRequestSchema).default([]),
});

const nodeOperationsRequestSchema = z.object({
  create: z.array(createNodeRequestSchema).default([]),
  update: z.array(updateNodeRequestSchema).default([]),
  delete: z.array(deleteNodeRequestSchema).default([]),
});

const materialOperationsRequestSchema = z.object({
  create: z.array(createMaterialRequestSchema).default([]),
  update: z.array(updateMaterialRequestSchema).default([]),
  delete: z.array(deleteMaterialRequestSchema).default([]),
});

const sectionProfileOperationsRequestSchema = z.object({
  create: z.array(createSectionProfileRequestSchema).default([]),
  update: z.array(updateSectionProfileRequestSchema).default([]),
  delete: z.array(deleteSectionProfileRequestSchema).default([]),
});

export const createModelRevisionReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    element1ds: element1dOperationsRequestSchema,
    nodes: nodeOperationsRequestSchema,
    materials: materialOperationsRequestSchema,
    sectionProfiles: sectionProfileOperationsRequestSchema,
  }),
});
