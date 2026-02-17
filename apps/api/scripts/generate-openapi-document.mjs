import path from "node:path";
import { fileURLToPath } from "node:url";
import { mkdir, writeFile } from "node:fs/promises";
import * as z from "zod";
import { zodToJsonSchema } from "zod-to-json-schema";
import { apiPlugins } from "../src/plugins/registry";
import { collectPluginEndpoints } from "../src/plugins/types";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const zodToOpenApiJsonSchema = (schema) => {
  if (
    typeof schema === "object" &&
    schema !== null &&
    "_zod" in schema &&
    typeof z.toJSONSchema === "function"
  ) {
    return z.toJSONSchema(schema);
  }

  return zodToJsonSchema(schema, {
    target: "openApi3",
    $refStrategy: "none",
  });
};

const getSchemaShape = (schema) => {
  if (typeof schema !== "object" || schema === null) {
    return {};
  }

  if (typeof schema.shape === "function") {
    return schema.shape();
  }

  if (schema.shape && typeof schema.shape === "object") {
    return schema.shape;
  }

  return {};
};

const toOpenApiPath = (pathWithParams) =>
  pathWithParams.replace(/:([A-Za-z0-9_]+)/g, "{$1}");

const toParameters = (schema, location) => {
  const jsonSchema = zodToOpenApiJsonSchema(schema);
  const properties =
    typeof jsonSchema === "object" &&
    jsonSchema !== null &&
    "properties" in jsonSchema &&
    typeof jsonSchema.properties === "object" &&
    jsonSchema.properties !== null
      ? jsonSchema.properties
      : {};
  const required =
    typeof jsonSchema === "object" &&
    jsonSchema !== null &&
    "required" in jsonSchema &&
    Array.isArray(jsonSchema.required)
      ? new Set(jsonSchema.required)
      : new Set();

  return Object.entries(properties).map(([name, propertySchema]) => ({
    name,
    in: location,
    required: location === "path" ? true : required.has(name),
    schema: propertySchema,
  }));
};

const pluginIdByEndpoint = new Map();
for (const plugin of apiPlugins) {
  for (const endpoint of plugin.endpoints) {
    pluginIdByEndpoint.set(endpoint, plugin.id);
  }
}

const endpoints = collectPluginEndpoints(apiPlugins);

const outputPath =
  process.argv[2] ?? path.resolve(__dirname, "..", "openapi.json");

const paths = {};
const components = {
  schemas: {},
};
const schemaNameByZodSchema = new WeakMap();
const schemaNameCounts = new Map();

const toPascalCase = (value) =>
  value
    .split(/[^A-Za-z0-9]+/)
    .filter(Boolean)
    .map((segment) => segment[0].toUpperCase() + segment.slice(1))
    .join("");

const toUniqueSchemaName = (baseName) => {
  const count = schemaNameCounts.get(baseName) ?? 0;
  schemaNameCounts.set(baseName, count + 1);
  return count === 0 ? baseName : `${baseName}${count + 1}`;
};

const getPreferredSchemaName = (schema) => {
  if (typeof schema !== "object" || schema === null) {
    return null;
  }

  const meta =
    typeof schema.meta === "function"
      ? schema.meta()
      : null;

  if (typeof meta?.id === "string" && meta.id.trim().length > 0) {
    return meta.id.trim();
  }

  if (typeof meta?.title === "string" && meta.title.trim().length > 0) {
    return meta.title.trim();
  }

  return null;
};

const rewriteDefRefs = (value) => {
  if (Array.isArray(value)) {
    return value.map(rewriteDefRefs);
  }

  if (typeof value !== "object" || value === null) {
    return value;
  }

  const rewritten = {};

  for (const [key, rawChild] of Object.entries(value)) {
    if (key === "$defs") {
      continue;
    }

    if (
      key === "$ref" &&
      typeof rawChild === "string" &&
      rawChild.startsWith("#/$defs/")
    ) {
      rewritten[key] = rawChild.replace(
        "#/$defs/",
        "#/components/schemas/",
      );
      continue;
    }

    rewritten[key] = rewriteDefRefs(rawChild);
  }

  return rewritten;
};

const hoistSchemaDefs = (value) => {
  if (typeof value !== "object" || value === null) {
    return;
  }

  if (
    "$defs" in value &&
    typeof value.$defs === "object" &&
    value.$defs !== null &&
    !Array.isArray(value.$defs)
  ) {
    for (const [defName, defSchema] of Object.entries(value.$defs)) {
      if (!(defName in components.schemas)) {
        components.schemas[defName] = rewriteDefRefs(defSchema);
      }
      hoistSchemaDefs(defSchema);
    }
  }

  for (const child of Object.values(value)) {
    if (Array.isArray(child)) {
      for (const item of child) {
        hoistSchemaDefs(item);
      }
      continue;
    }
    hoistSchemaDefs(child);
  }
};

const toComponentSchemaRef = (schema, baseName) => {
  if (typeof schema !== "object" || schema === null) {
    return zodToOpenApiJsonSchema(schema);
  }

  const existingName = schemaNameByZodSchema.get(schema);
  if (existingName) {
    return {
      $ref: `#/components/schemas/${existingName}`,
    };
  }

  const preferredName = getPreferredSchemaName(schema);
  const schemaName = toUniqueSchemaName(
    toPascalCase(preferredName ?? baseName),
  );
  const jsonSchema = zodToOpenApiJsonSchema(schema);
  hoistSchemaDefs(jsonSchema);
  components.schemas[schemaName] = rewriteDefRefs(jsonSchema);
  schemaNameByZodSchema.set(schema, schemaName);

  return {
    $ref: `#/components/schemas/${schemaName}`,
  };
};

for (const endpoint of endpoints) {
  const reqShape = getSchemaShape(endpoint.req);
  const parameters = [
    ...(reqShape.params ? toParameters(reqShape.params, "path") : []),
    ...(reqShape.query ? toParameters(reqShape.query, "query") : []),
  ];

  const operation = {
    tags: [pluginIdByEndpoint.get(endpoint) ?? "core"],
    responses: {
      "200": {
        description: "Successful response",
        content: {
          "application/json": {
            schema: toComponentSchemaRef(
              endpoint.res,
              `${endpoint.method} ${endpoint.path} response`,
            ),
          },
        },
      },
    },
  };

  if (parameters.length > 0) {
    operation.parameters = parameters;
  }

  if (reqShape.body) {
    operation.requestBody = {
      required:
        typeof reqShape.body?.isOptional === "function"
          ? !reqShape.body.isOptional()
          : true,
      content: {
        "application/json": {
          schema: toComponentSchemaRef(
            reqShape.body,
            `${endpoint.method} ${endpoint.path} request body`,
          ),
        },
      },
    };
  }

  const openApiPath = toOpenApiPath(endpoint.path);
  const method = endpoint.method.toLowerCase();
  paths[openApiPath] ??= {};
  paths[openApiPath][method] = operation;
}

const document = {
  openapi: "3.1.1",
  info: {
    title: "Beamos API",
    version: "0.0.1",
  },
  paths,
  components,
};

await mkdir(path.dirname(outputPath), { recursive: true });
await writeFile(outputPath, `${JSON.stringify(document, null, 2)}\n`, "utf8");
console.log(`OpenAPI document generated at ${outputPath}`);
