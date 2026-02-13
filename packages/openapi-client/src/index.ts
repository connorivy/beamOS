import createFetchClient from "openapi-fetch";
import { paths } from "./schema";

export type ApiPaths = paths;

export function createApiClient(baseUrl: string) {
  return createFetchClient<paths>({ baseUrl: baseUrl });
}
