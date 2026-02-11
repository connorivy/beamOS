import { drizzleModelRepository } from "../models/model-repository";
import { drizzleModelVersionRepository } from "../model-revisions/model-version-repository";
import { drizzleNodeRepository } from "../nodes/node-repository";
import { drizzleUserRepository } from "../repositories/user-repository";
import type { AppServices } from "./types";

export const createDefaultServices = (): AppServices => ({
  userRepository: drizzleUserRepository,
  modelRepository: drizzleModelRepository,
  nodeRepository: drizzleNodeRepository,
  modelVersionRepository: drizzleModelVersionRepository,
});
