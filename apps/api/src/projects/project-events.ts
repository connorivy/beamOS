import { NodeDomainEvents } from "src/nodes/node-events";
import { ProjectSnapshot } from "./project-aggregate";

export type ProjectDomainEvent =
  | NodeDomainEvents
  | {
      type: "project_renamed";
      payload: ProjectSnapshot;
    }
  | {
      type: "project_created";
      payload: ProjectSnapshot;
    };
