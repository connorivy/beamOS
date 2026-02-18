import { Alert, Box, Button, Paper, Stack, TextField, Typography } from "@mui/material";
import { useEffect, useMemo, useRef, useState } from "react";
import type { PutNodeRequest } from "@beamos/openapi-client";
import { BeamOsEditor } from "../components/editor/BeamOsEditor";
import { EditorConfigurations } from "../components/editor/EditorConfigurations";
import type {
  MoveNodeCommand,
} from "../components/editor/EditorApi/EditorEventsApi";
import { ZustandEditorEventsDispatcher } from "../components/editor/EditorApi/ZustandEditorEventsDispatcher";
import {
  Element1dResponse,
  ModelResponse,
  NodeResponse,
} from "../components/editor/EditorApi/EditorApiAlpha";
import {
  selectActiveModelRevision,
  selectActivePendingRevision,
  useModelRevisionStore,
} from "../store/model-revision-store";

const DEFAULT_NODE_RESTRAINT: NonNullable<PutNodeRequest["restraint"]> = {
  canTranslateAlongX: true,
  canTranslateAlongY: true,
  canTranslateAlongZ: true,
  canRotateAboutX: true,
  canRotateAboutY: true,
  canRotateAboutZ: true,
};

const canvasId = "beamos-editor-canvas";

const parseInitialRouteState = () => {
  const pathMatch = window.location.pathname.match(/^\/editor\/([^/]+)\/([^/]+)$/);
  if (pathMatch) {
    return {
      projectId: decodeURIComponent(pathMatch[1]),
      branchName: decodeURIComponent(pathMatch[2]),
    };
  }

  const params = new window.URLSearchParams(window.location.search);
  return {
    projectId: params.get("projectId") ?? "",
    branchName: params.get("branch") ?? "main",
  };
};

