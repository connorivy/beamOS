import { describe, expect, it } from "bun:test";
import { Pressure } from "unitsnet-js";
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
      pressureE: Pressure.FromPascals(1),
      pressureG: Pressure.FromPascals(2),
    });

    const events = aggregate.pullDomainEvents();
    expect(events).toHaveLength(1);
    expect(events[0]?.type).toBe("material_created");
    expect(aggregate.pullDomainEvents()).toHaveLength(0);
  });
});
