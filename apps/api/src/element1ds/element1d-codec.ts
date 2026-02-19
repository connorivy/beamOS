import { z } from "zod";
import { Element1dEntity } from "./element1d-entity";

const element1dPayloadV1Schema = z.object({
    startNodeId: z.string().uuid(),
    endNodeId: z.string().uuid(),
    materialId: z.string().uuid(),
    sectionProfileId: z.string().uuid(),
});

type Element1dPayloadV1 = z.infer<typeof element1dPayloadV1Schema>;

export const element1dRevisionChangeCodec = {
    entityType: "element1d" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: Element1dEntity): {
        schemaVersion: number;
        payload: Element1dPayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                startNodeId: entity.startNodeId,
                endNodeId: entity.endNodeId,
                materialId: entity.materialId,
                sectionProfileId: entity.sectionProfileId,
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): Element1dEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = element1dPayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return Element1dEntity.rehydrate({
            id: entityId,
            revisionId,
            startNodeId: parseResult.data.startNodeId,
            endNodeId: parseResult.data.endNodeId,
            materialId: parseResult.data.materialId,
            sectionProfileId: parseResult.data.sectionProfileId,
        });
    },
};
