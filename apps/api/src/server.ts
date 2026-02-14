import type { Endpoint } from "./contracts/endpoint";
import { Elysia } from "elysia";
import { fromTypes, openapi } from "@elysiajs/openapi";
import * as z from "zod";
import { zodToJsonSchema } from "zod-to-json-schema";
import { bootstrapDb } from "./db/bootstrap";
import { apiPlugins } from "./plugins/registry";
import { buildServices, collectPluginEndpoints } from "./plugins/types";
import { createDefaultServices } from "./services";

const endpoints = collectPluginEndpoints(apiPlugins);
const services = buildServices(createDefaultServices(), apiPlugins);

const zodToOpenApiJsonSchema = (schema: unknown) => {
  if (
    typeof schema === "object" &&
    schema !== null &&
    "_zod" in (schema as object) &&
    typeof (z as { toJSONSchema?: unknown }).toJSONSchema === "function"
  ) {
    return (z as { toJSONSchema: (value: unknown) => unknown }).toJSONSchema(
      schema,
    );
  }

  return zodToJsonSchema(schema as never, {
    target: "openApi3",
    $refStrategy: "none",
  });
};

const toErrorBody = (message: string, issues?: unknown) => ({
  error: message,
  issues,
});

const isZodErrorLike = (error: unknown): error is { issues: unknown } =>
  typeof error === "object" &&
  error !== null &&
  Array.isArray((error as { issues?: unknown }).issues);

const getSchemaShape = (
  schema: unknown,
): Partial<Record<"params" | "query" | "body", z.ZodTypeAny>> => {
  if (typeof schema !== "object" || schema === null) {
    return {};
  }

  const candidate = schema as {
    shape?:
      | Partial<Record<"params" | "query" | "body", z.ZodTypeAny>>
      | (() => Partial<Record<"params" | "query" | "body", z.ZodTypeAny>>);
  };

  if (typeof candidate.shape === "function") {
    return candidate.shape();
  }

  if (candidate.shape && typeof candidate.shape === "object") {
    return candidate.shape;
  }

  return {};
};

type AnyEndpoint = Endpoint<
  z.ZodType<any, any, any>,
  z.ZodType<any, any, any>,
  any
>;

const toRouteSchema = (endpoint: AnyEndpoint) => {
  const reqShape = getSchemaShape(endpoint.req);

  return {
    ...(reqShape.params ? { params: reqShape.params } : {}),
    ...(reqShape.query ? { query: reqShape.query } : {}),
    ...(reqShape.body ? { body: reqShape.body } : {}),
    response: {
      200: endpoint.res as unknown as z.ZodTypeAny,
    },
  };
};

const pluginIdByEndpoint = new Map<AnyEndpoint, string>();
for (const plugin of apiPlugins) {
  for (const endpoint of plugin.endpoints) {
    pluginIdByEndpoint.set(endpoint, plugin.id);
  }
}

export const createApp = () => {
  const app = new Elysia()
    .use(
      openapi({
        path: "/openapi",
        specPath: "/openapi/json",
        mapJsonSchema: {
          zod: zodToOpenApiJsonSchema,
        },
        documentation: {
          info: {
            title: "Beamos API",
            version: "0.0.1",
          },
        },
        references: fromTypes(),
      }),
    )
    .get("/health", () => ({
      ok: true,
      plugins: apiPlugins.map((plugin) => plugin.id),
    }))
    .onAfterHandle(({ set }) => {
      set.headers["x-powered-by"] = "beamos-api";
    })
    .onError(({ code, error, set }) => {
      if (code === "NOT_FOUND") {
        set.status = 404;
        return toErrorBody("Not found");
      }

      if (code === "VALIDATION") {
        set.status = 400;
        return toErrorBody(
          "Validation failed",
          (error as { all?: unknown; issues?: unknown }).all ??
            (error as { issues?: unknown }).issues,
        );
      }

      if (error instanceof Error) {
        const status = (error as Error & { status?: unknown }).status;
        if (typeof status === "number") {
          set.status = status;
          return toErrorBody(error.message);
        }
      }

      set.status = 500;
      return toErrorBody("Internal server error");
    });

  for (const endpoint of endpoints) {
    app.route(
      endpoint.method,
      endpoint.path,
      async ({ body, params, query, request }) => {
        const requestId =
          request.headers.get("x-request-id") ?? crypto.randomUUID();

        try {
          const parsedReq = endpoint.req.parse({ body, params, query });
          const result = await endpoint.handler(parsedReq, {
            requestId,
            services,
          });
          return endpoint.res.parse(result);
        } catch (error) {
          if (error instanceof z.ZodError || isZodErrorLike(error)) {
            return Response.json(
              toErrorBody(
                "Validation failed",
                (error as { issues?: unknown }).issues,
              ),
              { status: 400 },
            );
          }

          throw error;
        }
      },
      {
        ...toRouteSchema(endpoint),
        tags: [pluginIdByEndpoint.get(endpoint) ?? "core"],
      } as never,
    );
  }

  return app;
};

export async function createAppAndMigrate() {
  await bootstrapDb();
  return createApp();
}

export const createServer = () => {
  const app = createApp();
  app.listen(Number(process.env.PORT ?? 3001));

  if (!app.server) {
    throw new Error("Failed to start API server");
  }

  return app.server;
};
