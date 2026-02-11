import type { Endpoint } from "@beamos/contracts";
import { ZodError } from "zod";
import { matchPath } from "./path";

const json = (data: unknown, status = 200) =>
  Response.json(data, {
    status,
    headers: {
      "x-powered-by": "beamos-api",
    },
  });

const toErrorBody = (message: string, issues?: unknown) => ({
  error: message,
  issues,
});

export const runEndpoints = async <Ctx extends { requestId: string }>(
  req: Request,
  endpoints: Endpoint<unknown, unknown, Ctx>[],
  context: Ctx,
): Promise<Response> => {
  const url = new URL(req.url);

  for (const endpoint of endpoints) {
    if (endpoint.method !== req.method) {
      continue;
    }

    const params = matchPath(endpoint.path, url.pathname);
    if (!params) {
      continue;
    }

    let body: unknown = undefined;
    if (req.method !== "GET" && req.method !== "DELETE") {
      body = await req.json().catch(() => undefined);
    }

    try {
      const parsedReq = endpoint.req.parse({
        params,
        body,
        query: Object.fromEntries(url.searchParams.entries()),
      });

      const result = await endpoint.handler(parsedReq, context);
      const parsedRes = endpoint.res.parse(result);
      return json(parsedRes);
    } catch (error) {
      if (error instanceof ZodError) {
        return json(toErrorBody("Validation failed", error.issues), 400);
      }

      if (error instanceof Error) {
        const status = (error as Error & { status?: unknown }).status;
        if (typeof status === "number") {
          return json(toErrorBody(error.message), status);
        }
      }

      return json(toErrorBody("Internal server error"), 500);
    }
  }

  return json(toErrorBody("Not found"), 404);
};
