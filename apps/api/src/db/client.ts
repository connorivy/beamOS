import { drizzle } from "drizzle-orm/bun-sql";

const databaseUrl = process.env.DB_URI;

if (!databaseUrl) {
  throw new Error(
    "DATABASE_URL must be set to a PostgreSQL connection string.",
  );
}

export const db = drizzle(databaseUrl);
