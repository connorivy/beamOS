import { drizzleUserRepository } from "../repositories/user-repository";
import type { AppServices } from "./types";

export const createDefaultServices = (): AppServices => ({
  userRepository: drizzleUserRepository,
});
