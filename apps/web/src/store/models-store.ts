import { create } from "zustand";
import { apiClient } from "../api/client";

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
  error: string | null;
  loadModels: () => Promise<void>;
};

const roleMap: Record<"Owner" | "Contributor" | "Reviewer", ModelRole> = {
  Owner: "owner",
  Contributor: "contributor",
  Reviewer: "reviewer",
};

export const useModelsStore = create<ModelsState>((set) => ({
  models: [],
  isLoading: false,
  error: null,
  loadModels: async () => {
    try {
      set({ isLoading: true, error: null });
      const { data } = await apiClient.GET("/api/models");
      set({
        models:
          data?.map((model) => ({
            id: model.id,
            name: model.name,
            description: model.description,
            role: roleMap[model.role],
          })) ?? [],
      });
    } catch (error) {
      console.error(error);
      set({ error: "Failed to load models. Please try again." });
    } finally {
      set({ isLoading: false });
    }
  },
}));
