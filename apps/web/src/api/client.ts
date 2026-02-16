import { createApiClient } from "@beamos/openapi-client";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

if (!apiBaseUrl) {
  throw new Error("Missing required VITE_API_BASE_URL environment variable");
}

export const apiClient = createApiClient(apiBaseUrl);
