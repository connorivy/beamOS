import { bootstrapDb } from "./bootstrap";

await bootstrapDb();
console.log("Database schema is up to date.");
