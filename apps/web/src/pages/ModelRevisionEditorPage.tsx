import AddRoundedIcon from "@mui/icons-material/AddRounded";
import ArchitectureRoundedIcon from "@mui/icons-material/ArchitectureRounded";
import AutoFixHighRoundedIcon from "@mui/icons-material/AutoFixHighRounded";
import BackHandRoundedIcon from "@mui/icons-material/BackHandRounded";
import CallSplitRoundedIcon from "@mui/icons-material/CallSplitRounded";
import CategoryRoundedIcon from "@mui/icons-material/CategoryRounded";
import CenterFocusStrongRoundedIcon from "@mui/icons-material/CenterFocusStrongRounded";
import CompareArrowsRoundedIcon from "@mui/icons-material/CompareArrowsRounded";
import ConstructionRoundedIcon from "@mui/icons-material/ConstructionRounded";
import GridOnRoundedIcon from "@mui/icons-material/GridOnRounded";
import LayersRoundedIcon from "@mui/icons-material/LayersRounded";
import PanToolAltRoundedIcon from "@mui/icons-material/PanToolAltRounded";
import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import SettingsRoundedIcon from "@mui/icons-material/SettingsRounded";
import TuneRoundedIcon from "@mui/icons-material/TuneRounded";
import ZoomInRoundedIcon from "@mui/icons-material/ZoomInRounded";
import {
  Alert,
  Box,
  Button,
  Chip,
  Divider,
  IconButton,
  Paper,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { PutNodeRequest } from "@beamos/openapi-client";
import { apiClient } from "../api/client";
import { BeamOsEditor } from "../components/editor/BeamOsEditor";
import { EditorConfigurations } from "../components/editor/EditorConfigurations";
import { TutorialTour } from "../components/TutorialTour";
import { TUTORIAL_PROJECT_ID } from "./ModelsPage";
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
import { useModelsStore } from "../store/models-store";

const DEFAULT_NODE_RESTRAINT: NonNullable<PutNodeRequest["restraint"]> = {
  canTranslateAlongX: true,
  canTranslateAlongY: true,
  canTranslateAlongZ: true,
  canRotateAboutX: true,
  canRotateAboutY: true,
  canRotateAboutZ: true,
};

const canvasId = "beamos-editor-canvas";
const editorTopBarHeight = 52;

const parseInitialRouteState = () => {
  const pathMatch = window.location.pathname.match(/^\/editor\/projects\/([^/]+)\/([^/]+)$/);
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
  const navigate = useNavigate();
  const initialRouteState = useMemo(() => parseInitialRouteState(), []);
  const [projectIdInput] = useState(initialRouteState.projectId);
  const [branchInput] = useState(initialRouteState.branchName);
  const [activePanel, setActivePanel] = useState<"models" | "revisionControl">("models");
  const [isForkLoading, setIsForkLoading] = useState(false);
  const [forkError, setForkError] = useState<string | null>(null);

  const canvasRef = useRef<globalThis.HTMLCanvasElement | null>(null);
  const editorRef = useRef<BeamOsEditor | null>(null);

  const nodeNumericToUuidRef = useRef<Map<number, string>>(new Map());
  const nodeUuidToNumericRef = useRef<Map<string, number>>(new Map());

  const activeBranchKey = useModelRevisionStore((state) => state.activeBranchKey);
  const activeModelRevision = useModelRevisionStore(selectActiveModelRevision);
  const activePendingRevision = useModelRevisionStore(selectActivePendingRevision);
  const activeEntry = useModelRevisionStore((state) => state.getActiveBranch());

  const openBranch = useModelRevisionStore((state) => state.openBranch);
  const saveActiveBranch = useModelRevisionStore((state) => state.saveActiveBranch);
  const queueNodeUpdate = useModelRevisionStore((state) => state.queueNodeUpdate);
  const models = useModelsStore((state) => state.models);
  const loadProjects = useModelsStore((state) => state.loadProjects);
  const trimmedProjectId = projectIdInput.trim();
  const trimmedBranch = branchInput.trim() || "main";
  const isTutorial = trimmedProjectId === TUTORIAL_PROJECT_ID;
  const breadcrumbProjectId = activeEntry?.projectId ?? trimmedProjectId;
  const projectName =
    models.find((model) => model.id === breadcrumbProjectId)?.name ??
    breadcrumbProjectId ??
    "project";

  const handleFork = async () => {
    if (!breadcrumbProjectId) return;
    setIsForkLoading(true);
    setForkError(null);
    try {
      const { data, error } = await apiClient.POST("/api/projects/{projectId}/fork", {
        params: { path: { projectId: breadcrumbProjectId } },
        body: { name: `${projectName} (fork)` },
      });
      if (error || !data) {
        setForkError("Failed to fork the project. Please try again.");
        return;
      }
      await loadProjects();
      setActivePanel("models");
      navigate(`/editor/projects/${data.id}/main`);
    } catch {
      setForkError("An unexpected error occurred.");
    } finally {
      setIsForkLoading(false);
    }
  };

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
    if (models.length > 0) {
      return;
    }
    void loadProjects();
  }, [loadProjects, models.length]);

  useEffect(() => {
    if (isTutorial) {
      setActivePanel("revisionControl");
    }
  }, [isTutorial]);

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

    const externalNodes = modelRevision.nodes;

    const pendingNodeUpdatesById = new Map(
      (activePendingRevision?.nodes?.update ?? []).map(
        (node) => [node.id, node] as const,
      ),
    );

    const projectedNodes = externalNodes.map((node, index) => {
      const numericId = index + 1;
      nextNumericToUuid.set(numericId, node.id);
      nextUuidToNumeric.set(node.id, numericId);

      const pendingUpdate = pendingNodeUpdatesById.get(node.id);
      const fallbackX = (index % 10) * 2;
      const fallbackY = Math.floor(index / 10) * 2;
      const resolvedLocation = pendingUpdate?.location ?? node.location;
      const locationPoint =
        resolvedLocation.type === "spatial"
          ? resolvedLocation.point
          : { x: fallbackX, y: fallbackY, z: 0 };

      return NodeResponse.fromJS({
        id: numericId,
        modelId: activeEntry?.projectId ?? trimmedProjectId,
        locationPoint: {
          x: locationPoint.x,
          y: locationPoint.y,
          z: locationPoint.z,
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
          modelId: activeEntry?.projectId ?? trimmedProjectId,
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
      id: activeEntry?.projectId ?? trimmedProjectId,
      name: `Project ${(activeEntry?.projectId ?? trimmedProjectId).slice(0, 8)}`,
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
  }, [activeModelRevision, activePendingRevision, activeEntry, trimmedProjectId]);

  const toolbarButtons = [
    { label: "Select", icon: <PanToolAltRoundedIcon fontSize="small" /> },
    { label: "Drag", icon: <BackHandRoundedIcon fontSize="small" /> },
    { label: "Node", icon: <AddRoundedIcon fontSize="small" /> },
    { label: "Element", icon: <CompareArrowsRoundedIcon fontSize="small" /> },
    { label: "Zoom", icon: <ZoomInRoundedIcon fontSize="small" /> },
    { label: "Focus", icon: <CenterFocusStrongRoundedIcon fontSize="small" /> },
    { label: "Grid", icon: <GridOnRoundedIcon fontSize="small" /> },
    { label: "Tools", icon: <ConstructionRoundedIcon fontSize="small" /> },
    { label: "Inspect", icon: <TuneRoundedIcon fontSize="small" /> },
  ] as const;

  return (
    <Box
      sx={{
        height: "100vh",
        position: "relative",
        overflow: "hidden",
      }}
    >
      <Paper
        elevation={0}
        sx={{
          position: "absolute",
          top: 0,
          left: 0,
          right: 0,
          zIndex: 20,
          px: 2,
          height: `${editorTopBarHeight}px`,
          display: "flex",
          alignItems: "center",
          borderBottom: "1px solid",
          borderColor: "divider",
          borderRadius: 0,
        }}
      >
        <Stack direction="row" alignItems="center" spacing={1.5}>
          <ArchitectureRoundedIcon />
          <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
            beamOS Editor
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
            Projects / {projectName} / {trimmedBranch}
          </Typography>
          <Box sx={{ flex: 1 }} />
          <Button
            variant="contained"
            startIcon={<SaveRoundedIcon />}
            onClick={() => void saveActiveBranch()}
            disabled={!activeBranchKey}
          >
            Save
          </Button>
        </Stack>
      </Paper>

      <Box sx={{ position: "absolute", inset: 0, pt: `${editorTopBarHeight}px` }}>
        <Box
          component="canvas"
          id={canvasId}
          ref={canvasRef}
          sx={{ width: "100%", height: "100%", display: "block" }}
        />
      </Box>

      <Paper
        elevation={2}
        sx={{
          position: "absolute",
          top: editorTopBarHeight + 14,
          left: 12,
          zIndex: 12,
          p: 0.75,
          borderRadius: 2,
          border: "1px solid",
          borderColor: "divider",
        }}
      >
        <Stack spacing={0.5}>
          {toolbarButtons.map((button) => (
            <Tooltip key={button.label} title={button.label} placement="right">
              <IconButton
                size="small"
                aria-label={button.label}
              >
                {button.icon}
              </IconButton>
            </Tooltip>
          ))}
          <Divider />
          <Tooltip title="Models" placement="right">
            <IconButton
              size="small"
              aria-label="Models"
              onClick={() => setActivePanel("models")}
              sx={activePanel === "models" ? { color: "primary.main", bgcolor: "action.selected" } : {}}
            >
              <LayersRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Revision Control" placement="right">
            <IconButton
              size="small"
              aria-label="Revision Control"
              onClick={() => setActivePanel("revisionControl")}
              sx={activePanel === "revisionControl" ? { color: "primary.main", bgcolor: "action.selected" } : {}}
            >
              <CallSplitRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      </Paper>

      <Paper
        elevation={3}
        sx={{
          position: "absolute",
          top: editorTopBarHeight + 14,
          left: 72,
          zIndex: 12,
          width: 320,
          p: 1.25,
          borderRadius: 2,
        }}
      >
        {activePanel === "revisionControl" ? (
          <Stack spacing={1.5}>
            <Stack direction="row" alignItems="center" spacing={1}>
              <CallSplitRoundedIcon fontSize="small" color="primary" />
              <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
                Revision Control
              </Typography>
            </Stack>
            <Paper variant="outlined" sx={{ p: 1 }}>
              <Typography variant="caption" color="text.secondary">
                Project
              </Typography>
              <Typography variant="body2" sx={{ fontWeight: 600 }}>
                {projectName}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Branch
              </Typography>
              <Stack direction="row" alignItems="center" spacing={0.5}>
                <Chip label={trimmedBranch} size="small" variant="outlined" />
              </Stack>
            </Paper>
            {forkError ? (
              <Alert severity="error" sx={{ py: 0 }}>
                {forkError}
              </Alert>
            ) : null}
            <Button
              id="tutorial-fork-button"
              variant="contained"
              startIcon={<CallSplitRoundedIcon />}
              onClick={() => void handleFork()}
              disabled={isForkLoading || !breadcrumbProjectId}
              fullWidth
            >
              {isForkLoading ? "Forking…" : "Fork Project"}
            </Button>
          </Stack>
        ) : (
          <Stack spacing={1}>
            <Stack direction="row" alignItems="center" spacing={1}>
              <LayersRoundedIcon fontSize="small" />
              <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
                Models
              </Typography>
              <Box sx={{ flex: 1 }} />
              <Button size="small" variant="outlined" startIcon={<AddRoundedIcon />}>
                Add
              </Button>
            </Stack>
            <Paper variant="outlined" sx={{ p: 1 }}>
              <Typography variant="body2" sx={{ fontWeight: 600 }}>
                {activeEntry ? `${activeEntry.projectId}/${activeEntry.branchName}` : "No branch loaded"}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Sync: {activeEntry?.syncStatus ?? "idle"}
              </Typography>
            </Paper>
            <Stack direction="row" spacing={1}>
              <Button size="small" variant="text" startIcon={<CategoryRoundedIcon />}>
                Isolate
              </Button>
              <Button size="small" variant="text" startIcon={<AutoFixHighRoundedIcon />}>
                Style
              </Button>
              <IconButton size="small" sx={{ ml: "auto" }}>
                <SettingsRoundedIcon fontSize="small" />
              </IconButton>
            </Stack>
          </Stack>
        )}
      </Paper>

      <Paper
        elevation={3}
        sx={{
          position: "absolute",
          top: editorTopBarHeight + 14,
          right: 12,
          zIndex: 12,
          width: { xs: 290, sm: 330 },
          p: 1.25,
          borderRadius: 2,
        }}
      >
        <Stack spacing={1}>
          <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
            Selection Info
          </Typography>
          <Stack direction="row" spacing={1}>
            <Button size="small" variant="outlined">Hide</Button>
            <Button size="small" variant="outlined">Isolate</Button>
          </Stack>
          <Paper variant="outlined" sx={{ p: 1 }}>
            <Typography variant="caption" color="text.secondary">Active</Typography>
            <Typography variant="body2" sx={{ fontWeight: 600 }}>
              {activeEntry ? `${activeEntry.projectId}/${activeEntry.branchName}` : "none"}
            </Typography>
          </Paper>
          <Paper variant="outlined" sx={{ p: 1 }}>
            <Typography variant="caption" color="text.secondary">Pending node updates</Typography>
            <Typography variant="body2" sx={{ fontWeight: 600 }}>
              {activePendingRevision?.nodes?.update?.length ?? 0}
            </Typography>
          </Paper>
          {activeEntry?.error ? (
            <Alert severity="error" sx={{ py: 0 }}>
              {activeEntry.error}
            </Alert>
          ) : null}
          <Alert severity="info" sx={{ py: 0 }}>
            Nodes render from API location data; only non-spatial nodes fall back to a temporary grid.
          </Alert>
        </Stack>
      </Paper>

      <Paper
        elevation={2}
        sx={{
          position: "absolute",
          left: 12,
          bottom: 12,
          zIndex: 12,
          px: 1.25,
          py: 0.75,
        }}
      >
        <Stack direction="row" spacing={2}>
          <Typography variant="caption">Branch: {trimmedBranch}</Typography>
          <Typography variant="caption">Sync: {activeEntry?.syncStatus ?? "idle"}</Typography>
          <Typography variant="caption">
            Nodes: {activeModelRevision?.nodes.length ?? 0}
          </Typography>
        </Stack>
      </Paper>

      {isTutorial ? <TutorialTour /> : null}
    </Box>
  );
};
