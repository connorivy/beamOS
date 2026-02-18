import type { PointLoadSnapshot } from "./point-load-entity";

export type PointLoadDomainEvent = {
  type: "point_load_created";
  payload: PointLoadSnapshot;
};
