import { z } from "zod";
import {
    Area,
    AreaMomentOfInertia,
    Volume,
    WarpingMomentOfInertia,
} from "unitsnet-js";
import {
    SECTION_PROFILE_DISCRIMINATORS,
    SectionProfileEntity,
} from "./section-profile-entity";

const sectionProfilePayloadV1Schema = z.object({
    name: z.string().trim().min(1),
    discriminator: z.enum(SECTION_PROFILE_DISCRIMINATORS),
    area: z.number().finite(),
    strongAxisMomentOfInertia: z.number().finite(),
    weakAxisMomentOfInertia: z.number().finite(),
    torsionalConstant: z.number().finite(),
    warpingConstant: z.number().finite(),
    strongAxisPlasticSectionModulus: z.number().finite(),
    weakAxisPlasticSectionModulus: z.number().finite(),
    strongAxisElasticSectionModulus: z.number().finite(),
    weakAxisElasticSectionModulus: z.number().finite(),
    strongAxisShearArea: z.number().finite().optional(),
    weakAxisShearArea: z.number().finite().optional(),
});

type SectionProfilePayloadV1 = z.infer<typeof sectionProfilePayloadV1Schema>;

export const sectionProfileRevisionChangeCodec = {
    entityType: "section_profile" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: SectionProfileEntity): {
        schemaVersion: number;
        payload: SectionProfilePayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                name: entity.name,
                discriminator: entity.discriminator,
                area: entity.area.SquareMeters,
                strongAxisMomentOfInertia: entity.strongAxisMomentOfInertia.MetersToTheFourth,
                weakAxisMomentOfInertia: entity.weakAxisMomentOfInertia.MetersToTheFourth,
                torsionalConstant: entity.torsionalConstant.MetersToTheFourth,
                warpingConstant: entity.warpingConstant.MetersToTheSixth,
                strongAxisPlasticSectionModulus: entity.strongAxisPlasticSectionModulus.CubicMeters,
                weakAxisPlasticSectionModulus: entity.weakAxisPlasticSectionModulus.CubicMeters,
                strongAxisElasticSectionModulus: entity.strongAxisElasticSectionModulus.CubicMeters,
                weakAxisElasticSectionModulus: entity.weakAxisElasticSectionModulus.CubicMeters,
                ...(entity.strongAxisShearArea
                    ? { strongAxisShearArea: entity.strongAxisShearArea.SquareMeters }
                    : {}),
                ...(entity.weakAxisShearArea
                    ? { weakAxisShearArea: entity.weakAxisShearArea.SquareMeters }
                    : {}),
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): SectionProfileEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = sectionProfilePayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return SectionProfileEntity.rehydrate({
            id: entityId,
            revisionId,
            name: parseResult.data.name,
            discriminator: parseResult.data.discriminator,
            area: Area.FromSquareMeters(parseResult.data.area),
            strongAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(
                parseResult.data.strongAxisMomentOfInertia,
            ),
            weakAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(
                parseResult.data.weakAxisMomentOfInertia,
            ),
            torsionalConstant: AreaMomentOfInertia.FromMetersToTheFourth(
                parseResult.data.torsionalConstant,
            ),
            warpingConstant: WarpingMomentOfInertia.FromMetersToTheSixth(
                parseResult.data.warpingConstant,
            ),
            strongAxisPlasticSectionModulus: Volume.FromCubicMeters(
                parseResult.data.strongAxisPlasticSectionModulus,
            ),
            weakAxisPlasticSectionModulus: Volume.FromCubicMeters(
                parseResult.data.weakAxisPlasticSectionModulus,
            ),
            strongAxisElasticSectionModulus: Volume.FromCubicMeters(
                parseResult.data.strongAxisElasticSectionModulus,
            ),
            weakAxisElasticSectionModulus: Volume.FromCubicMeters(
                parseResult.data.weakAxisElasticSectionModulus,
            ),
            ...(parseResult.data.strongAxisShearArea !== undefined
                ? {
                      strongAxisShearArea: Area.FromSquareMeters(
                          parseResult.data.strongAxisShearArea,
                      ),
                  }
                : {}),
            ...(parseResult.data.weakAxisShearArea !== undefined
                ? {
                      weakAxisShearArea: Area.FromSquareMeters(parseResult.data.weakAxisShearArea),
                  }
                : {}),
        });
    },
};
