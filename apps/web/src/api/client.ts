import { createApiClient } from "@beamos/openapi-client";

function normalizeApiBaseUrl(rawBaseUrl: string): string {
    const trimmedBaseUrl = rawBaseUrl.replace(/\/+$/, "");

    if (trimmedBaseUrl === "/api") {
        return "";
    }

    if (trimmedBaseUrl.endsWith("/api")) {
        return trimmedBaseUrl.slice(0, -"/api".length);
    }

    return trimmedBaseUrl;
}

const apiBaseUrl = normalizeApiBaseUrl(
    import.meta.env.VITE_API_BASE_URL ?? (import.meta.env.DEV ? "" : ""),
);

if (!apiBaseUrl) {
    if (!import.meta.env.DEV) {
        throw new Error("Missing required VITE_API_BASE_URL environment variable");
    }
}

export const apiClient = createApiClient(apiBaseUrl);
