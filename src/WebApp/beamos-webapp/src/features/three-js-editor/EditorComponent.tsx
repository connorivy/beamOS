import { useEffect, useRef } from "react"
import { BeamOsEditor } from "./BeamOsEditor"
import { useApiClient } from "../api-client/ApiClientContext"

// Generates a unique id for the canvas element
function generateUniqueId(prefix = "editor-canvas-") {
    return prefix + Math.random().toString(36).substring(2, 11)
}

type EditorComponentProps = {
    isReadOnly?: boolean
    onEditorReady?: (editor: BeamOsEditor) => void
}

export const EditorComponent = ({ isReadOnly = false, onEditorReady }: EditorComponentProps) => {
    const canvasRef = useRef<HTMLCanvasElement | null>(null)
    const idRef = useRef<string>(generateUniqueId())
    const editorRef = useRef<BeamOsEditor | null>(null)

    useEffect(() => {
        if (canvasRef.current) {
            const editor = BeamOsEditor.createFromId(idRef.current, isReadOnly)
            editorRef.current = editor
            onEditorReady?.(editor)
        }
    }, [isReadOnly, onEditorReady])

    return (
        <canvas
            ref={canvasRef}
            id={idRef.current}
            style={{ width: "100%", height: "100%", display: "block" }}
            tabIndex={0}
        />
    )
}

type RemoteEditorComponentProps = {
    modelId: string
    isReadOnly?: boolean
}
// remoteEditorComponent that inhertits from EditorComponent
export const RemoteEditorComponent = ({ modelId, isReadOnly = false }: RemoteEditorComponentProps) => {
    const apiClient = useApiClient()
    const editorRef = useRef<BeamOsEditor | null>(null)

    useEffect(() => {
        const fetchModel = async () => {
            const modelResponse = await apiClient.getModel(modelId)
            await editorRef.current?.api.createModel(modelResponse)
        }
        fetchModel().catch(console.error)
    }, [apiClient, modelId])

    const handleEditorReady = (editor: BeamOsEditor) => {
        editorRef.current = editor
    }

    return (
        <EditorComponent
            isReadOnly={isReadOnly}
            onEditorReady={handleEditorReady}
        />
    )
}