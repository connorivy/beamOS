import type { SectionProfileSnapshot } from "./section-profile-aggregate";

export type SectionProfileDomainEvent = {
  type: "section_profile_created";
  payload: SectionProfileSnapshot;
};
