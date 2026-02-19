import { defineEndpoint } from "../contracts/endpoint";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { element1dResponseSchema } from "./element1d-contract-schemas";

const uuidV7Schema = z.uuid().refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getElement1dReqSchema = z
    .object({
        params: z.object({
            projectId: uuidV7Schema,
            branchName: z.string(),
            element1dId: uuidV7Schema,
        }),
    })
    .meta({ id: "GetElement1dRequest" });

export const getElement1dResSchema = z
    .object({
        element1d: element1dResponseSchema,
    })
    .meta({ id: "GetElement1dResponse" });

export const getElement1d = defineEndpoint({
    method: "GET",
    path: "/api/projects/:projectId/branches/:branchName/element1ds/:element1dId",
    req: getElement1dReqSchema,
    res: getElement1dResSchema,
    async handler(req, ctx: AppContext) {
        const modelRevision = await ctx.services.modelRevisionRepository.load(
            req.params.projectId,
            req.params.branchName,
        );
        if (!modelRevision) {
            throw httpError("Model revision not found", 404);
        }

        const element1d = modelRevision.getElement1dById(req.params.element1dId);

        if (!element1d) {
            throw httpError("Element1d not found", 404);
        }

        return {
            element1d: {
                id: element1d.id,
                revisionId: element1d.revisionId,
                startNodeId: element1d.startNodeId,
                endNodeId: element1d.endNodeId,
                materialId: element1d.materialId,
                sectionProfileId: element1d.sectionProfileId,
            },
        };
    },
});
