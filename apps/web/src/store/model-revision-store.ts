import { create } from "zustand";
import type {
  CreateElement1dRequest,
  CreateMaterialRequest,
  CreateModelRevisionRequest,
  CreateNodeRequest,
  CreateSectionProfileRequest,
  ModelRevision,
  PutElement1dRequest,
  PutMaterialRequest,
  PutNodeRequest,
  PutSectionProfileRequest,
} from "@beamos/openapi-client";
import { apiClient } from "../api/client";

type BranchKey = `${string}:${string}`;
type SyncStatus = "idle" | "loading" | "saving" | "error";

type ModelRevisionSnapshot = ModelRevision;

type BranchRevisionCacheEntry = {
  projectId: string;
  branchName: string;
  modelRevision: ModelRevisionSnapshot | null;
  pendingRevision: CreateModelRevisionRequest;
  syncStatus: SyncStatus;
  error: string | null;
  lastFetchedAt: number | null;
  lastAccessedAt: number;
};

type OpenBranchOptions = {
  preferCache?: boolean;
  refreshInBackground?: boolean;
};

type ModelRevisionState = {
  activeBranchKey: BranchKey | null;
  branchEntries: Record<BranchKey, BranchRevisionCacheEntry>;
  maxCachedBranches: number;

  getActiveBranch: () => BranchRevisionCacheEntry | null;
  openBranch: (
    projectId: string,
    branchName: string,
    options?: OpenBranchOptions,
  ) => Promise<void>;
  refreshActiveBranch: () => Promise<void>;
  saveActiveBranch: () => Promise<void>;
  clearBranchCache: (projectId?: string, branchName?: string) => void;

  queueNodeCreate: (node: CreateNodeRequest) => string;
  queueNodeUpdate: (node: PutNodeRequest) => void;
  queueNodeDelete: (nodeId: string) => void;

  queueMaterialCreate: (material: CreateMaterialRequest) => string;
  queueMaterialUpdate: (material: PutMaterialRequest) => void;
  queueMaterialDelete: (materialId: string) => void;

  queueSectionProfileCreate: (
    sectionProfile: CreateSectionProfileRequest,
  ) => string;
  queueSectionProfileUpdate: (sectionProfile: PutSectionProfileRequest) => void;
  queueSectionProfileDelete: (sectionProfileId: string) => void;

  queueElement1dCreate: (element1d: CreateElement1dRequest) => string;
  queueElement1dUpdate: (element1d: PutElement1dRequest) => void;
  queueElement1dDelete: (element1dId: string) => void;

  resetPendingForActiveBranch: () => void;
};

const DEFAULT_MAX_CACHED_BRANCHES = 8;

const buildBranchKey = (projectId: string, branchName: string): BranchKey =>
  `${projectId}:${branchName}`;

const parseBranchKey = (
  branchKey: BranchKey,
): { projectId: string; branchName: string } => {
  const separatorIndex = branchKey.indexOf(":");
  return {
    projectId: branchKey.slice(0, separatorIndex),
    branchName: branchKey.slice(separatorIndex + 1),
  };
};

const emptyPendingRevision = (): CreateModelRevisionRequest => ({
  nodes: {},
  materials: {},
  sectionProfiles: {},
  element1ds: {},
});

const normalizeModelRevision = (
  modelRevision: ModelRevision & Partial<{ version: unknown }>,
): ModelRevisionSnapshot => {
  const { version: _ignoredVersion, ...withoutVersion } = modelRevision;
  void _ignoredVersion;
  return withoutVersion;
};

const ensureTempId = <T extends { tempId?: string }>(payload: T): string => {
  const tempId = payload.tempId?.trim() || crypto.randomUUID();
  payload.tempId = tempId;
  return tempId;
};

const upsertById = <T extends { id: string }>(items: T[], nextItem: T): T[] => {
  const index = items.findIndex((item) => item.id === nextItem.id);
  if (index === -1) {
    return [...items, nextItem];
  }

  const next = items.slice();
  next[index] = nextItem;
  return next;
};

