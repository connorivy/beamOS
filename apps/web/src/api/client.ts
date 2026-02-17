const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

if (!apiBaseUrl) {
  throw new Error("Missing required VITE_API_BASE_URL environment variable");
}

type ModelsListResponse = {
  models: Array<{
    id: string;
    name: string;
    description: string;
    lastModified: string | null;
    role: "Owner" | "Contributor" | "Reviewer";
  }>;
};

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
          throw new Error(`Failed to fetch models: ${response.status}`);
        }
        return (await response.json()) as ModelsListResponse;
      },
    },
  },
};
