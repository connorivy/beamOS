import { useEffect, useRef } from "react"
import { useAppDispatch } from "../../app/hooks"
import { addEditor, updateEditor, removeEditor, modelLoaded } from "./editorsSlice"
import { BeamOsEditor } from "../three-js-editor/BeamOsEditor"
import { EventsApi } from "./EventsApi"
import { useApiClient } from "../api-client/ApiClientContext"
import { useEditors } from "./EditorContext"

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
  eventsApiRef.current ??= new EventsApi(dispatch)

  // Register editor in Redux on mount, update on prop change, remove on unmount
  useEffect(() => {
    dispatch(addEditor({ canvasId, isReadOnly, selection: null, model: null }))
    return () => {
      dispatch(removeEditor(canvasId))
    }
  }, [canvasId, dispatch, isReadOnly])

  useEffect(() => {
    const id = canvasId
    dispatch(updateEditor({ canvasId: id, changes: { isReadOnly } }))
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

  return (
    <canvas
      ref={canvasRef}
      id={canvasId}
      style={{ width: "100%", height: "100%", display: "block" }}
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
    const fetchModel = async () => {
      const modelResponse = await apiClient.getModel(modelId)
      dispatch(modelLoaded({ canvasId, model: modelResponse, remoteModelId: modelId }))
      await editors[canvasId].api.createModel(modelResponse)
    }
    fetchModel().catch(console.error)
  }, [apiClient, canvasId, dispatch, editors, modelId])

  return (
    <EditorComponent
      canvasId={canvasId}
      isReadOnly={isReadOnly}
    />
  )
}
