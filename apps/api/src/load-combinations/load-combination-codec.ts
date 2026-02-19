import { z } from "zod";
import { LoadCombinationEntity } from "./load-combination-entity";

const loadCombinationPayloadV1Schema = z.object({
    loadCaseFactors: z.record(z.string().uuid(), z.number().finite()),
});

type LoadCombinationPayloadV1 = z.infer<typeof loadCombinationPayloadV1Schema>;

export const loadCombinationRevisionChangeCodec = {
    entityType: "loadcombination" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: LoadCombinationEntity): {
        schemaVersion: number;
        payload: LoadCombinationPayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                loadCaseFactors: { ...entity.loadCaseFactors },
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): LoadCombinationEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = loadCombinationPayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return LoadCombinationEntity.rehydrate({
            id: entityId,
            revisionId,
            loadCaseFactors: parseResult.data.loadCaseFactors,
        });
    },
};
