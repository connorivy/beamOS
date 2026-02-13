import { drizzleModelRepository } from "./models/model-repository";
import { drizzleModelVersionRepository } from "./model-revisions/model-revision-repository";
import { drizzleMaterialRepository } from "./materials/material-repository";
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
});
