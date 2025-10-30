import { useRef, useState } from "react"
import { RemoteEditorComponent } from "./EditorComponent"
import { useParams } from "react-router"
import ResponsiveIconSidebarLayout from "../../layouts/ResponsiveIconSidebarLayout"
import ResponsiveSecondarySidebar from "../../components/ResponsiveSecondarySidebar"
import ResultViewer from "../results-viewing/ResultViewer"

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

const ModelEditorPage = () => {
  const canvasId = useRef(generateUniqueId()).current
  const { modelId } = useParams<{ modelId: string }>()
  const [sidebarOpen, setSidebarOpen] = useState(true)

  if (!modelId) {
    // Handle error, show a message, or redirect
    return <div>Model ID is required.</div>
  }

  return (
    <div className="h-full w-full">
      <ResponsiveIconSidebarLayout canvasId={canvasId}>
        <RemoteEditorComponent modelId={modelId} isReadOnly={false} canvasId={canvasId} />
        <ResponsiveSecondarySidebar open={sidebarOpen} onOpen={() => { setSidebarOpen(true) }} onClose={() => { setSidebarOpen(false) }}>
          <ResultViewer canvasId={canvasId} onOpen={() => { setSidebarOpen(true); }} onClose={() => { setSidebarOpen(false); }} />
        </ResponsiveSecondarySidebar>
      </ResponsiveIconSidebarLayout>
    </div>
  )
}

export { ModelEditorPage }