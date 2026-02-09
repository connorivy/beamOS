import { eq } from "drizzle-orm";
import { db } from "./client";
import { users } from "./schema";

export const ensureSeedData = async () => {
  const existing = await db.select().from(users).where(eq(users.id, "1")).limit(1);
  if (existing.length === 0) {
    await db.insert(users).values({ id: "1", name: "Ada Lovelace" });
  }
};
