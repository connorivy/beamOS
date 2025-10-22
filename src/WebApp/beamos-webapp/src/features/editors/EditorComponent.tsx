import { useEffect, useRef } from "react"
import { useAppDispatch } from "../../app/hooks"
import { addEditor, updateEditor, removeEditor, modelLoaded } from "./editorsSlice"
import { BeamOsEditor } from "../three-js-editor/BeamOsEditor"
import { EventsApi } from "./EventsApi"
import { useApiClient } from "../api-client/ApiClientContext"

type EditorComponentProps = {
  isReadOnly?: boolean
  canvasId: string
  onEditorReady?: (editor: BeamOsEditor) => void
}

export const EditorComponent = ({
  isReadOnly = false,
  canvasId,
  onEditorReady,
}: EditorComponentProps) => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null)
  const editorRef = useRef<BeamOsEditor | null>(null)
  const dispatch = useAppDispatch()
  const eventsApiRef = useRef<EventsApi | null>(null)
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
      editorRef.current = editor
      // Store editor reference in Redux
      dispatch(updateEditor({ canvasId, changes: { editorRef: editor } }))
      onEditorReady?.(editor)
    }
  }, [canvasId, isReadOnly, onEditorReady, dispatch])

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
// remoteEditorComponent that inhertits from EditorComponent
export const RemoteEditorComponent = ({
  modelId,
  canvasId,
  isReadOnly = false,
}: RemoteEditorComponentProps) => {
  const apiClient = useApiClient()
  const dispatch = useAppDispatch()
  const editorRef = useRef<BeamOsEditor | null>(null)

  useEffect(() => {
    const fetchModel = async () => {
      const modelResponse = await apiClient.getModel(modelId)
      dispatch(modelLoaded({ canvasId, model: modelResponse, remoteModelId: modelId }))
      await editorRef.current?.api.createModel(modelResponse)
    }
    fetchModel().catch(console.error)
  }, [apiClient, canvasId, dispatch, modelId])

  const handleEditorReady = (editor: BeamOsEditor) => {
    editorRef.current = editor
  }

  return (
    <EditorComponent
      canvasId={canvasId}
      isReadOnly={isReadOnly}
      onEditorReady={handleEditorReady}
    />
  )
}
