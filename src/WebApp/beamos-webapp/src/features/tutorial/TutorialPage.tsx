import { useState, useEffect } from "react"
import { RemoteEditorComponent } from "../editors/EditorComponent"
import AppBarMain from "../../components/AppBarMain"
import SidebarMain from "../../components/SidebarMain"
import SelectionInfo from "../editors/selection-info/SelectionInfo"
import TutorialWelcomeDialog from "./TutorialWelcomeDialog"
import { useApiClient } from "../api-client/ApiClientContext"
import {
  CreateModelRequest,
  ModelSettings,
  UnitSettings,
  AnalysisSettings,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

const TutorialPage = () => {
  const apiClient = useApiClient()
  const [modelId, setModelId] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [sidebarOpen, setSidebarOpen] = useState(false)

  useEffect(() => {
    // Create a temporary model when the tutorial page loads
    const createTutorialModel = async () => {
      try {
        const unitSettings = new UnitSettings()
        unitSettings.lengthUnit = 1 // K_IN
        unitSettings.forceUnit = 2
        unitSettings.angleUnit = 1

        const analysisSettings = new AnalysisSettings()
        analysisSettings.element1DAnalysisType = 1

        const modelSettings = new ModelSettings()
        modelSettings.unitSettings = unitSettings
        modelSettings.analysisSettings = analysisSettings
        modelSettings.yAxisUp = true

        const request = new CreateModelRequest()
        request.name = "Tutorial Model"
        request.description =
          "This model was created as part of the BeamOS tutorial."
        request.settings = modelSettings

        const response = await apiClient.createTempModel(request)
        if (response.id) {
          setModelId(response.id)
        }
      } catch (error) {
        console.error("Error creating tutorial model:", error)
      }
    }

    void createTutorialModel()
    // Show dialog immediately when component mounts
    setDialogOpen(true)
  }, [apiClient])

  const handleDialogClose = () => {
    setDialogOpen(false)
  }

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
          <SelectionInfo />
        </SidebarMain>
        <div style={{ flex: 1, position: "relative" }}>
          {modelId ? (
            <RemoteEditorComponent modelId={modelId} isReadOnly={false} />
          ) : (
            <div>Loading tutorial...</div>
          )}
        </div>
      </div>
      <TutorialWelcomeDialog open={dialogOpen} onClose={handleDialogClose} />
    </div>
  )
}

export default TutorialPage
