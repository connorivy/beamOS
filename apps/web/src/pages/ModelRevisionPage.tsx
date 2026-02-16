import { Box, Stack, Typography } from "@mui/material";
import { useEffect, useMemo, useRef, useState } from "react";
import { BeamOsEditor } from "../components/beamOS.Editor/BeamOsEditor";
import type { IEditorEventsApi } from "../components/beamOS.Editor/EditorApi/EditorEventsApi";
import { EditorConfigurations } from "../components/beamOS.Editor/EditorConfigurations";
import { getModelRouteParams } from "./model-route-utils";
import { useModelsStore } from "../store/models-store";

const noopEditorEventsApi: IEditorEventsApi = {
  dispatchChangeSelectionCommand: async () => {},
  dispatchMoveNodeCommand: async () => {},
  dispatchPutNodeClientCommand: async () => {},
};

const createEditorNodeRestraint = () => ({
  canTranslateAlongX: true,
  canTranslateAlongY: true,
  canTranslateAlongZ: true,
  canRotateAboutX: true,
  canRotateAboutY: true,
  canRotateAboutZ: true,
});

export const ModelRevisionPage = () => {
  const params = useMemo(
    () => getModelRouteParams(window.location.pathname),
    [],
  );
  const modelRevision = useModelsStore((state) => state.modelRevision);
  const revisionNodes = useModelsStore((state) => state.revisionNodes);
  const revisionElement1ds = useModelsStore((state) => state.revisionElement1ds);
  const revisionModelSettings = useModelsStore(
    (state) => state.revisionModelSettings,
  );
  const modelRevisionIsLoading = useModelsStore(
    (state) => state.modelRevisionIsLoading,
  );
  const modelRevisionError = useModelsStore((state) => state.modelRevisionError);
  const editorEvents = useModelsStore((state) => state.editorEvents);
  const loadModelRevision = useModelsStore((state) => state.loadModelRevision);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const editorRef = useRef<BeamOsEditor | null>(null);
  const processedEditorEventsRef = useRef(0);
  const [editorReady, setEditorReady] = useState(false);

  useEffect(() => {
    if (!params) {
      return;
    }
    void loadModelRevision(params.modelId, params.branchName);
  }, [loadModelRevision, params]);

  useEffect(() => {
    if (!canvasRef.current) {
      return;
    }

    const editor = new BeamOsEditor(
      canvasRef.current,
      noopEditorEventsApi,
      new EditorConfigurations(false),
    );
    editorRef.current = editor;
    setEditorReady(true);

    return () => {
      editor.dispose();
      editorRef.current = null;
      setEditorReady(false);
    };
  }, []);

  useEffect(() => {
    if (!editorReady || !modelRevision || !editorRef.current) {
      return;
    }

    const applyRevisionToEditor = async () => {
      const editor = editorRef.current;
      if (!editor) {
        return;
      }

      for (let eventIndex = processedEditorEventsRef.current; eventIndex < editorEvents.length; eventIndex += 1) {
        const event = editorEvents[eventIndex];
        if (event.type !== "model_revision_loaded") {
          continue;
        }
        if (event.revisionId !== modelRevision.id) {
          continue;
        }

        const externalNodes = revisionNodes.filter(
          (node) => node.nodeTypeDescriminator === "external",
        );
        const nodeIdMap = new Map(
          externalNodes.map((node, index) => [node.id, index + 1]),
        );

        const editorNodes = externalNodes.map((node, index) => ({
          id: index + 1,
          modelId: node.modelId,
          locationPoint: { x: index * 3, y: 0, z: 0, lengthUnit: 0 },
          restraint: createEditorNodeRestraint(),
        }));

        const editorElement1ds = revisionElement1ds
          .map((element1d, index) => ({
            id: index + 1,
            modelId: modelRevision.modelId,
            startNodeId: nodeIdMap.get(element1d.startNodeId),
            endNodeId: nodeIdMap.get(element1d.endNodeId),
            materialId: 1,
            sectionProfileId: 1,
            sectionProfileRotation: { value: 0, unit: 0 },
          }))
          .filter(
            (element1d): element1d is {
              id: number;
              modelId: string;
              startNodeId: number;
              endNodeId: number;
              materialId: number;
              sectionProfileId: number;
              sectionProfileRotation: { value: number; unit: number };
            } => !!element1d.startNodeId && !!element1d.endNodeId,
          );

        await editor.api.clear();
        await editor.api.setSettings({
          yAxisUp: revisionModelSettings?.yAxisUp ?? false,
        } as never);
        await editor.api.createNodes(editorNodes as never);
        await editor.api.createElement1ds(editorElement1ds as never);
      }

      processedEditorEventsRef.current = editorEvents.length;
    };

    void applyRevisionToEditor();
  }, [
    editorEvents,
    editorReady,
    modelRevision,
    revisionElement1ds,
    revisionModelSettings?.yAxisUp,
    revisionNodes,
  ]);

  if (!params) {
    return <Typography color="error">Invalid model route.</Typography>;
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h5" sx={{ fontWeight: 600 }}>
        {modelRevision?.name ?? `Model ${params.modelId}`}
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Branch: {params.branchName}
      </Typography>
      {modelRevisionIsLoading ? (
        <Typography color="text.secondary">Loading revision...</Typography>
      ) : null}
      {modelRevisionError ? <Typography color="error">{modelRevisionError}</Typography> : null}
      <Box sx={{ width: "100%", height: "70vh", borderRadius: 1, overflow: "hidden" }}>
        <canvas id="beam-os-editor-canvas" ref={canvasRef} style={{ width: "100%", height: "100%" }} />
      </Box>
    </Stack>
  );
};
