import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import {
  createElement1dRequestSchema,
  element1dResponseSchema,
  putElement1dRequestSchema as updateElement1dRequestSchema,
} from "../element1ds/element1d-contract-schemas";
import {
  createMaterialRequestSchema,
  materialResponseSchema,
  putMaterialRequestSchema as updateMaterialRequestSchema,
} from "../materials/material-contract-schemas";
import {
  createLoadCaseRequestSchema,
  loadCaseResponseSchema,
  putLoadCaseRequestSchema as updateLoadCaseRequestSchema,
} from "../load-cases/load-case-contract-schemas";
import {
  createLoadCombinationRequestSchema,
  loadCombinationResponseSchema,
  putLoadCombinationRequestSchema as updateLoadCombinationRequestSchema,
} from "../load-combinations/load-combination-contract-schemas";
import {
  createPointLoadRequestSchema,
  pointLoadResponseSchema,
  putPointLoadRequestSchema as updatePointLoadRequestSchema,
} from "../point-loads/point-load-contract-schemas";
import {
  createNodeRequestSchema,
  revisionNodeResponseSchema,
  putNodeRequestSchema as updateNodeRequestSchema,
} from "../nodes/node-contract-schemas";
import {
  createSectionProfileRequestSchema,
  deleteSectionProfileRequestSchema,
  sectionProfileResponseSchema,
  putSectionProfileRequestSchema as updateSectionProfileRequestSchema,
} from "../section-profiles/section-profile-contract-schemas";
import { modelSettingsResponseSchema } from "src/model-settings/model-settings-contract-schemas";

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
    delete: z.array(deleteSectionProfileRequestSchema).optional(),
  })
  .meta({ id: "CreateModelRevisionSectionProfileOperationsRequest" });

const loadCaseOperationsRequestSchema = z
  .object({
    create: z.array(createLoadCaseRequestSchema).optional(),
    update: z.array(updateLoadCaseRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionLoadCaseOperationsRequest" });

const loadCombinationOperationsRequestSchema = z
  .object({
    create: z.array(createLoadCombinationRequestSchema).optional(),
    update: z.array(updateLoadCombinationRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionLoadCombinationOperationsRequest" });

const pointLoadOperationsRequestSchema = z
  .object({
    create: z.array(createPointLoadRequestSchema).optional(),
    update: z.array(updatePointLoadRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionPointLoadOperationsRequest" });

export const createModelRevisionReqSchema = z
  .object({
    params: z.object({
      projectId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
    body: z
      .object({
        element1ds: element1dOperationsRequestSchema.optional(),
        nodes: nodeOperationsRequestSchema.optional(),
        materials: materialOperationsRequestSchema.optional(),
        sectionProfiles: sectionProfileOperationsRequestSchema.optional(),
        loadCases: loadCaseOperationsRequestSchema.optional(),
        loadCombinations: loadCombinationOperationsRequestSchema.optional(),
        pointLoads: pointLoadOperationsRequestSchema.optional(),
      })
      .meta({ id: "CreateModelRevisionRequest" }),
  })
  .meta({ id: "CreateModelRevisionEndpointRequest" });

export const modelRevisionResSchema = z
  .object({
    id: uuidV7Schema,
    projectId: uuidV7Schema,
    parentRevisionId: uuidV7Schema.nullable(),
    secondParentRevisionId: uuidV7Schema.nullable(),
    authorId: z.uuid(),
    message: z.string().min(1),
    createdAt: z.iso.datetime(),
    nodes: z.array(revisionNodeResponseSchema),
    materials: z.array(materialResponseSchema),
    modelSettings: modelSettingsResponseSchema.nullable(),
    sectionProfiles: z.array(sectionProfileResponseSchema),
    element1ds: z.array(element1dResponseSchema),
    loadCases: z.array(loadCaseResponseSchema),
    loadCombinations: z.array(loadCombinationResponseSchema),
    pointLoads: z.array(pointLoadResponseSchema),
  })
  .meta({ id: "ModelRevision" });
