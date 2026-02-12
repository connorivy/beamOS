import { drizzleModelRepository } from "../models/model-repository";
import { drizzleModelVersionRepository } from "../model-revisions/model-revision-repository";
import { drizzleUserRepository } from "../repositories/user-repository";
import type { AppServices } from "./types";

export const createDefaultServices = (): AppServices => ({
  userRepository: drizzleUserRepository,
  modelRepository: drizzleModelRepository,
  modelRevisionRepository: drizzleModelVersionRepository,
});
