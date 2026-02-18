import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import {
  createElement1dRequestSchema,
  putElement1dRequestSchema as updateElement1dRequestSchema,
} from "../element1ds/element1d-contract-schemas";
import {
  createMaterialRequestSchema,
  putMaterialRequestSchema as updateMaterialRequestSchema,
} from "../materials/material-contract-schemas";
import {
  createNodeRequestSchema,
  putNodeRequestSchema as updateNodeRequestSchema,
} from "../nodes/node-contract-schemas";
import {
  createSectionProfileRequestSchema,
  putSectionProfileRequestSchema as updateSectionProfileRequestSchema,
} from "../section-profiles/section-profile-contract-schemas";

const element1dOperationsRequestSchema = z
  .object({
    create: z.array(createElement1dRequestSchema).optional(),
    update: z.array(updateElement1dRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionElement1dOperationsRequest" });

const nodeOperationsRequestSchema = z
  .object({
    create: z.array(createNodeRequestSchema).optional(),
    update: z.array(updateNodeRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionNodeOperationsRequest" });

const materialOperationsRequestSchema = z
  .object({
    create: z.array(createMaterialRequestSchema).optional(),
    update: z.array(updateMaterialRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionMaterialOperationsRequest" });

const sectionProfileOperationsRequestSchema = z
  .object({
    create: z.array(createSectionProfileRequestSchema).optional(),
    update: z.array(updateSectionProfileRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionSectionProfileOperationsRequest" });

export const createModelRevisionReqSchema = z
  .object({
    params: z.object({
      modelId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
    body: z
      .object({
        element1ds: element1dOperationsRequestSchema,
        nodes: nodeOperationsRequestSchema,
        materials: materialOperationsRequestSchema,
        sectionProfiles: sectionProfileOperationsRequestSchema,
      })
      .meta({ id: "CreateModelRevisionRequest" }),
  })
  .meta({ id: "CreateModelRevisionEndpointRequest" });
