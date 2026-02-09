import { bootstrapDb } from "./db/bootstrap";
import { ensureSeedData } from "./db/seed";
import { createServer } from "./server";

await bootstrapDb();
await ensureSeedData();
const server = createServer();

console.log(`API listening on http://127.0.0.1:${server.port}`);
