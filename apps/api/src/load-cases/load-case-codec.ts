import { z } from "zod";
import { LoadCaseEntity } from "./load-case-entity";

const loadCasePayloadV1Schema = z.object({
    name: z.string().trim().min(1),
});

type LoadCasePayloadV1 = z.infer<typeof loadCasePayloadV1Schema>;

export const loadCaseRevisionChangeCodec = {
    entityType: "loadcase" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: LoadCaseEntity): {
        schemaVersion: number;
        payload: LoadCasePayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                name: entity.name,
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): LoadCaseEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = loadCasePayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return LoadCaseEntity.rehydrate({
            id: entityId,
            revisionId,
            name: parseResult.data.name,
        });
    },
};
