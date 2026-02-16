import createFetchClient from "openapi-fetch";
import type { paths } from "./generated/schema";

export type ApiPaths = paths;
export type * from "./generated/schema";

export function createApiClient(baseUrl: string) {
  return createFetchClient<paths>({ baseUrl: baseUrl });
}
