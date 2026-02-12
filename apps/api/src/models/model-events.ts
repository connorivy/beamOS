import type { NodeSnapshot } from "./node-entity";

export type ModelDomainEvent =
  | {
      type: "node_added";
      node: NodeSnapshot;
    };
