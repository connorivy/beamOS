import { defineConfig } from "drizzle-kit";

const databaseUrl = process.env.DB_URI;

if (!databaseUrl) {
  throw new Error(
    "DATABASE_URL must be set to a PostgreSQL connection string.",
  );
}

export default defineConfig({
  out: "./drizzle",
  schema: "./src/db/schema.ts",
  dialect: "postgresql",
  dbCredentials: {
    url: databaseUrl,
  },
});
