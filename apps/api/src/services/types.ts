import type { User } from "@beamos/contracts";

export type UserRepository = {
  getById: (id: string) => Promise<User | undefined>;
};

export type AppServices = {
  userRepository: UserRepository;
};

export type AppContext = {
  requestId: string;
  services: AppServices;
};
