import { z } from "zod";
import {
    AreaMomentOfInertiaUnits,
    AreaUnits,
    PressureUnits,
    VolumeUnits,
    WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { ModelSettingsEntity } from "./model-settings-entity";

const modelSettingsPayloadV1Schema = z.object({
    units: z.object({
        pressure: z.enum(PressureUnits),
        area: z.enum(AreaUnits),
        areaMomentOfInertia: z.enum(AreaMomentOfInertiaUnits),
        warpingMomentOfInertia: z.enum(WarpingMomentOfInertiaUnits),
        volume: z.enum(VolumeUnits),
    }),
    yAxisUp: z.boolean(),
});

type ModelSettingsPayloadV1 = z.infer<typeof modelSettingsPayloadV1Schema>;

export const modelSettingsRevisionChangeCodec = {
    entityType: "model_settings" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: ModelSettingsEntity): {
        schemaVersion: number;
        payload: ModelSettingsPayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                units: entity.units,
                yAxisUp: entity.yAxisUp,
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): ModelSettingsEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = modelSettingsPayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return ModelSettingsEntity.rehydrate({
            id: entityId,
            revisionId,
            units: parseResult.data.units,
            yAxisUp: parseResult.data.yAxisUp,
        });
    },
};
