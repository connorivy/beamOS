import { drizzle } from "drizzle-orm/bun-sql";

let client: ReturnType<typeof drizzle> | undefined;
let clientUri: string | undefined;

export const getDb = () => {
  const databaseUrl = process.env.DB_URI;

  if (!databaseUrl) {
    throw new Error("DB_URI must be set to a PostgreSQL connection string.");
  }

  if (!client || clientUri !== databaseUrl) {
    client = drizzle(databaseUrl);
    clientUri = databaseUrl;
  }

  return client;
};
