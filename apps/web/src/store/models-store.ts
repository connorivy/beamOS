import { create } from "zustand";
import { createApiClient } from "@beamos/openapi-client";

export type ModelRole = "owner" | "contributor" | "reviewer";

export type UserModel = {
  id: string;
  name: string;
  description: string;
  role: ModelRole;
};

type ModelsState = {
  models: UserModel[];
  isLoading: boolean;
  loadModels: () => Promise<void>;
};

const apiClient = createApiClient(
  import.meta.env.VITE_API_BASE_URL ??
    (typeof window === "undefined" ? "http://127.0.0.1:3001" : ""),
);

const roleMap: Record<"Owner" | "Contributor" | "Reviewer", ModelRole> = {
  Owner: "owner",
  Contributor: "contributor",
  Reviewer: "reviewer",
};

export const useModelsStore = create<ModelsState>((set) => ({
  models: [],
  isLoading: false,
  loadModels: async () => {
    try {
      set({ isLoading: true });
      const { data } = await apiClient.GET("/api/models");
      set({
        models:
          data?.models.map((model) => ({
            id: model.id,
            name: model.name,
            description: model.description,
            role: roleMap[model.role],
          })) ?? [],
      });
    } finally {
      set({ isLoading: false });
    }
  },
}));
