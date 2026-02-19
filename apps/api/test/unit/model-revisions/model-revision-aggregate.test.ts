import { describe, expect, it } from "bun:test";
import {
  Area,
  AreaMomentOfInertia,
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Pressure,
  PressureUnits,
  Volume,
  VolumeUnits,
  WarpingMomentOfInertia,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { ModelRevisionAggregate } from "../../../src/model-revisions/model-revision-aggregate";

const createAggregate = () => {
  const revisionId = Bun.randomUUIDv7();
  return ModelRevisionAggregate.create({
    id: revisionId,
    projectId: Bun.randomUUIDv7(),
    parentRevisionId: null,
    secondParentRevisionId: null,
    authorId: Bun.randomUUIDv7(),
    message: "Create revision",
    createdAt: new Date(),
    nodes: [],
    materials: [],
    modelSettings: {
      id: Bun.randomUUIDv7(),
      revisionId,
      units: {
        pressure: PressureUnits.Pascals,
        area: AreaUnits.SquareMeters,
        areaMomentOfInertia: AreaMomentOfInertiaUnits.MetersToTheFourth,
        warpingMomentOfInertia:
          WarpingMomentOfInertiaUnits.MetersToTheSixth,
        volume: VolumeUnits.CubicMeters,
      },
      yAxisUp: true,
    },
    sectionProfiles: [],
    element1ds: [],
    loadCases: [],
    loadCombinations: [],
    pointLoads: [],
  });
};

describe("ModelRevisionAggregate", () => {
  it("pulls domain events from added materials", () => {
    const aggregate = createAggregate();

    aggregate.addMaterial({
      id: Bun.randomUUIDv7(),
      revisionId: aggregate.id,
      name: "A36 Steel",
      pressureE: Pressure.FromPascals(1),
      pressureG: Pressure.FromPascals(2),
    });

    const events = aggregate.pullDomainEvents();
    expect(events).toHaveLength(1);
    expect(events[0]?.type).toBe("material_created");
    expect(aggregate.pullDomainEvents()).toHaveLength(0);
  });

  it("pulls domain events from added section profiles", () => {
    const aggregate = createAggregate();

    aggregate.addSectionProfile({
      id: Bun.randomUUIDv7(),
      revisionId: aggregate.id,
      name: "W12x26",
      discriminator: "STANDARD",
      area: Area.FromSquareMeters(1),
      strongAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(2),
      weakAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(3),
      torsionalConstant: AreaMomentOfInertia.FromMetersToTheFourth(4),
      warpingConstant: WarpingMomentOfInertia.FromMetersToTheSixth(5),
      strongAxisPlasticSectionModulus: Volume.FromCubicMeters(6),
      weakAxisPlasticSectionModulus: Volume.FromCubicMeters(7),
      strongAxisElasticSectionModulus: Volume.FromCubicMeters(8),
      weakAxisElasticSectionModulus: Volume.FromCubicMeters(9),
    });

    const events = aggregate.pullDomainEvents();
    expect(events).toHaveLength(1);
    expect(events[0]?.type).toBe("section_profile_created");
    expect(aggregate.pullDomainEvents()).toHaveLength(0);
  });

  it("rejects duplicate material names", () => {
    const aggregate = createAggregate();
    aggregate.addMaterial({
      id: Bun.randomUUIDv7(),
      revisionId: aggregate.id,
      name: "A36 Steel",
      pressureE: Pressure.FromPascals(1),
      pressureG: Pressure.FromPascals(2),
    });

    expect(() =>
      aggregate.addMaterial({
        id: Bun.randomUUIDv7(),
        revisionId: aggregate.id,
        name: "A36 Steel",
        pressureE: Pressure.FromPascals(3),
        pressureG: Pressure.FromPascals(4),
      }),
    ).toThrow('Material name "A36 Steel" already exists');
  });

  it("rejects duplicate section profile names", () => {
    const aggregate = createAggregate();
    aggregate.addSectionProfile({
      id: Bun.randomUUIDv7(),
      revisionId: aggregate.id,
      name: "W12x26",
      discriminator: "STANDARD",
      area: Area.FromSquareMeters(1),
      strongAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(2),
      weakAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(3),
      torsionalConstant: AreaMomentOfInertia.FromMetersToTheFourth(4),
      warpingConstant: WarpingMomentOfInertia.FromMetersToTheSixth(5),
      strongAxisPlasticSectionModulus: Volume.FromCubicMeters(6),
      weakAxisPlasticSectionModulus: Volume.FromCubicMeters(7),
      strongAxisElasticSectionModulus: Volume.FromCubicMeters(8),
      weakAxisElasticSectionModulus: Volume.FromCubicMeters(9),
    });

    expect(() =>
      aggregate.addSectionProfile({
        id: Bun.randomUUIDv7(),
        revisionId: aggregate.id,
        name: "W12x26",
        discriminator: "STANDARD",
        area: Area.FromSquareMeters(10),
        strongAxisMomentOfInertia:
          AreaMomentOfInertia.FromMetersToTheFourth(11),
        weakAxisMomentOfInertia: AreaMomentOfInertia.FromMetersToTheFourth(12),
        torsionalConstant: AreaMomentOfInertia.FromMetersToTheFourth(13),
        warpingConstant: WarpingMomentOfInertia.FromMetersToTheSixth(14),
        strongAxisPlasticSectionModulus: Volume.FromCubicMeters(15),
        weakAxisPlasticSectionModulus: Volume.FromCubicMeters(16),
        strongAxisElasticSectionModulus: Volume.FromCubicMeters(17),
        weakAxisElasticSectionModulus: Volume.FromCubicMeters(18),
      }),
    ).toThrow('Section profile name "W12x26" already exists');
  });
});
