import { z } from "zod";

export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export type Endpoint<Req, Res> = {
  method: HttpMethod;
  path: string;
  req: z.ZodType<Req>;
  res: z.ZodType<Res>;
  handler: (req: Req, ctx: { requestId: string }) => Promise<Res> | Res;
};

export const defineEndpoint = <Req, Res>(e: Endpoint<Req, Res>) => e;

export type ReqOf<T> = T extends Endpoint<infer Req, unknown> ? Req : never;
export type ResOf<T> = T extends Endpoint<unknown, infer Res> ? Res : never;
