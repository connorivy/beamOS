import { z } from "zod";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

if (!apiBaseUrl) {
  throw new Error("Missing required VITE_API_BASE_URL environment variable");
}

const modelsListResponseSchema = z.object({
  models: z.array(
    z.object({
      id: z.string().uuid(),
      name: z.string(),
      description: z.string(),
      lastModified: z.string().datetime().nullable(),
      role: z.enum(["Owner", "Contributor", "Reviewer"]),
    }),
  ),
});

type ModelsListResponse = z.infer<typeof modelsListResponseSchema>;

type TrpcApiClient = {
  models: {
    list: {
      query: () => Promise<ModelsListResponse>;
    };
  };
};

export const apiClient: TrpcApiClient = {
  models: {
    list: {
      query: async () => {
        const response = await fetch(`${apiBaseUrl}/trpc/models.list`, {
          method: "POST",
        });
        if (!response.ok) {
          throw new Error(
            `Failed to fetch models: ${response.status} ${response.statusText}`,
          );
        }
        return modelsListResponseSchema.parse(await response.json());
      },
    },
  },
};
