import type { Element1dSnapshot } from "./element1d-entity";

export type Element1dDomainEvent = {
  type: "element1d_created";
  payload: Element1dSnapshot;
};
