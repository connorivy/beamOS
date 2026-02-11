import { z } from "zod";

export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export type DefaultEndpointContext = {
  requestId: string;
};

export type Endpoint<Req, Res, Ctx = DefaultEndpointContext> = {
  method: HttpMethod;
  path: string;
  req: z.ZodType<Req>;
  res: z.ZodType<Res>;
  handler: (req: Req, ctx: Ctx) => Promise<Res> | Res;
};

export const defineEndpoint = <Req, Res, Ctx = DefaultEndpointContext>(
  e: Endpoint<Req, Res, Ctx>,
) => e;

export type ReqOf<T> = T extends Endpoint<infer Req, unknown, unknown> ? Req : never;
export type ResOf<T> = T extends Endpoint<unknown, infer Res, unknown> ? Res : never;
