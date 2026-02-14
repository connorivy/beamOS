import { drizzleModelRepository } from "./models/model-repository";
import { drizzleModelVersionRepository } from "./model-revisions/model-revision-repository";
import { drizzleMaterialRepository } from "./materials/material-repository";
import { drizzleSectionProfileRepository } from "./section-profiles/section-profile-repository";
import { drizzleElement1dRepository } from "./element1ds/element1d-repository";
import { drizzleRevisionChangeRepository } from "./revision-changes/revision-change-repository";
import type { AppServices } from "./common/types";

export const createDefaultServices = (): AppServices => ({
  userRepository: {
    async getById() {
      return undefined;
    },
  },
  modelRepository: drizzleModelRepository,
  modelRevisionRepository: drizzleModelVersionRepository,
  materialRepository: drizzleMaterialRepository,
  sectionProfileRepository: drizzleSectionProfileRepository,
  element1dRepository: drizzleElement1dRepository,
  revisionChangeRepository: drizzleRevisionChangeRepository,
});
