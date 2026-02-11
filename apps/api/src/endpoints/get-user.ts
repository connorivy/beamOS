import { defineEndpoint, getUserReqSchema, getUserResSchema } from "@beamos/contracts";
import type { AppContext } from "../services/types";

export const getUser = defineEndpoint({
  method: "GET",
  path: "/api/users/:id",
  req: getUserReqSchema,
  res: getUserResSchema,
  async handler(req, ctx: AppContext) {
    const user = await ctx.services.userRepository.getById(req.params.id);
    if (!user) {
      return { id: req.params.id, name: "Unknown" };
    }

    return user;
  },
});
