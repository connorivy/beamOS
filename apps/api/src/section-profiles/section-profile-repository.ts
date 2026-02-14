import { and, desc, eq } from "drizzle-orm";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { revisionChanges } from "../db/schema";
import { SectionProfileAggregate } from "./section-profile-aggregate";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import {
  Area,
  AreaMomentOfInertia,
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Volume,
  VolumeUnits,
  WarpingMomentOfInertia,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";

const sectionProfilePayloadSchema = z.object({
  id: z.uuid(),
  revisionId: z.uuid(),
  name: z.string().trim().min(1),
  discriminator: z.enum(["STANDARD", "WITH_SHEAR_AREAS"]),
  area: z.object({
    value: z.number().finite(),
    unit: z.enum(AreaUnits),
  }),
  strongAxisMomentOfInertia: z.object({
    value: z.number().finite(),
    unit: z.enum(AreaMomentOfInertiaUnits),
  }),
  weakAxisMomentOfInertia: z.object({
    value: z.number().finite(),
    unit: z.enum(AreaMomentOfInertiaUnits),
  }),
  torsionalConstant: z.object({
    value: z.number().finite(),
    unit: z.enum(AreaMomentOfInertiaUnits),
  }),
  warpingConstant: z.object({
    value: z.number().finite(),
    unit: z.enum(WarpingMomentOfInertiaUnits),
  }),
  strongAxisPlasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.enum(VolumeUnits),
  }),
  weakAxisPlasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.enum(VolumeUnits),
  }),
  strongAxisElasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.enum(VolumeUnits),
  }),
  weakAxisElasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.enum(VolumeUnits),
  }),
  strongAxisShearArea: z
    .object({
      value: z.number().finite(),
      unit: z.enum(AreaUnits),
    })
    .optional(),
  weakAxisShearArea: z
    .object({
      value: z.number().finite(),
      unit: z.enum(AreaUnits),
    })
    .optional(),
});

export type SectionProfileRepository = {
  batchCreate: (
    tx: DbTransaction,
    input: SectionProfileAggregate[],
  ) => Promise<SectionProfileAggregate[]>;
  getById: (
    sectionProfileId: string,
  ) => Promise<SectionProfileAggregate | undefined>;
};

