import {
  Area,
  AreaMomentOfInertia,
  Volume,
  WarpingMomentOfInertia,
} from "unitsnet-js";
import {
  sectionProfiles,
  type SectionProfile as SectionProfileRow,
} from "../db/schema";
import { SectionProfileAggregate } from "./section-profile-aggregate";

export const sectionProfileMapper = {
  toDomain(row: SectionProfileRow): SectionProfileAggregate {
    return SectionProfileAggregate.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      name: row.name,
      discriminator: row.discriminator,
      area: Area.FromSquareMeters(row.areaSi),
      strongAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(
        row.strongAxisMomentOfInertiaSi,
      ),
      weakAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(
        row.weakAxisMomentOfInertiaSi,
      ),
      torsionalConstant: AreaMomentOfInertia.FromMetersToTheFourth(
        row.torsionalConstantSi,
      ),
      warpingConstant: WarpingMomentOfInertia.FromMetersToTheSixth(
        row.warpingConstantSi,
      ),
      strongAxisPlasticSectionModulus: Volume.FromCubicMeters(
        row.strongAxisPlasticSectionModulusSi,
      ),
      weakAxisPlasticSectionModulus: Volume.FromCubicMeters(
        row.weakAxisPlasticSectionModulusSi,
      ),
      strongAxisElasticSectionModulus: Volume.FromCubicMeters(
        row.strongAxisElasticSectionModulusSi,
      ),
      weakAxisElasticSectionModulus: Volume.FromCubicMeters(
        row.weakAxisElasticSectionModulusSi,
      ),
      strongAxisShearArea:
        row.strongAxisShearAreaSi == null
          ? undefined
          : Area.FromSquareMeters(row.strongAxisShearAreaSi),
      weakAxisShearArea:
        row.weakAxisShearAreaSi == null
          ? undefined
          : Area.FromSquareMeters(row.weakAxisShearAreaSi),
    });
  },

  toPersistence(
    aggregate: SectionProfileAggregate,
  ): typeof sectionProfiles.$inferInsert {
    return {
      id: aggregate.id,
      revisionId: aggregate.revisionId,
      name: aggregate.name,
      discriminator: aggregate.discriminator,
      areaSi: aggregate.area.SquareMeters,
      strongAxisMomentOfInertiaSi:
        aggregate.strongAxisMomentOfInertia.MetersToTheFourth,
      weakAxisMomentOfInertiaSi: aggregate.weakAxisMomentOfInertia.MetersToTheFourth,
      torsionalConstantSi: aggregate.torsionalConstant.MetersToTheFourth,
      warpingConstantSi: aggregate.warpingConstant.MetersToTheSixth,
      strongAxisPlasticSectionModulusSi:
        aggregate.strongAxisPlasticSectionModulus.CubicMeters,
      weakAxisPlasticSectionModulusSi:
        aggregate.weakAxisPlasticSectionModulus.CubicMeters,
      strongAxisElasticSectionModulusSi:
        aggregate.strongAxisElasticSectionModulus.CubicMeters,
      weakAxisElasticSectionModulusSi:
        aggregate.weakAxisElasticSectionModulus.CubicMeters,
      strongAxisShearAreaSi: aggregate.strongAxisShearArea?.SquareMeters,
      weakAxisShearAreaSi: aggregate.weakAxisShearArea?.SquareMeters,
    };
  },
};
