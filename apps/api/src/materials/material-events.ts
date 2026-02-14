import type { MaterialSnapshot } from "./material-entity";

export type MaterialDomainEvent = {
  type: "material_created";
  payload: MaterialSnapshot;
};
