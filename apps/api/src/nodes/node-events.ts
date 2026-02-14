import type { NodeSnapshot } from "./node-entity";

export type NodeDomainEvents =
  | {
      type: "node_created";
      payload: NodeSnapshot;
    }
  | {
      type: "node_added";
      payload: NodeSnapshot;
    }
  | {
      type: "node_updated";
      payload: NodeSnapshot;
    }
  | {
      type: "node_deleted";
      payload: NodeSnapshot;
    };
