import { z } from "zod";
import { MaterialEntity } from "./material-entity";
import { PressureUnits } from "unitsnet-js";

const materialPayloadV1Schema = z.object({
    name: z.string(),
    modulusOfElasticity: z.number(),
    modulusOfRigidity: z.number(),
    applicationId: z.string().trim().min(1).optional(),
});
type MaterialPayloadV1 = z.infer<typeof materialPayloadV1Schema>;

export const materialRevisionChangeCodec = {
    entityType: "material" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: MaterialEntity): {
        schemaVersion: number;
        payload: MaterialPayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                name: entity.name,
                modulusOfElasticity: entity.modulusOfElasticity.Pascals,
                modulusOfRigidity: entity.modulusOfRigidity.Pascals,
                applicationId: entity.applicationId,
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): MaterialEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = materialPayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }
        return MaterialEntity.rehydrate(
            { ...parseResult.data, units: { pressure: PressureUnits.Pascals } },
            revisionId,
            entityId,
        );
    },
};