const upsertCreateByTempId = <T extends { tempId?: string }>(
  items: T[] | undefined,
  nextItem: T,
): T[] => {
  const tempId = nextItem.tempId;
  if (!tempId) {
    return [...(items ?? []), nextItem];
  }

  const base = items ?? [];
  const index = base.findIndex((item) => item.tempId === tempId);
  if (index === -1) {
    return [...base, nextItem];
  }

  const next = base.slice();
  next[index] = nextItem;
  return next;
};

const removeCreateByTempId = <T extends { tempId?: string }>(
  items: T[] | undefined,
  tempId: string,
): T[] | undefined => {
  const filtered = (items ?? []).filter((item) => item.tempId !== tempId);
  return filtered.length > 0 ? filtered : undefined;
};

const isCreatedInPending = <T extends { tempId?: string }>(
  createItems: T[] | undefined,
  id: string,
): boolean => Boolean((createItems ?? []).find((item) => item.tempId === id));

const removeById = <T extends { id: string }>(
  items: T[] | undefined,
  id: string,
): T[] | undefined => {
  const filtered = (items ?? []).filter((item) => item.id !== id);
  return filtered.length > 0 ? filtered : undefined;
};

const addUniqueDelete = (items: string[] | undefined, id: string): string[] =>
  Array.from(new Set([...(items ?? []), id]));

const pruneCachedBranches = (
  entries: Record<BranchKey, BranchRevisionCacheEntry>,
  maxCachedBranches: number,
): Record<BranchKey, BranchRevisionCacheEntry> => {
  const keys = Object.keys(entries) as BranchKey[];
  if (keys.length <= maxCachedBranches) {
    return entries;
  }

  const sorted = keys.sort(
    (a, b) => entries[a].lastAccessedAt - entries[b].lastAccessedAt,
  );

  const nextEntries = { ...entries };
  while (Object.keys(nextEntries).length > maxCachedBranches) {
    const keyToDrop = sorted.shift();
    if (!keyToDrop) {
      break;
    }
    delete nextEntries[keyToDrop];
  }

  return nextEntries;
};

const hasPendingChanges = (pending: CreateModelRevisionRequest): boolean =>
  (pending.nodes.create?.length ?? 0) > 0 ||
  (pending.nodes.update?.length ?? 0) > 0 ||
  (pending.nodes.delete?.length ?? 0) > 0 ||
  (pending.materials.create?.length ?? 0) > 0 ||
  (pending.materials.update?.length ?? 0) > 0 ||
  (pending.materials.delete?.length ?? 0) > 0 ||
  (pending.sectionProfiles.create?.length ?? 0) > 0 ||
  (pending.sectionProfiles.update?.length ?? 0) > 0 ||
  (pending.sectionProfiles.delete?.length ?? 0) > 0 ||
  (pending.element1ds.create?.length ?? 0) > 0 ||
  (pending.element1ds.update?.length ?? 0) > 0 ||
  (pending.element1ds.delete?.length ?? 0) > 0;

const withActiveEntry = (
  set: (partial: Partial<ModelRevisionState>) => void,
  get: () => ModelRevisionState,
  updater: (entry: BranchRevisionCacheEntry) => BranchRevisionCacheEntry,
): void => {
  const activeBranchKey = get().activeBranchKey;
  if (!activeBranchKey) {
    return;
  }

  const currentEntry = get().branchEntries[activeBranchKey];
  if (!currentEntry) {
    return;
  }

  set({
    branchEntries: {
      ...get().branchEntries,
      [activeBranchKey]: updater(currentEntry),
    },
  });
};

