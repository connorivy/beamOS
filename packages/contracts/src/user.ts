import { z } from "zod";

export const getUserReqSchema = z.object({
  params: z.object({ id: z.string() }),
});

export const userSchema = z.object({
  id: z.string(),
  name: z.string(),
});

export const getUserResSchema = userSchema;

export type GetUserReq = z.infer<typeof getUserReqSchema>;
export type User = z.infer<typeof userSchema>;
