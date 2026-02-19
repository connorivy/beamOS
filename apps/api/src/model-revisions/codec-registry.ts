import { materialRevisionChangeCodec } from "src/materials/material-codec";
import { MaterialEntity } from "src/materials/material-entity";

export const revisionChangeCodecRegistry = {
    material: materialRevisionChangeCodec as ModelEntityCodec<MaterialEntity>,
} as const;

export type ModelEntityCodec<TEntity> = {
    toPersistencePayload(entity: TEntity): {
        schemaVersion: number;
        payload: unknown;
    };
    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): TEntity;
};
