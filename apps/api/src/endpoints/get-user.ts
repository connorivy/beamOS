import { defineEndpoint, getUserReqSchema, getUserResSchema } from "@beamos/contracts";
import { eq } from "drizzle-orm";
import { db } from "../db/client";
import { users } from "../db/schema";

export const getUser = defineEndpoint({
  method: "GET",
  path: "/api/users/:id",
  req: getUserReqSchema,
  res: getUserResSchema,
  async handler(req) {
    const row = await db.select().from(users).where(eq(users.id, req.params.id)).limit(1);
    if (row.length === 0) {
      return { id: req.params.id, name: "Unknown" };
    }

    return row[0];
  },
});
