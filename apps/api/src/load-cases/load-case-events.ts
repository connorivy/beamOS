import type { LoadCaseSnapshot } from "./load-case-entity";

export type LoadCaseDomainEvent = {
  type: "load_case_created";
  payload: LoadCaseSnapshot;
};
