import { NodeDomainEvents } from "src/nodes/node-events";
import { ModelSnapshot } from "./model-aggregate";

export type ModelDomainEvent =
  | NodeDomainEvents
  | {
      type: "model_renamed";
      payload: ModelSnapshot;
    }
  | {
      type: "model_created";
      payload: ModelSnapshot;
    };
