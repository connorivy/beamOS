import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import {
  createElement1dRequestSchema,
  deleteElement1dRequestSchema,
  putElement1dRequestSchema as updateElement1dRequestSchema,
} from "../element1ds/element1d-contract-schemas";
import {
  createMaterialRequestSchema,
  deleteMaterialRequestSchema,
  putMaterialRequestSchema as updateMaterialRequestSchema,
} from "../materials/material-contract-schemas";
import {
  createNodeRequestSchema,
  deleteNodeRequestSchema,
  putNodeRequestSchema as updateNodeRequestSchema,
} from "../nodes/node-contract-schemas";
import {
  createSectionProfileRequestSchema,
  deleteSectionProfileRequestSchema,
  putSectionProfileRequestSchema as updateSectionProfileRequestSchema,
} from "../section-profiles/section-profile-contract-schemas";

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
