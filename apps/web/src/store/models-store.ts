import { create } from "zustand";

export type ModelRole = "owner" | "contributor" | "reviewer";

export type UserModel = {
  id: string;
  name: string;
  description: string;
  role: ModelRole;
};

type ModelsState = {
  models: UserModel[];
};

const mockModels: UserModel[] = [
  {
    id: "m-001",
    name: "Tower Drift Check",
    description:
      "60-story lateral system study with wind and seismic combinations.",
    role: "owner",
  },
  {
    id: "m-002",
    name: "Warehouse Frame Retrofit",
    description:
      "Steel frame strengthening alternatives and connection demand tracking.",
    role: "contributor",
  },
  {
    id: "m-003",
    name: "Bridge Truss Variant B",
    description:
      "Comparative dead/live load envelope validation for the revised truss.",
    role: "reviewer",
  },
];

export const useModelsStore = create<ModelsState>(() => ({
  models: mockModels,
}));