export const ModelRevisionEditorPage = () => {
  const initialRouteState = useMemo(() => parseInitialRouteState(), []);
  const [projectIdInput, setProjectIdInput] = useState(initialRouteState.projectId);
  const [branchInput, setBranchInput] = useState(initialRouteState.branchName);

  const canvasRef = useRef<globalThis.HTMLCanvasElement | null>(null);
  const editorRef = useRef<BeamOsEditor | null>(null);

  const nodeNumericToUuidRef = useRef<Map<number, string>>(new Map());
  const nodeUuidToNumericRef = useRef<Map<string, number>>(new Map());

  const activeBranchKey = useModelRevisionStore((state) => state.activeBranchKey);
  const activeModelRevision = useModelRevisionStore(selectActiveModelRevision);
  const activePendingRevision = useModelRevisionStore(selectActivePendingRevision);
  const activeEntry = useModelRevisionStore((state) => state.getActiveBranch());

  const openBranch = useModelRevisionStore((state) => state.openBranch);
  const refreshActiveBranch = useModelRevisionStore((state) => state.refreshActiveBranch);
  const saveActiveBranch = useModelRevisionStore((state) => state.saveActiveBranch);
  const queueNodeUpdate = useModelRevisionStore((state) => state.queueNodeUpdate);

  useEffect(() => {
    if (!canvasRef.current || editorRef.current) {
      return;
    }

    const dispatcher = new ZustandEditorEventsDispatcher({
      onChangeSelection: async () => { },
      onPutNodeClient: async () => { },
      onMoveNode: async (body: MoveNodeCommand) => {
        const nodeUuid = nodeNumericToUuidRef.current.get(body.nodeId);
        if (!nodeUuid) {
          return;
        }

        queueNodeUpdate({
          id: nodeUuid,
          location: {
            type: "spatial",
            point: {
              x: body.newLocation.x,
              y: body.newLocation.y,
              z: body.newLocation.z,
            },
          },
          restraint: DEFAULT_NODE_RESTRAINT,
        });
      },
    });

    editorRef.current = new BeamOsEditor(
      canvasRef.current,
      dispatcher,
      new EditorConfigurations(false),
    );

    return () => {
      editorRef.current?.dispose();
      editorRef.current = null;
      nodeNumericToUuidRef.current = new Map();
      nodeUuidToNumericRef.current = new Map();
    };
  }, [queueNodeUpdate]);

  useEffect(() => {
    if (!projectIdInput.trim()) {
      return;
    }

    void openBranch(projectIdInput.trim(), branchInput.trim() || "main", {
      preferCache: true,
      refreshInBackground: true,
    });
  }, [branchInput, openBranch, projectIdInput]);

  useEffect(() => {
    if (!editorRef.current) {
      return;
    }

    const editor = editorRef.current;
    const modelRevision = activeModelRevision;
    if (!modelRevision) {
      void editor.api.clear();
      return;
    }

    const nextNumericToUuid = new Map<number, string>();
    const nextUuidToNumeric = new Map<string, number>();

    const externalNodes = modelRevision.nodes.filter(
      (node) => node.nodeTypeDescriminator === "external",
    );

    const pendingNodeUpdatesById = new Map(
      (activePendingRevision?.nodes.update ?? []).map((node) => [node.id, node] as const),
    );

    const projectedNodes = externalNodes.map((node, index) => {
      const numericId = index + 1;
      nextNumericToUuid.set(numericId, node.id);
      nextUuidToNumeric.set(node.id, numericId);

      const pendingUpdate = pendingNodeUpdatesById.get(node.id);
      const fallbackX = (index % 10) * 2;
      const fallbackY = Math.floor(index / 10) * 2;

      return NodeResponse.fromJS({
        id: numericId,
        modelId: modelRevision.projectId,
        locationPoint: {
          x: pendingUpdate?.location.type === "spatial" ? pendingUpdate.location.point.x : fallbackX,
          y: pendingUpdate?.location.type === "spatial" ? pendingUpdate.location.point.y : fallbackY,
          z: pendingUpdate?.location.type === "spatial" ? pendingUpdate.location.point.z : 0,
        },
        restraint: pendingUpdate?.restraint ?? DEFAULT_NODE_RESTRAINT,
      });
    });

    const projectedElement1ds = modelRevision.element1ds
      .map((element, index) => {
        const startNumeric = nextUuidToNumeric.get(element.startNodeId);
        const endNumeric = nextUuidToNumeric.get(element.endNodeId);
        if (!startNumeric || !endNumeric) {
          return null;
        }

        return Element1dResponse.fromJS({
          id: index + 1,
          modelId: modelRevision.projectId,
          startNodeId: startNumeric,
          endNodeId: endNumeric,
          materialId: 1,
          sectionProfileId: 1,
          sectionProfileRotation: { value: 0, unit: "Radian" },
          metadata: {},
        });
      })
      .filter((element): element is Element1dResponse => element !== null);

    nodeNumericToUuidRef.current = nextNumericToUuid;
    nodeUuidToNumericRef.current = nextUuidToNumeric;

    const projection = ModelResponse.fromJS({
      id: modelRevision.projectId,
      name: `Project ${modelRevision.projectId.slice(0, 8)}`,
      description: `Revision ${modelRevision.id.slice(0, 8)}`,
      settings: {
        yAxisUp: modelRevision.modelSettings?.yAxisUp ?? false,
        unitSettings: {
          lengthUnit: "Meters",
          forceUnit: "Newtons",
          angleUnit: "Degrees",
        },
        analysisSettings: {
          element1DAnalysisType: 0,
        },
      },
      lastModified: new Date(modelRevision.createdAt),
      nodes: projectedNodes,
      internalNodes: [],
      element1ds: projectedElement1ds,
      materials: [],
      sectionProfiles: [],
      pointLoads: [],
      momentLoads: [],
      resultSets: [],
      loadCases: [],
      loadCombinations: [],
      sectionProfilesFromLibrary: [],
    });

    void editor.api.clear().then(() => editor.api.createModel(projection));
  }, [activeModelRevision, activePendingRevision]);

  const handleLoad = () => {
    const projectId = projectIdInput.trim();
    if (!projectId) {
      return;
    }

    const branchName = branchInput.trim() || "main";
    const nextUrl = `/editor/${encodeURIComponent(projectId)}/${encodeURIComponent(branchName)}`;
    window.history.replaceState({}, "", nextUrl);

    void openBranch(projectId, branchName, {
      preferCache: true,
      refreshInBackground: true,
    });
  };

  return (
    <Stack spacing={2} sx={{ p: 2 }}>
      <Typography variant="h4">Model Revision Editor</Typography>

      <Paper variant="outlined" sx={{ p: 2 }}>
        <Stack direction={{ xs: "column", md: "row" }} spacing={2} alignItems={{ md: "center" }}>
          <TextField
            label="Project ID"
            value={projectIdInput}
            onChange={(event) => setProjectIdInput(event.target.value)}
            fullWidth
          />
          <TextField
            label="Branch"
            value={branchInput}
            onChange={(event) => setBranchInput(event.target.value)}
            sx={{ minWidth: 200 }}
          />
          <Button variant="contained" onClick={handleLoad} disabled={!projectIdInput.trim()}>
            Load
          </Button>
          <Button variant="outlined" onClick={() => void refreshActiveBranch()} disabled={!activeBranchKey}>
            Refresh
          </Button>
          <Button variant="contained" color="secondary" onClick={() => void saveActiveBranch()} disabled={!activeBranchKey}>
            Save Revision
          </Button>
        </Stack>
      </Paper>

      {activeEntry?.error ? <Alert severity="error">{activeEntry.error}</Alert> : null}

      <Alert severity="info">
        Revision nodes currently do not include server-side spatial coordinates, so this projection uses a temporary grid layout for external nodes.
      </Alert>

      <Paper variant="outlined" sx={{ p: 1 }}>
        <Stack direction="row" spacing={2} sx={{ px: 1, py: 0.5 }}>
          <Typography variant="body2">
            Active: {activeEntry ? `${activeEntry.projectId}/${activeEntry.branchName}` : "none"}
          </Typography>
          <Typography variant="body2">Sync: {activeEntry?.syncStatus ?? "idle"}</Typography>
          <Typography variant="body2">
            Pending node updates: {activePendingRevision?.nodes.update?.length ?? 0}
          </Typography>
        </Stack>
      </Paper>

      <Box sx={{ border: "1px solid", borderColor: "divider", borderRadius: 1, overflow: "hidden", height: "70vh", minHeight: 420 }}>
        <Box
          component="canvas"
          id={canvasId}
          ref={canvasRef}
          sx={{ width: "100%", height: "100%", display: "block" }}
        />
      </Box>
    </Stack>
  );
};
