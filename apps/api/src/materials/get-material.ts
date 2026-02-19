import { defineEndpoint } from "../contracts/endpoint";
import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { materialResponseSchema } from "./material-contract-schemas";

const uuidV7Schema = z.uuid().refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getMaterialReqSchema = z
    .object({
        params: z.object({
            projectId: uuidV7Schema,
            branchName: z.string().trim().min(1),
            materialId: uuidV7Schema,
        }),
    })
    .meta({ id: "GetMaterialRequest" });

export const getMaterial = defineEndpoint({
    method: "GET",
    path: "/api/projects/:projectId/branches/:branchName/materials/:materialId",
    req: getMaterialReqSchema,
    res: materialResponseSchema,
    async handler(req, ctx: AppContext) {
        const modelRevision = await ctx.services.modelRevisionRepository.load(
            req.params.projectId,
            req.params.branchName,
        );
        if (!modelRevision) {
            throw httpError("Model revision not found", 404);
        }

        const material = modelRevision.getMaterialById(req.params.materialId);

        if (!material) {
            throw httpError("Material not found", 404);
        }

        return {
            id: material.id,
            revisionId: material.revisionId,
            name: material.name,
            modulusOfElasticity: material.modulusOfElasticity.Pascals,
            modulusOfRigidity: material.modulusOfRigidity.Pascals,
            units: {
                pressure: PressureUnits.Pascals as const,
            },
        };
    },
});
