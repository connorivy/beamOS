import { useEffect, useRef } from "react"
import { useAppDispatch } from "../../app/hooks"
import { addEditor, removeEditor, modelLoaded, addShearForceDiagrams, addMomentDiagrams, addDeflectionDiagrams, setSelectedResultSetId } from "./editorsSlice"
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
    let cancelled = false

    const fetchModel = async () => {
      const modelResponse = await apiClient.getModel(modelId)

      if (cancelled) {
        return
      }

      dispatch(modelLoaded({ canvasId, model: modelResponse, remoteModelId: modelId }))

      const editor = canvasId in editors ? editors[canvasId] : null
      if (!editor) {
        console.error("Editor not ready for canvasId:", canvasId)
        return
      }

      await editor.api.setSettings(modelResponse.settings)
      await editor.api.createModel(modelResponse)

      // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
      if (cancelled) return

      if (modelResponse.resultSets && modelResponse.resultSets.length > 0) {
        for (const resultSet of modelResponse.resultSets ?? []) {
          // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
          if (cancelled) break

          const diagramResponse = await apiClient.getDiagrams(modelId, resultSet.id, "kn-m")

          // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
          if (cancelled) break

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
    }
    fetchModel().catch(console.error)

    return () => {
      cancelled = true
    }
  }, [apiClient, canvasId, dispatch, editors, modelId])

  return (
    <EditorComponent
      canvasId={canvasId}
      isReadOnly={isReadOnly}
    />
  )
}
