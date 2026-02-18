import type { LoadCombinationSnapshot } from "./load-combination-entity";

export type LoadCombinationDomainEvent = {
  type: "load_combination_created";
  payload: LoadCombinationSnapshot;
};
