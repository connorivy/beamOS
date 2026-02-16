import { createApiClient } from "@beamos/openapi-client";

export const apiClient = createApiClient(
  import.meta.env.VITE_API_BASE_URL ??
    (typeof window === "undefined"
      ? "http://127.0.0.1:3001"
      : window.location.origin),
);
