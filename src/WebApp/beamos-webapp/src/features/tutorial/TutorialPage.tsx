import { useState, useEffect } from "react"
import { RemoteEditorComponent } from "../editors/EditorComponent"
import TutorialWelcomeDialog from "./TutorialWelcomeDialog"
import { useApiClient } from "../api-client/ApiClientContext"
import type {
  CreateModelRequest,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import ResponsiveIconSidebarLayout from "../../layouts/ResponsiveIconSidebarLayout"
import ResponsiveSecondarySidebar from "../../components/ResponsiveSecondarySidebar"
import ResultViewer from "../results-viewing/ResultViewer"
import { AngleUnit, ForceUnit, LengthUnit } from "../../utils/type-extensions/UnitTypeContracts"
import { useSearchParams } from "react-router"

const TutorialPage = () => {
  const apiClient = useApiClient()
  const [searchParams] = useSearchParams();
  const [modelId, setModelId] = useState<string | null>(searchParams.get("modelId"))
  const [bimSourceModelId, setBimSourceModelId] = useState<string | null>(searchParams.get("bimSourceModelId"))
  const [dialogOpen, setDialogOpen] = useState(false)
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const canvasId = "tutorial-canvas"

  useEffect(() => {
    // Create a temporary model when the tutorial page loads, but only if no modelId was provided
    const createTutorialModel = async () => {
      // If a modelId is already present from URL params, don't create a new one
      if (searchParams.get("modelId")) {
        return;
      }

      try {
        const request: CreateModelRequest = {
          name: "Tutorial Model",
          description:
            "This model was created as part of the BeamOS tutorial.",
          settings: {
            unitSettings: {
              lengthUnit: LengthUnit.Inch,
              forceUnit: ForceUnit.KilopoundForce,
              angleUnit: AngleUnit.Degree,
            },
            yAxisUp: true,
            analysisSettings: {
              element1DAnalysisType: 1,
            },
          },
          options: {
            isTempModel: true
          }
        }

        const response = await apiClient.createModel(request)
        if (!response.id) {
          throw new Error("Model ID not found in response")
        }
        setModelId(response.id)
        const bimSourceModelId = response.settings.workflowSettings?.bimSourceModelId;
        console.log("response:", response);
        if (!bimSourceModelId) {
          throw new Error("BIM Source Model ID not found in response");
        }
        setBimSourceModelId(bimSourceModelId);
      } catch (error) {
        console.error("Error creating tutorial model:", error)
      }
    }

    void createTutorialModel()
    // Show dialog immediately when component mounts
    setDialogOpen(true)
  }, [apiClient, searchParams])

  const handleDialogClose = () => {
    setDialogOpen(false)
  }

  return (
    <div className="h-full w-full">
      <ResponsiveIconSidebarLayout canvasId={canvasId}>
        {modelId ? (
          <RemoteEditorComponent modelId={modelId} isReadOnly={false} canvasId={canvasId} />
        ) : (
          <div>Loading tutorial model...</div>
        )}
        <ResponsiveSecondarySidebar open={sidebarOpen} onOpen={() => { setSidebarOpen(true) }} onClose={() => { setSidebarOpen(false) }}>
          <ResultViewer canvasId={canvasId} onOpen={() => { setSidebarOpen(true); }} onClose={() => { setSidebarOpen(false); }} />
        </ResponsiveSecondarySidebar>
      </ResponsiveIconSidebarLayout>
      {modelId && bimSourceModelId ? (
        <TutorialWelcomeDialog
          open={dialogOpen}
          onClose={handleDialogClose}
          modelId={modelId}
          bimSourceModelId={bimSourceModelId}
          apiClient={apiClient}
        />
      ) : (
        <></>
      )}
    </div>
  )
}

export default TutorialPage
