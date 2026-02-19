import { create } from "zustand";
import type {
  CreateElement1dRequest,
  CreateMaterialRequest,
  CreateModelRevisionRequest,
  CreateModelRevisionElement1dOperationsRequest,
  CreateModelRevisionMaterialOperationsRequest,
  CreateModelRevisionNodeOperationsRequest,
  CreateModelRevisionSectionProfileOperationsRequest,
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

type PendingRevisionBuilder = {
  request: CreateModelRevisionRequest;
  ensureNodes: () => CreateModelRevisionNodeOperationsRequest;
  ensureMaterials: () => CreateModelRevisionMaterialOperationsRequest;
  ensureSectionProfiles: () => CreateModelRevisionSectionProfileOperationsRequest;
  ensureElement1ds: () => CreateModelRevisionElement1dOperationsRequest;
};

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
  queueMaterialDelete: (materialName: string) => void;

  queueSectionProfileCreate: (
    sectionProfile: CreateSectionProfileRequest,
  ) => string;
  queueSectionProfileUpdate: (sectionProfile: PutSectionProfileRequest) => void;
  queueSectionProfileDelete: (sectionProfileName: string) => void;

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

const emptyPendingRevision = (): CreateModelRevisionRequest => ({});

const createPendingRevisionBuilder = (
  pendingRevision: CreateModelRevisionRequest,
): PendingRevisionBuilder => {
  const request: CreateModelRevisionRequest = { ...pendingRevision };

  const ensureNodes = (): CreateModelRevisionNodeOperationsRequest => {
    const current = request.nodes as
      | CreateModelRevisionNodeOperationsRequest
      | null
      | undefined;
    if (!current) {
      request.nodes = {};
    }
    return request.nodes as CreateModelRevisionNodeOperationsRequest;
  };

  const ensureMaterials = (): CreateModelRevisionMaterialOperationsRequest => {
    const current = request.materials as
      | CreateModelRevisionMaterialOperationsRequest
      | null
      | undefined;
    if (!current) {
      request.materials = {};
    }
    return request.materials as CreateModelRevisionMaterialOperationsRequest;
  };

  const ensureSectionProfiles =
    (): CreateModelRevisionSectionProfileOperationsRequest => {
      const current = request.sectionProfiles as
        | CreateModelRevisionSectionProfileOperationsRequest
        | null
        | undefined;
      if (!current) {
        request.sectionProfiles = {};
      }
      return request.sectionProfiles as CreateModelRevisionSectionProfileOperationsRequest;
    };

  const ensureElement1ds =
    (): CreateModelRevisionElement1dOperationsRequest => {
      const current = request.element1ds as
        | CreateModelRevisionElement1dOperationsRequest
        | null
        | undefined;
      if (!current) {
        request.element1ds = {};
      }
      return request.element1ds as CreateModelRevisionElement1dOperationsRequest;
    };

  return {
    request,
    ensureNodes,
    ensureMaterials,
    ensureSectionProfiles,
    ensureElement1ds,
  };
};

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
  items: T[] | null | undefined,
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
  items: T[] | null | undefined,
  tempId: string,
): T[] | undefined => {
  const filtered = (items ?? []).filter((item) => item.tempId !== tempId);
  return filtered.length > 0 ? filtered : undefined;
};

const isCreatedInPending = <T extends { tempId?: string }>(
  createItems: T[] | null | undefined,
  id: string,
): boolean => Boolean((createItems ?? []).find((item) => item.tempId === id));

const removeById = <T extends { id: string }>(
  items: T[] | null | undefined,
  id: string,
): T[] | undefined => {
  const filtered = (items ?? []).filter((item) => item.id !== id);
  return filtered.length > 0 ? filtered : undefined;
};

const upsertByName = <T extends { name: string }>(
  items: T[] | null | undefined,
  nextItem: T,
): T[] => {
  const base = items ?? [];
  const index = base.findIndex((item) => item.name === nextItem.name);
  if (index === -1) {
    return [...base, nextItem];
  }

  const next = base.slice();
  next[index] = nextItem;
  return next;
};