export const useModelRevisionStore = create<ModelRevisionState>((set, get) => ({
  activeBranchKey: null,
  branchEntries: {},
  maxCachedBranches: DEFAULT_MAX_CACHED_BRANCHES,

  getActiveBranch: () => {
    const activeBranchKey = get().activeBranchKey;
    if (!activeBranchKey) {
      return null;
    }
    return get().branchEntries[activeBranchKey] ?? null;
  },

  openBranch: async (projectId, branchName, options) => {
    const branchKey = buildBranchKey(projectId, branchName);
    const now = Date.now();
    const preferCache = options?.preferCache ?? true;
    const refreshInBackground = options?.refreshInBackground ?? true;
    const existing = get().branchEntries[branchKey];

    if (existing && preferCache) {
      set({
        activeBranchKey: branchKey,
        branchEntries: {
          ...get().branchEntries,
          [branchKey]: {
            ...existing,
            syncStatus: "idle",
            error: null,
            lastAccessedAt: now,
          },
        },
      });

      if (!refreshInBackground) {
        return;
      }
    } else {
      set({
        activeBranchKey: branchKey,
        branchEntries: {
          ...get().branchEntries,
          [branchKey]: {
            projectId,
            branchName,
            modelRevision: existing?.modelRevision ?? null,
            pendingRevision:
              existing?.pendingRevision ?? emptyPendingRevision(),
            syncStatus: "loading",
            error: null,
            lastFetchedAt: existing?.lastFetchedAt ?? null,
            lastAccessedAt: now,
          },
        },
      });
    }

    const { data, error } = await apiClient.GET(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      { params: { path: { projectId, branchName } } },
    );

    if (error || !data) {
      set({
        branchEntries: {
          ...get().branchEntries,
          [branchKey]: {
            ...(get().branchEntries[branchKey] ?? {
              projectId,
              branchName,
              modelRevision: null,
              pendingRevision: emptyPendingRevision(),
              lastFetchedAt: null,
              lastAccessedAt: now,
            }),
            syncStatus: "error",
            error: "Failed to load model revision.",
          },
        },
      });
      return;
    }

    const currentEntry = get().branchEntries[branchKey];

    set({
      branchEntries: pruneCachedBranches(
        {
          ...get().branchEntries,
          [branchKey]: {
            projectId,
            branchName,
            modelRevision: normalizeModelRevision(data),
            pendingRevision:
              currentEntry?.pendingRevision ?? emptyPendingRevision(),
            syncStatus: "idle",
            error: null,
            lastFetchedAt: Date.now(),
            lastAccessedAt: Date.now(),
          },
        },
        get().maxCachedBranches,
      ),
    });
  },

  refreshActiveBranch: async () => {
    const activeBranchKey = get().activeBranchKey;
    if (!activeBranchKey) {
      return;
    }

    const { projectId, branchName } = parseBranchKey(activeBranchKey);
    await get().openBranch(projectId, branchName, {
      preferCache: false,
      refreshInBackground: false,
    });
  },

  saveActiveBranch: async () => {
    const activeBranchKey = get().activeBranchKey;
    if (!activeBranchKey) {
      return;
    }

    const entry = get().branchEntries[activeBranchKey];
    if (!entry || !hasPendingChanges(entry.pendingRevision)) {
      return;
    }

    set({
      branchEntries: {
        ...get().branchEntries,
        [activeBranchKey]: {
          ...entry,
          syncStatus: "saving",
          error: null,
        },
      },
    });

    const { data, error } = await apiClient.POST(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: {
          path: {
            projectId: entry.projectId,
            branchName: entry.branchName,
          },
        },
        body: entry.pendingRevision,
      },
    );

    if (error || !data) {
      set({
        branchEntries: {
          ...get().branchEntries,
          [activeBranchKey]: {
            ...entry,
            syncStatus: "error",
            error: "Failed to save model revision.",
          },
        },
      });
      return;
    }

    set({
      branchEntries: {
        ...get().branchEntries,
        [activeBranchKey]: {
          ...entry,
          modelRevision: normalizeModelRevision(data),
          pendingRevision: emptyPendingRevision(),
          syncStatus: "idle",
          error: null,
          lastFetchedAt: Date.now(),
          lastAccessedAt: Date.now(),
        },
      },
    });
  },

  clearBranchCache: (projectId, branchName) => {
    if (!projectId && !branchName) {
      set({ activeBranchKey: null, branchEntries: {} });
      return;
    }

    if (!projectId || !branchName) {
      return;
    }

    const branchKey = buildBranchKey(projectId, branchName);
    const nextEntries = { ...get().branchEntries };
    delete nextEntries[branchKey];

    set({
      activeBranchKey:
        get().activeBranchKey === branchKey ? null : get().activeBranchKey,
      branchEntries: nextEntries,
    });
  },

  queueNodeCreate: (node) => {
    const nextNode = { ...node };
    const tempId = ensureTempId(nextNode);

    withActiveEntry(set, get, (entry) => ({
      ...entry,
      pendingRevision: {
        ...entry.pendingRevision,
        nodes: {
          ...entry.pendingRevision.nodes,
          create: upsertCreateByTempId(
            entry.pendingRevision.nodes.create,
            nextNode,
          ),
          delete: (entry.pendingRevision.nodes.delete ?? []).filter(
            (id) => id !== tempId,
          ),
        },
      },
      syncStatus: "idle",
      error: null,
      lastAccessedAt: Date.now(),
    }));

    return tempId;
  },

  queueNodeUpdate: (node) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.nodes.create,
        node.id,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          nodes: createdNotSaved
            ? {
                ...entry.pendingRevision.nodes,
                create: upsertCreateByTempId(
                  entry.pendingRevision.nodes.create,
                  {
                    location: node.location,
                    restraint: node.restraint,
                    tempId: node.id,
                  },
                ),
              }
            : {
                ...entry.pendingRevision.nodes,
                update: upsertById(
                  entry.pendingRevision.nodes.update ?? [],
                  node,
                ),
              },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueNodeDelete: (nodeId) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.nodes.create,
        nodeId,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          nodes: {
            ...entry.pendingRevision.nodes,
            create: removeCreateByTempId(
              entry.pendingRevision.nodes.create,
              nodeId,
            ),
            update: removeById(entry.pendingRevision.nodes.update, nodeId),
            delete: createdNotSaved
              ? entry.pendingRevision.nodes.delete
              : addUniqueDelete(entry.pendingRevision.nodes.delete, nodeId),
          },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueMaterialCreate: (material) => {
    const nextMaterial = { ...material };
    const tempId = ensureTempId(nextMaterial);

    withActiveEntry(set, get, (entry) => ({
      ...entry,
      pendingRevision: {
        ...entry.pendingRevision,
        materials: {
          ...entry.pendingRevision.materials,
          create: upsertCreateByTempId(
            entry.pendingRevision.materials.create,
            nextMaterial,
          ),
          delete: (entry.pendingRevision.materials.delete ?? []).filter(
            (id) => id !== tempId,
          ),
        },
      },
      syncStatus: "idle",
      error: null,
      lastAccessedAt: Date.now(),
    }));

    return tempId;
  },

  queueMaterialUpdate: (material) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.materials.create,
        material.id,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          materials: createdNotSaved
            ? {
                ...entry.pendingRevision.materials,
                create: upsertCreateByTempId(
                  entry.pendingRevision.materials.create,
                  {
                    name: material.name,
                    modulusOfElasticity: material.modulusOfElasticity,
                    modulusOfRigidity: material.modulusOfRigidity,
                    units: material.units,
                    tempId: material.id,
                  },
                ),
              }
            : {
                ...entry.pendingRevision.materials,
                update: upsertById(
                  entry.pendingRevision.materials.update ?? [],
                  material,
                ),
              },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueMaterialDelete: (materialId) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.materials.create,
        materialId,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          materials: {
            ...entry.pendingRevision.materials,
            create: removeCreateByTempId(
              entry.pendingRevision.materials.create,
              materialId,
            ),
            update: removeById(
              entry.pendingRevision.materials.update,
              materialId,
            ),
            delete: createdNotSaved
              ? entry.pendingRevision.materials.delete
              : addUniqueDelete(
                  entry.pendingRevision.materials.delete,
                  materialId,
                ),
          },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueSectionProfileCreate: (sectionProfile) => {
    const nextSectionProfile = { ...sectionProfile };
    const tempId = ensureTempId(nextSectionProfile);

    withActiveEntry(set, get, (entry) => ({
      ...entry,
      pendingRevision: {
        ...entry.pendingRevision,
        sectionProfiles: {
          ...entry.pendingRevision.sectionProfiles,
          create: upsertCreateByTempId(
            entry.pendingRevision.sectionProfiles.create,
            nextSectionProfile,
          ),
          delete: (entry.pendingRevision.sectionProfiles.delete ?? []).filter(
            (id) => id !== tempId,
          ),
        },
      },
      syncStatus: "idle",
      error: null,
      lastAccessedAt: Date.now(),
    }));

    return tempId;
  },

  queueSectionProfileUpdate: (sectionProfile) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.sectionProfiles.create,
        sectionProfile.id,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          sectionProfiles: createdNotSaved
            ? {
                ...entry.pendingRevision.sectionProfiles,
                create: upsertCreateByTempId(
                  entry.pendingRevision.sectionProfiles.create,
                  {
                    ...sectionProfile,
                    tempId: sectionProfile.id,
                  },
                ),
              }
            : {
                ...entry.pendingRevision.sectionProfiles,
                update: upsertById(
                  entry.pendingRevision.sectionProfiles.update ?? [],
                  sectionProfile,
                ),
              },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueSectionProfileDelete: (sectionProfileId) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.sectionProfiles.create,
        sectionProfileId,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          sectionProfiles: {
            ...entry.pendingRevision.sectionProfiles,
            create: removeCreateByTempId(
              entry.pendingRevision.sectionProfiles.create,
              sectionProfileId,
            ),
            update: removeById(
              entry.pendingRevision.sectionProfiles.update,
              sectionProfileId,
            ),
            delete: createdNotSaved
              ? entry.pendingRevision.sectionProfiles.delete
              : addUniqueDelete(
                  entry.pendingRevision.sectionProfiles.delete,
                  sectionProfileId,
                ),
          },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueElement1dCreate: (element1d) => {
    const nextElement1d = { ...element1d };
    const tempId = ensureTempId(nextElement1d);

    withActiveEntry(set, get, (entry) => ({
      ...entry,
      pendingRevision: {
        ...entry.pendingRevision,
        element1ds: {
          ...entry.pendingRevision.element1ds,
          create: upsertCreateByTempId(
            entry.pendingRevision.element1ds.create,
            nextElement1d,
          ),
          delete: (entry.pendingRevision.element1ds.delete ?? []).filter(
            (id) => id !== tempId,
          ),
        },
      },
      syncStatus: "idle",
      error: null,
      lastAccessedAt: Date.now(),
    }));

    return tempId;
  },

  queueElement1dUpdate: (element1d) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.element1ds.create,
        element1d.id,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          element1ds: createdNotSaved
            ? {
                ...entry.pendingRevision.element1ds,
                create: upsertCreateByTempId(
                  entry.pendingRevision.element1ds.create,
                  {
                    startNodeId: element1d.startNodeId,
                    endNodeId: element1d.endNodeId,
                    materialId: element1d.materialId,
                    sectionProfileId: element1d.sectionProfileId,
                    tempId: element1d.id,
                  },
                ),
              }
            : {
                ...entry.pendingRevision.element1ds,
                update: upsertById(
                  entry.pendingRevision.element1ds.update ?? [],
                  element1d,
                ),
              },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueElement1dDelete: (element1dId) => {
    withActiveEntry(set, get, (entry) => {
      const createdNotSaved = isCreatedInPending(
        entry.pendingRevision.element1ds.create,
        element1dId,
      );

      return {
        ...entry,
        pendingRevision: {
          ...entry.pendingRevision,
          element1ds: {
            ...entry.pendingRevision.element1ds,
            create: removeCreateByTempId(
              entry.pendingRevision.element1ds.create,
              element1dId,
            ),
            update: removeById(
              entry.pendingRevision.element1ds.update,
              element1dId,
            ),
            delete: createdNotSaved
              ? entry.pendingRevision.element1ds.delete
              : addUniqueDelete(
                  entry.pendingRevision.element1ds.delete,
                  element1dId,
                ),
          },
        },
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  resetPendingForActiveBranch: () => {
    withActiveEntry(set, get, (entry) => ({
      ...entry,
      pendingRevision: emptyPendingRevision(),
      syncStatus: "idle",
      error: null,
      lastAccessedAt: Date.now(),
    }));
  },
}));

export const selectActiveModelRevision = (
  state: ModelRevisionState,
): ModelRevisionSnapshot | null =>
  state.getActiveBranch()?.modelRevision ?? null;

export const selectActivePendingRevision = (
  state: ModelRevisionState,
): CreateModelRevisionRequest | null =>
  state.getActiveBranch()?.pendingRevision ?? null;

export type {
  BranchKey,
  BranchRevisionCacheEntry,
  ModelRevisionSnapshot,
  ModelRevisionState,
  OpenBranchOptions,
  SyncStatus,
};
