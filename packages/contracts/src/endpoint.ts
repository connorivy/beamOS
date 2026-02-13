import { z } from "zod";

export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export type DefaultEndpointContext = {
  requestId: string;
};

export type Endpoint<
  ReqSchema extends z.ZodTypeAny,
  ResSchema extends z.ZodTypeAny,
  Ctx = DefaultEndpointContext,
> = {
  method: HttpMethod;
  path: string;
  req: ReqSchema;
  res: ResSchema;
  handler: (
    req: z.infer<ReqSchema>,
    ctx: Ctx,
  ) => Promise<z.infer<ResSchema>> | z.infer<ResSchema>;
};

export const defineEndpoint = <
  ReqSchema extends z.ZodTypeAny,
  ResSchema extends z.ZodTypeAny,
  Ctx = DefaultEndpointContext,
>(
  e: Endpoint<ReqSchema, ResSchema, Ctx>,
) => e;
