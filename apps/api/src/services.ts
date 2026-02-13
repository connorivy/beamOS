import { drizzleModelRepository } from "./models/model-repository";
import { drizzleModelVersionRepository } from "./model-revisions/model-revision-repository";
import type { AppServices } from "./common/types";

export const createDefaultServices = (): AppServices => ({
  modelRepository: drizzleModelRepository,
  modelRevisionRepository: drizzleModelVersionRepository,
});
