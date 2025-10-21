import { useRef, useState } from "react"
import { RemoteEditorComponent } from "./EditorComponent"
import { useParams } from "react-router-dom"
import AppBarMain from "../../components/AppBarMain"
import SidebarMain from "../../components/SidebarMain"
import SelectionInfo from "./selection-info/SelectionInfo"

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
  // Get modelId from route params
  const { modelId } = useParams<{ modelId: string }>()
  // (Panel content stack and lock state removed as unused)

  // Sidebar state
  const [sidebarOpen, setSidebarOpen] = useState(false)
  // const theme = useTheme();
  // const isLargeScreen = useMediaQuery(theme.breakpoints.up('md'));

  if (!modelId) {
    // Handle error, show a message, or redirect
    return <div>Model ID is required.</div>
  }

  // Panel navigation handlers
  // const goBack = () => {
  //     if (!panelContentLocked && panelContentStack.length > 0) {
  //         setPanelContentStack(stack => stack.slice(0, -1));
  //     }
  // };

  // Layout and rendering
  // modelId will be used for child components and logic (see Blazor version)
  return (
    <div
      className="relative h-full w-full"
      style={{ display: "flex", flexDirection: "column", height: "100vh" }}
    >
      <AppBarMain
        onSidebarToggle={() => {
          setSidebarOpen(true)
        }}
      />
      <div style={{ display: "flex", flex: 1, position: "relative" }}>
        <SidebarMain
          open={sidebarOpen}
          onClose={() => {
            setSidebarOpen(false)
          }}
          drawerWidth={320}
        >
          <SelectionInfo canvasId={canvasId} />
        </SidebarMain>
        <div style={{ flex: 1, position: "relative" }}>
          <RemoteEditorComponent modelId={modelId} isReadOnly={false} canvasId={canvasId} />
        </div>
      </div>
    </div>
  )
}

export default ModelEditor
