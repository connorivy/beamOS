import { eq } from "drizzle-orm";
import { db } from "../db/client";
import { users } from "../db/schema";
import type { UserRepository } from "../services/types";

export const drizzleUserRepository: UserRepository = {
  async getById(id) {
    const row = await db.select().from(users).where(eq(users.id, id)).limit(1);
    return row[0];
  },
};
