import type { SectionProfileSnapshot } from "./section-profile-entity";

export type SectionProfileDomainEvent = {
  type: "section_profile_created";
  payload: SectionProfileSnapshot;
};
