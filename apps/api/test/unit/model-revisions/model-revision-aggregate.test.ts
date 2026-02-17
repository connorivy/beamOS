import { describe, expect, it } from "bun:test";
import {
  Area,
  AreaMomentOfInertia,
  Pressure,
  Volume,
  WarpingMomentOfInertia,
} from "unitsnet-js";
import { ModelRevisionAggregate } from "../../../src/model-revisions/model-revision-aggregate";

describe("ModelRevisionAggregate", () => {
  it("pulls domain events from added materials", () => {
    const aggregate = ModelRevisionAggregate.create({
      modelId: Bun.randomUUIDv7(),
      branchName: "main",
      name: "Revision",
      parentRevisionId: null,
      secondParentRevisionId: null,
      authorId: Bun.randomUUIDv7(),
      message: "Create revision",
      createdAt: new Date(),
      nodes: [],
      materials: [],
      sectionProfiles: [],
      element1ds: [],
    });

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
    const aggregate = ModelRevisionAggregate.create({
      modelId: Bun.randomUUIDv7(),
      branchName: "main",
      name: "Revision",
      parentRevisionId: null,
      secondParentRevisionId: null,
      authorId: Bun.randomUUIDv7(),
      message: "Create revision",
      createdAt: new Date(),
      nodes: [],
      materials: [],
      sectionProfiles: [],
      element1ds: [],
    });

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
});