export const drizzleSectionProfileRepository: SectionProfileRepository = {
  async batchCreate(tx, input) {
    if (input.length === 0) {
      return [];
    }

    const now = new Date();
    const changeRows = input.flatMap((sectionProfile) =>
      sectionProfile.pullDomainEvents().map((event) =>
        revisionChangeMapper.toPersistence(
          RevisionChangeEntity.create({
            id: Bun.randomUUIDv7(),
            revisionId: event.payload.revisionId,
            draftId: null,
            entityType: "sectionprofile",
            entityId: event.payload.id,
            schemaVersion: 1,
            op: "insert",
            payload: {
              id: event.payload.id,
              revisionId: event.payload.revisionId,
              name: event.payload.name,
              discriminator: event.payload.discriminator,
              area: {
                value: event.payload.area.SquareMeters,
                unit: AreaUnits.SquareMeters,
              },
              strongAxisMomentOfInertia: {
                value: event.payload.strongAxisMomentOfInertia.MetersToTheFourth,
                unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
              },
              weakAxisMomentOfInertia: {
                value: event.payload.weakAxisMomentOfInertia.MetersToTheFourth,
                unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
              },
              torsionalConstant: {
                value: event.payload.torsionalConstant.MetersToTheFourth,
                unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
              },
              warpingConstant: {
                value: event.payload.warpingConstant.MetersToTheSixth,
                unit: WarpingMomentOfInertiaUnits.MetersToTheSixth,
              },
              strongAxisPlasticSectionModulus: {
                value: event.payload.strongAxisPlasticSectionModulus.CubicMeters,
                unit: VolumeUnits.CubicMeters,
              },
              weakAxisPlasticSectionModulus: {
                value: event.payload.weakAxisPlasticSectionModulus.CubicMeters,
                unit: VolumeUnits.CubicMeters,
              },
              strongAxisElasticSectionModulus: {
                value: event.payload.strongAxisElasticSectionModulus.CubicMeters,
                unit: VolumeUnits.CubicMeters,
              },
              weakAxisElasticSectionModulus: {
                value: event.payload.weakAxisElasticSectionModulus.CubicMeters,
                unit: VolumeUnits.CubicMeters,
              },
              ...(event.payload.strongAxisShearArea
                ? {
                    strongAxisShearArea: {
                      value: event.payload.strongAxisShearArea.SquareMeters,
                      unit: AreaUnits.SquareMeters,
                    },
                  }
                : {}),
              ...(event.payload.weakAxisShearArea
                ? {
                    weakAxisShearArea: {
                      value: event.payload.weakAxisShearArea.SquareMeters,
                      unit: AreaUnits.SquareMeters,
                    },
                  }
                : {}),
            },
            createdAt: now,
          }),
        ),
      ),
    );

    if (changeRows.length > 0) {
      await tx.insert(revisionChanges).values(changeRows).onConflictDoNothing();
    }

    return input;
  },

  async getById(sectionProfileId) {
    const rows = await getDb()
      .select()
      .from(revisionChanges)
      .where(
        and(
          eq(revisionChanges.entityType, "sectionprofile"),
          eq(revisionChanges.entityId, sectionProfileId),
        ),
      )
      .orderBy(desc(revisionChanges.createdAt), desc(revisionChanges.id))
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    const latestChange = rows[0];
    if (latestChange.op === "delete") {
      return undefined;
    }

    const payload = sectionProfilePayloadSchema.parse(latestChange.payload);
    return SectionProfileAggregate.rehydrate({
      id: payload.id,
      revisionId: payload.revisionId,
      name: payload.name,
      discriminator: payload.discriminator,
      area: new Area(payload.area.value, payload.area.unit),
      strongAxisMomentOfInertia: new AreaMomentOfInertia(
        payload.strongAxisMomentOfInertia.value,
        payload.strongAxisMomentOfInertia.unit,
      ),
      weakAxisMomentOfInertia: new AreaMomentOfInertia(
        payload.weakAxisMomentOfInertia.value,
        payload.weakAxisMomentOfInertia.unit,
      ),
      torsionalConstant: new AreaMomentOfInertia(
        payload.torsionalConstant.value,
        payload.torsionalConstant.unit,
      ),
      warpingConstant: new WarpingMomentOfInertia(
        payload.warpingConstant.value,
        payload.warpingConstant.unit,
      ),
      strongAxisPlasticSectionModulus: new Volume(
        payload.strongAxisPlasticSectionModulus.value,
        payload.strongAxisPlasticSectionModulus.unit,
      ),
      weakAxisPlasticSectionModulus: new Volume(
        payload.weakAxisPlasticSectionModulus.value,
        payload.weakAxisPlasticSectionModulus.unit,
      ),
      strongAxisElasticSectionModulus: new Volume(
        payload.strongAxisElasticSectionModulus.value,
        payload.strongAxisElasticSectionModulus.unit,
      ),
      weakAxisElasticSectionModulus: new Volume(
        payload.weakAxisElasticSectionModulus.value,
        payload.weakAxisElasticSectionModulus.unit,
      ),
      ...(payload.strongAxisShearArea
        ? {
            strongAxisShearArea: new Area(
              payload.strongAxisShearArea.value,
              payload.strongAxisShearArea.unit,
            ),
          }
        : {}),
      ...(payload.weakAxisShearArea
        ? {
            weakAxisShearArea: new Area(
              payload.weakAxisShearArea.value,
              payload.weakAxisShearArea.unit,
            ),
          }
        : {}),
    });
  },
};
