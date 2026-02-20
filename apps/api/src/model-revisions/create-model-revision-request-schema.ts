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
  nodeResponseSchema,
  putNodeRequestSchema as updateNodeRequestSchema,
} from "../nodes/node-contract-schemas";
import {
  createSectionProfileRequestSchema,
  deleteSectionProfileRequestSchema,
  sectionProfileResponseSchema,
  putSectionProfileRequestSchema as updateSectionProfileRequestSchema,
} from "../section-profiles/section-profile-contract-schemas";
import { modelSettingsPropertiesSchema, modelSettingsResponseSchema } from "src/model-settings/model-settings-contract-schemas";

export const element1dOperationsRequestSchema = z
  .object({
    create: z.array(createElement1dRequestSchema).optional(),
    update: z.array(updateElement1dRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionElement1dOperationsRequest" });

export const nodeOperationsRequestSchema = z
  .object({
    create: z.array(createNodeRequestSchema).optional(),
    update: z.array(updateNodeRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionNodeOperationsRequest" });

export const materialOperationsRequestSchema = z
  .object({
    create: z.array(createMaterialRequestSchema).optional(),
    update: z.array(updateMaterialRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionMaterialOperationsRequest" });

export const sectionProfileOperationsRequestSchema = z
  .object({
    create: z.array(createSectionProfileRequestSchema).optional(),
    update: z.array(updateSectionProfileRequestSchema).optional(),
    delete: z.array(deleteSectionProfileRequestSchema).optional(),
  })
  .meta({ id: "CreateModelRevisionSectionProfileOperationsRequest" });

export const loadCaseOperationsRequestSchema = z
  .object({
    create: z.array(createLoadCaseRequestSchema).optional(),
    update: z.array(updateLoadCaseRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionLoadCaseOperationsRequest" });

export const loadCombinationOperationsRequestSchema = z
  .object({
    create: z.array(createLoadCombinationRequestSchema).optional(),
    update: z.array(updateLoadCombinationRequestSchema).optional(),
    delete: z.array(uuidV7Schema).optional(),
  })
  .meta({ id: "CreateModelRevisionLoadCombinationOperationsRequest" });

export const pointLoadOperationsRequestSchema = z
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
        modelSettings: modelSettingsPropertiesSchema.optional(),
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
    nodes: z.array(nodeResponseSchema),
    materials: z.array(materialResponseSchema),
    modelSettings: modelSettingsResponseSchema,
    sectionProfiles: z.array(sectionProfileResponseSchema),
    element1ds: z.array(element1dResponseSchema),
    loadCases: z.array(loadCaseResponseSchema),
    loadCombinations: z.array(loadCombinationResponseSchema),
    pointLoads: z.array(pointLoadResponseSchema),
  })
  .meta({ id: "ModelRevision" });

export type NodeOperationsRequest = z.infer<typeof nodeOperationsRequestSchema>;
export type MaterialOperationsRequest = z.infer<
  typeof materialOperationsRequestSchema
>;
export type SectionProfileOperationsRequest = z.infer<
  typeof sectionProfileOperationsRequestSchema
>;
export type Element1dOperationsRequest = z.infer<
  typeof element1dOperationsRequestSchema
>;
export type LoadCaseOperationsRequest = z.infer<
  typeof loadCaseOperationsRequestSchema
>;
export type LoadCombinationOperationsRequest = z.infer<
  typeof loadCombinationOperationsRequestSchema
>;
export type PointLoadOperationsRequest = z.infer<
  typeof pointLoadOperationsRequestSchema
>;
export type ModelSettingsUpdateRequest = z.infer<typeof modelSettingsPropertiesSchema>;
