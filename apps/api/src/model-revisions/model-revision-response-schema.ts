import z from "zod";
import { uuidV7Schema } from "../common/uuid";
import { element1dResponseSchema } from "../element1ds/element1d-contract-schemas";
import { materialResponseSchema } from "../materials/material-contract-schemas";
import { revisionNodeResponseSchema } from "../nodes/node-contract-schemas";
import { sectionProfileResponseSchema } from "../section-profiles/section-profile-contract-schemas";

export const modelRevisionResponseSchema = z.object({
  modelRevision: z.object({
    id: uuidV7Schema,
    version: z.object({
      modelId: uuidV7Schema,
      branchName: z.string().trim().min(1),
      revisionId: uuidV7Schema,
      revisionsAhead: z.number().min(0).meta({
        description:
          "Number of revisions ahead of the parent branch. In progress revisions are not included in the number",
      }),
      revisionsBehind: z.number().min(0),
      inProgressRevisionId: uuidV7Schema,
    }),
    modelId: uuidV7Schema,
    name: z.string().min(1),
    parentRevisionId: uuidV7Schema.nullable(),
    secondParentRevisionId: uuidV7Schema.nullable(),
    authorId: z.uuid(),
    message: z.string().min(1),
    createdAt: z.iso.datetime(),
    nodes: z.array(revisionNodeResponseSchema),
    materials: z.array(materialResponseSchema),
    sectionProfiles: z.array(sectionProfileResponseSchema),
    element1ds: z.array(element1dResponseSchema),
  }),
});
