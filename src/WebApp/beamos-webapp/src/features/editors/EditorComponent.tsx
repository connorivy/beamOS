import { Dispatch, useEffect, useRef } from "react"
import { useAppDispatch } from "../../app/hooks"
import { addEditor, removeEditor, modelLoaded, addShearForceDiagrams, addMomentDiagrams, addDeflectionDiagrams, setSelectedResultSetId, modelProposalsLoaded } from "./editorsSlice"
import { BeamOsEditor } from "../three-js-editor/BeamOsEditor"
import { EventsApi } from "./EventsApi"
import { useApiClient } from "../api-client/ApiClientContext"
import { useEditors } from "./EditorContext"
import { IStructuralAnalysisApiClientV1, ModelResponse } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { Action } from "@reduxjs/toolkit"

export async function handleModelResponse({
  modelResponse,
  canvasId,
  modelId,
  editor,
  apiClient,
  dispatch,
  signal,
}: {
  modelResponse: ModelResponse
  canvasId: string
  modelId: string
  editor: BeamOsEditor
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>
  signal: AbortSignal
}) {
  if (signal.aborted) return;

  dispatch(modelLoaded({ canvasId, model: modelResponse, remoteModelId: modelId }))

  await editor.api.setSettings(modelResponse.settings)
  await editor.api.createModel(modelResponse)

  if (signal.aborted) return

  if (modelResponse.resultSets && modelResponse.resultSets.length > 0) {
    for (const resultSet of modelResponse.resultSets ?? []) {
      if (signal.aborted) return;

      const diagramResponse = await apiClient.getDiagrams(modelId, resultSet.id, "kn-m")

      if (signal.aborted) return;

      if (diagramResponse.shearDiagrams) {
        dispatch(addShearForceDiagrams({ canvasId, resultSetId: resultSet.id, shearForceResults: diagramResponse.shearDiagrams }))
      }
      if (diagramResponse.momentDiagrams) {
        dispatch(addMomentDiagrams({ canvasId, resultSetId: resultSet.id, momentResults: diagramResponse.momentDiagrams }))
      }
      if (diagramResponse.deflectionDiagrams) {
        dispatch(addDeflectionDiagrams({ canvasId, resultSetId: resultSet.id, deflectionResults: diagramResponse.deflectionDiagrams }))
      }
    }
    dispatch(setSelectedResultSetId({ canvasId: canvasId, selectedResultSetId: modelResponse.resultSets[0].id }))
  }

  const proposals = await apiClient.getModelProposals(modelId)
  if (proposals && proposals.length > 0) {
    dispatch(modelProposalsLoaded({ canvasId, proposals }))
  }
}

type EditorComponentProps = {
  isReadOnly?: boolean
  canvasId: string
}

export const EditorComponent = ({
  isReadOnly = false,
  canvasId,
}: EditorComponentProps) => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null)
  const dispatch = useAppDispatch()
  const eventsApiRef = useRef<EventsApi | null>(null)
  const editors = useEditors()
  eventsApiRef.current ??= new EventsApi(canvasId, dispatch)

  useEffect(() => {
    dispatch(addEditor({ canvasId, isReadOnly, selection: null, selectedType: null, selectedResultSetId: null, model: null }))
    return () => {
      dispatch(removeEditor(canvasId))
    }
  }, [canvasId, dispatch, isReadOnly])

  useEffect(() => {
    if (canvasRef.current && eventsApiRef.current) {
      const editor = BeamOsEditor.createFromId(
        canvasId,
        isReadOnly,
        eventsApiRef.current,
      )
      editors[canvasId] = editor
    }
  }, [canvasId, editors, isReadOnly])

  // Dispose editor on unmount
  useEffect(() => {
    return () => {
      const editor = editors[canvasId]
      editor.dispose()
    }
  }, [canvasId, editors])

  return (
    <canvas
      className="w-full h-full absolute"
      ref={canvasRef}
      id={canvasId}
    // style={{ width: "100%", height: "100%", display: "absolute" }}
    />
  )
}

type RemoteEditorComponentProps = {
  modelId: string
  isReadOnly?: boolean
  canvasId: string
}

export const RemoteEditorComponent = ({
  modelId,
  canvasId,
  isReadOnly = false,
}: RemoteEditorComponentProps) => {
  const apiClient = useApiClient()
  const dispatch = useAppDispatch()
  const editors = useEditors()

  useEffect(() => {
    const abortController = new AbortController();
    const signal = abortController.signal;

    const fetchModel = async () => {
      const modelResponse = await apiClient.getModel(modelId);
      const editor = canvasId in editors ? editors[canvasId] : null;
      if (!editor) {
        console.error("Editor not ready for canvasId:", canvasId);
        return;
      }
      await handleModelResponse({
        modelResponse,
        canvasId,
        modelId,
        editor,
        apiClient,
        dispatch,
        signal,
      });
    };
    fetchModel().catch(console.error);

    return () => {
      abortController.abort();
    };
  }, [apiClient, canvasId, dispatch, editors, modelId]);

  return (
    <EditorComponent
      canvasId={canvasId}
      isReadOnly={isReadOnly}
    />
  )
}
