import { useRef } from "react"
import { RemoteEditorComponent } from "./EditorComponent"
import { useParams } from "react-router-dom"
import { Toolbar } from "./toolbar/Toolbar"
import ResponsiveIconSidebarLayout from "../../layouts/ResponsiveIconSidebarLayout"

// Generates a unique id for the canvas element
function generateUniqueId(prefix = "editor-canvas-") {
  return prefix + Math.random().toString(36).substring(2, 11)
}

// Types for panel content
export enum PanelContent {
  None,
  ProposalInfo,
  Element1dResults,
  ModelRepairScenarioCreator,
}

const ModelEditor = () => {
  const canvasId = useRef(generateUniqueId()).current
  const { modelId } = useParams<{ modelId: string }>()

  if (!modelId) {
    // Handle error, show a message, or redirect
    return <div>Model ID is required.</div>
  }

  return (
    <>
      <div className="flex flex-row w-full  h-full">
        <div className="h-full w-full relative">
          <RemoteEditorComponent modelId={modelId} isReadOnly={false} canvasId={canvasId} />
          <div className="flex items-center justify-center absolute left-4 right-4 top-4">
            <Toolbar canvasId={canvasId} />
          </div>
        </div>
      </div>
    </>
  )
}

export default ModelEditor

const ModelEditorPage = () => {
  const canvasId = useRef(generateUniqueId()).current
  const { modelId } = useParams<{ modelId: string }>()

  if (!modelId) {
    // Handle error, show a message, or redirect
    return <div>Model ID is required.</div>
  }

  return (
    <>
      <div className="h-full w-full">
        <ResponsiveIconSidebarLayout canvasId={canvasId}>
          <RemoteEditorComponent modelId={modelId} isReadOnly={false} canvasId={canvasId} />
        </ResponsiveIconSidebarLayout>
      </div>
    </>
  )
}

export { ModelEditorPage }