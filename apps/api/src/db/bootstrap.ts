import { sql } from "drizzle-orm";
import { db } from "./client";

export const bootstrapDb = async () => {
  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS users (
      id TEXT PRIMARY KEY NOT NULL,
      name TEXT NOT NULL
    );
  `);
};
