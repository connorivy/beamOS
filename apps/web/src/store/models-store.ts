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
  loadProjects: () => Promise<void>;
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
  loadProjects: async () => {
    try {
      set({ isLoading: true, error: null });
      const { data } = await apiClient.GET("/api/projects");
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
      set({ error: "Failed to load projects. Please try again." });
    } finally {
      set({ isLoading: false });
    }
  },
}));
