import { describe, expect, it } from "bun:test";
import { ModelAggregate } from "../../../src/models/model-aggregate";

describe("ModelAggregate", () => {
  it("adds a model_created event on create", () => {
    const model = ModelAggregate.create({
      name: "New Model",
      description: "A description",
    });

    const events = model.pullDomainEvents();
    expect(events).toHaveLength(1);
    expect(events[0]).toMatchObject({
      type: "model_created",
      payload: {
        id: model.id,
        name: "New Model",
        description: "A description",
      },
    });
    expect(model.pullDomainEvents()).toHaveLength(0);
  });
});
