import { create } from "zustand";
import { apiClient } from "../api/client";

export type ModelRole = "owner" | "contributor" | "reviewer";

export type UserModel = {
  id: string;
  name: string;
  description: string;
  role: ModelRole;
};

export type ModelRevisionNode = {
  id: string;
  modelId: string;
  nodeTypeDescriminator: "external" | "internal";
};

export type ModelRevisionMaterial = {
  id: string;
  revisionId: string;
  name: string;
  modulusOfElasticity: number;
  modulusOfRigidity: number;
};

export type ModelRevisionSettings = {
  id: string;
  revisionId: string;
  yAxisUp: boolean;
};

export type ModelRevisionSectionProfile = {
  id: string;
  revisionId: string;
  name: string;
};

export type ModelRevisionElement1d = {
  id: string;
  revisionId: string;
  startNodeId: string;
  endNodeId: string;
  materialId: string;
  sectionProfileId: string;
};

export type ModelRevisionSummary = {
  id: string;
  modelId: string;
  name: string;
  parentRevisionId: string | null;
  secondParentRevisionId: string | null;
  authorId: string;
  message: string;
  createdAt: string;
};

export type EditorEvent = {
  type: "model_revision_loaded";
  revisionId: string;
};

type ModelsState = {
  models: UserModel[];
  isLoading: boolean;
  error: string | null;
  modelRevision: ModelRevisionSummary | null;
  revisionNodes: ModelRevisionNode[];
  revisionMaterials: ModelRevisionMaterial[];
  revisionModelSettings: ModelRevisionSettings | null;
  revisionSectionProfiles: ModelRevisionSectionProfile[];
  revisionElement1ds: ModelRevisionElement1d[];
  modelRevisionIsLoading: boolean;
  modelRevisionError: string | null;
  editorEvents: EditorEvent[];
  loadModels: () => Promise<void>;
  loadModelRevision: (modelId: string, branchName: string) => Promise<void>;
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
  modelRevision: null,
  revisionNodes: [],
  revisionMaterials: [],
  revisionModelSettings: null,
  revisionSectionProfiles: [],
  revisionElement1ds: [],
  modelRevisionIsLoading: false,
  modelRevisionError: null,
  editorEvents: [],
  loadModels: async () => {
    try {
      set({ isLoading: true, error: null });
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
    } catch (error) {
      console.error(error);
      set({ error: "Failed to load models. Please try again." });
    } finally {
      set({ isLoading: false });
    }
  },
  loadModelRevision: async (modelId, branchName) => {
    try {
      set({ modelRevisionIsLoading: true, modelRevisionError: null });
      const { data, error } = await apiClient.GET(
        "/api/models/{modelId}/branches/{branchName}/revision",
        { params: { path: { modelId, branchName } } },
      );

      if (error || !data?.modelRevision) {
        throw new Error("Failed to fetch model revision");
      }

      const modelRevision = data.modelRevision;
      set((state) => ({
        modelRevision: {
          id: modelRevision.id,
          modelId: modelRevision.modelId,
          name: modelRevision.name,
          parentRevisionId: modelRevision.parentRevisionId,
          secondParentRevisionId: modelRevision.secondParentRevisionId,
          authorId: modelRevision.authorId,
          message: modelRevision.message,
          createdAt: modelRevision.createdAt,
        },
        revisionNodes: modelRevision.nodes.map((node) => ({
          id: node.id,
          modelId: node.modelId,
          nodeTypeDescriminator: node.nodeTypeDescriminator,
        })),
        revisionMaterials: modelRevision.materials.map((material) => ({
          id: material.id,
          revisionId: material.revisionId,
          name: material.name,
          modulusOfElasticity: material.modulusOfElasticity,
          modulusOfRigidity: material.modulusOfRigidity,
        })),
        revisionModelSettings: modelRevision.modelSettings
          ? {
              id: modelRevision.modelSettings.id,
              revisionId: modelRevision.modelSettings.revisionId,
              yAxisUp: modelRevision.modelSettings.yAxisUp,
            }
          : null,
        revisionSectionProfiles: modelRevision.sectionProfiles.map(
          (sectionProfile) => ({
            id: sectionProfile.id,
            revisionId: sectionProfile.revisionId,
            name: sectionProfile.name,
          }),
        ),
        revisionElement1ds: modelRevision.element1ds.map((element1d) => ({
          id: element1d.id,
          revisionId: element1d.revisionId,
          startNodeId: element1d.startNodeId,
          endNodeId: element1d.endNodeId,
          materialId: element1d.materialId,
          sectionProfileId: element1d.sectionProfileId,
        })),
        editorEvents: [
          ...state.editorEvents,
          {
            type: "model_revision_loaded",
            revisionId: modelRevision.id,
          },
        ],
      }));
    } catch (fetchError) {
      console.error(fetchError);
      set({ modelRevisionError: "Failed to load model revision. Please try again." });
    } finally {
      set({ modelRevisionIsLoading: false });
    }
  },
}));
