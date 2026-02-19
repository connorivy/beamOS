import { element1dRevisionChangeCodec } from "src/element1ds/element1d-codec";
import { Element1dEntity } from "src/element1ds/element1d-entity";
import { loadCaseRevisionChangeCodec } from "src/load-cases/load-case-codec";
import { LoadCaseEntity } from "src/load-cases/load-case-entity";
import { loadCombinationRevisionChangeCodec } from "src/load-combinations/load-combination-codec";
import { LoadCombinationEntity } from "src/load-combinations/load-combination-entity";
import { materialRevisionChangeCodec } from "src/materials/material-codec";
import { MaterialEntity } from "src/materials/material-entity";
import { modelSettingsRevisionChangeCodec } from "src/model-settings/model-settings-codec";
import { ModelSettingsEntity } from "src/model-settings/model-settings-entity";
import { nodeRevisionChangeCodec } from "src/nodes/node-codec";
import { NodeEntity } from "src/nodes/node-entity";
import { pointLoadRevisionChangeCodec } from "src/point-loads/point-load-codec";
import { PointLoadEntity } from "src/point-loads/point-load-entity";
import { sectionProfileRevisionChangeCodec } from "src/section-profiles/section-profile-codec";
import { SectionProfileEntity } from "src/section-profiles/section-profile-entity";

export type ModelEntityCodec<TEntity, TEntityType extends string = string> = {
    entityType: TEntityType;
    currentSchemaVersion: number;
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

export const revisionChangeCodecRegistry = {
    node: nodeRevisionChangeCodec as ModelEntityCodec<NodeEntity, "node">,
    material: materialRevisionChangeCodec as ModelEntityCodec<MaterialEntity, "material">,
    model_settings: modelSettingsRevisionChangeCodec as ModelEntityCodec<
        ModelSettingsEntity,
        "model_settings"
    >,
    section_profile: sectionProfileRevisionChangeCodec as ModelEntityCodec<
        SectionProfileEntity,
        "section_profile"
    >,
    element1d: element1dRevisionChangeCodec as ModelEntityCodec<Element1dEntity, "element1d">,
    loadcase: loadCaseRevisionChangeCodec as ModelEntityCodec<LoadCaseEntity, "loadcase">,
    loadcombination: loadCombinationRevisionChangeCodec as ModelEntityCodec<
        LoadCombinationEntity,
        "loadcombination"
    >,
    pointload: pointLoadRevisionChangeCodec as ModelEntityCodec<PointLoadEntity, "pointload">,
} as const;