const removeByName = <T extends { name: string }>(
  items: T[] | null | undefined,
  name: string,
): T[] | undefined => {
  const filtered = (items ?? []).filter((item) => item.name !== name);
  return filtered.length > 0 ? filtered : undefined;
};

const addUniqueDelete = (
  items: string[] | null | undefined,
  id: string,
): string[] =>
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
  (pending.nodes?.create?.length ?? 0) > 0 ||
  (pending.nodes?.update?.length ?? 0) > 0 ||
  (pending.nodes?.delete?.length ?? 0) > 0 ||
  (pending.materials?.create?.length ?? 0) > 0 ||
  (pending.materials?.update?.length ?? 0) > 0 ||
  (pending.materials?.delete?.length ?? 0) > 0 ||
  (pending.sectionProfiles?.create?.length ?? 0) > 0 ||
  (pending.sectionProfiles?.update?.length ?? 0) > 0 ||
  (pending.sectionProfiles?.delete?.length ?? 0) > 0 ||
  (pending.element1ds?.create?.length ?? 0) > 0 ||
  (pending.element1ds?.update?.length ?? 0) > 0 ||
  (pending.element1ds?.delete?.length ?? 0) > 0 ||
  (pending.loadCases?.create?.length ?? 0) > 0 ||
  (pending.loadCases?.update?.length ?? 0) > 0 ||
  (pending.loadCases?.delete?.length ?? 0) > 0 ||
  (pending.loadCombinations?.create?.length ?? 0) > 0 ||
  (pending.loadCombinations?.update?.length ?? 0) > 0 ||
  (pending.loadCombinations?.delete?.length ?? 0) > 0 ||
  (pending.pointLoads?.create?.length ?? 0) > 0 ||
  (pending.pointLoads?.update?.length ?? 0) > 0 ||
  (pending.pointLoads?.delete?.length ?? 0) > 0;

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
      "/api/projects/{projectId}/branches/{branchName}",
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

    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const nodes = builder.ensureNodes();

      nodes.create = upsertCreateByTempId(nodes.create, nextNode);
      nodes.delete = (nodes.delete ?? []).filter((id) => id !== tempId);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });

    return tempId;
  },

  queueNodeUpdate: (node) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const nodes = builder.ensureNodes();
      const createdNotSaved = isCreatedInPending(nodes.create, node.id);

      if (createdNotSaved) {
        nodes.create = upsertCreateByTempId(nodes.create, {
          location: node.location,
          restraint: node.restraint,
          tempId: node.id,
        });
      } else {
        nodes.update = upsertById(nodes.update ?? [], node);
      }

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueNodeDelete: (nodeId) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const nodes = builder.ensureNodes();
      const createdNotSaved = isCreatedInPending(nodes.create, nodeId);

      nodes.create = removeCreateByTempId(nodes.create, nodeId);
      nodes.update = removeById(nodes.update, nodeId);
      nodes.delete = createdNotSaved
        ? nodes.delete ?? undefined
        : addUniqueDelete(nodes.delete, nodeId);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueMaterialCreate: (material) => {
    const nextMaterial = { ...material };

    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const materials = builder.ensureMaterials();

      materials.create = upsertByName(materials.create, nextMaterial);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });

    return nextMaterial.name;
  },

  queueMaterialUpdate: (material) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const materials = builder.ensureMaterials();
      const createdNotSaved = Boolean(
        (materials.create ?? []).find(
          (candidate) => candidate.name === material.name,
        ),
      );

      if (createdNotSaved) {
        materials.create = upsertByName(materials.create, material);
      } else {
        materials.update = upsertByName(materials.update, material);
      }

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueMaterialDelete: (materialName) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const materials = builder.ensureMaterials();
      const createdNotSaved = Boolean(
        (materials.create ?? []).find((candidate) => candidate.name === materialName),
      );

      materials.create = removeByName(materials.create, materialName);
      materials.update = removeByName(materials.update, materialName);
      materials.delete = createdNotSaved
        ? (materials.delete ?? []).filter((name) => name !== materialName)
        : addUniqueDelete(materials.delete, materialName);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueSectionProfileCreate: (sectionProfile) => {
    const nextSectionProfile = { ...sectionProfile };

    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const sectionProfiles = builder.ensureSectionProfiles();

      sectionProfiles.create = upsertByName(sectionProfiles.create, nextSectionProfile);
      sectionProfiles.delete = (sectionProfiles.delete ?? []).filter(
        (name) => name !== nextSectionProfile.name,
      );

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });

    return nextSectionProfile.name;
  },

  queueSectionProfileUpdate: (sectionProfile) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const sectionProfiles = builder.ensureSectionProfiles();
      const createdNotSaved = Boolean(
        (sectionProfiles.create ?? []).find(
          (candidate) => candidate.name === sectionProfile.name,
        ),
      );

      if (createdNotSaved) {
        const createFromUpdate: CreateSectionProfileRequest = {
          name: sectionProfile.name,
          area: sectionProfile.area,
          strongAxisMomentOfInertia: sectionProfile.strongAxisMomentOfInertia,
          weakAxisMomentOfInertia: sectionProfile.weakAxisMomentOfInertia,
          torsionalConstant: sectionProfile.torsionalConstant,
          warpingConstant: sectionProfile.warpingConstant,
          strongAxisPlasticSectionModulus:
            sectionProfile.strongAxisPlasticSectionModulus,
          weakAxisPlasticSectionModulus:
            sectionProfile.weakAxisPlasticSectionModulus,
          strongAxisElasticSectionModulus:
            sectionProfile.strongAxisElasticSectionModulus,
          weakAxisElasticSectionModulus:
            sectionProfile.weakAxisElasticSectionModulus,
          ...(sectionProfile.strongAxisShearArea !== undefined &&
          sectionProfile.weakAxisShearArea !== undefined
            ? {
                strongAxisShearArea: sectionProfile.strongAxisShearArea,
                weakAxisShearArea: sectionProfile.weakAxisShearArea,
              }
            : {}),
        };

        sectionProfiles.create = removeByName(
          sectionProfiles.create,
          sectionProfile.name,
        );
        sectionProfiles.create = upsertByName(
          sectionProfiles.create,
          createFromUpdate,
        );
      } else {
        sectionProfiles.update = upsertByName(
          sectionProfiles.update ?? [],
          sectionProfile,
        );
      }

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueSectionProfileDelete: (sectionProfileName) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const sectionProfiles = builder.ensureSectionProfiles();
      const createdNotSaved = Boolean(
        (sectionProfiles.create ?? []).find(
          (candidate) => candidate.name === sectionProfileName,
        ),
      );

      sectionProfiles.create = removeByName(
        sectionProfiles.create,
        sectionProfileName,
      );
      sectionProfiles.update = removeByName(
        sectionProfiles.update,
        sectionProfileName,
      );
      sectionProfiles.delete = createdNotSaved
        ? sectionProfiles.delete ?? undefined
        : addUniqueDelete(sectionProfiles.delete, sectionProfileName);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueElement1dCreate: (element1d) => {
    const nextElement1d = { ...element1d };
    const tempId = ensureTempId(nextElement1d);

    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const element1ds = builder.ensureElement1ds();

      element1ds.create = upsertCreateByTempId(element1ds.create, nextElement1d);
      element1ds.delete = (element1ds.delete ?? []).filter((id) => id !== tempId);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });

    return tempId;
  },

  queueElement1dUpdate: (element1d) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const element1ds = builder.ensureElement1ds();
      element1ds.update = upsertById(element1ds.update ?? [], element1d);

      return {
        ...entry,
        pendingRevision: builder.request,
        syncStatus: "idle",
        error: null,
        lastAccessedAt: Date.now(),
      };
    });
  },

  queueElement1dDelete: (element1dId) => {
    withActiveEntry(set, get, (entry) => {
      const builder = createPendingRevisionBuilder(entry.pendingRevision);
      const element1ds = builder.ensureElement1ds();
      const createdNotSaved = isCreatedInPending(element1ds.create, element1dId);

      element1ds.create = removeCreateByTempId(element1ds.create, element1dId);
      element1ds.update = removeById(element1ds.update, element1dId);
      element1ds.delete = createdNotSaved
        ? element1ds.delete ?? undefined
        : addUniqueDelete(element1ds.delete, element1dId);

      return {
        ...entry,
        pendingRevision: builder.request,
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
