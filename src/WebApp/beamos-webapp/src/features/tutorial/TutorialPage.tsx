import { useState, useEffect } from "react"
import { driver } from "driver.js"
import { RemoteEditorComponent } from "../editors/EditorComponent"
import TutorialWelcomeDialog, { TutorialDialogExitType } from "./TutorialWelcomeDialog"
import { useApiClient } from "../api-client/ApiClientContext"
import type {
  CreateModelRequest,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import ResponsiveIconSidebarLayout from "../../layouts/ResponsiveIconSidebarLayout"
import ResponsiveSecondarySidebar from "../../components/ResponsiveSecondarySidebar"
import ResultViewer from "../results-viewing/ResultViewer"
import { AngleUnit, ForceUnit, LengthUnit } from "../../utils/type-extensions/UnitTypeContracts"
import { useSearchParams } from "react-router"
import "driver.js/dist/driver.css";

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

  const handleDialogClose = (exitType: TutorialDialogExitType) => {
    setDialogOpen(false)
    if (exitType === "completed") {
      let currentButton: HTMLElement | null = null;
      const driverObj = driver({
        allowClose: false,
        showButtons: undefined,
        steps: [
          {
            element: "#model-proposals-view",
            popover: {
              title: "View Model Proposals",
              description:
                "Importing this sample data has created a model proposal for your review. Click here to view and interact with it.",
            },
            onHighlighted: () => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              currentButton = document.getElementById("model-proposals-view");
              console.log("currentButton:", currentButton);
              if (currentButton) {
                currentButton.onclick = () => {
                  console.log("Clicked model proposals view");
                  setTimeout(() => {
                    driverObj.moveNext();
                  }, 200);
                }
              }
            },
          },
          {
            element: "#model-proposals-select",
            popover: {
              title: "Select Model Proposal",
              description:
                "Use this dropdown to select the model proposal created from the sample data. Selecting it will display the proposal in the editor.",
            },
            onHighlighted: () => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              var allProposals = document
                .querySelectorAll('#model-proposals-select ul li')

              for (const prop of allProposals) {
                if (!prop.textContent?.trim().includes('No Selection')) {
                  currentButton = prop as HTMLElement;
                  break;
                }
              }
              if (currentButton) {
                currentButton.onclick = () => {
                  console.log("Clicked proposal select label");
                  setTimeout(() => {
                    driverObj.moveNext();
                  }, 100);
                }
              }
            },
          },
          {
            element: "#tutorial-canvas",
            popover: {
              title: "Explore the Model",
              description:
                "Great! The model proposal is now displayed in the editor. You can explore the model, inspect elements, and get familiar with the BeamOS interface.",
            },
            onHighlighted: (popover, { config, state }) => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              currentButton = document.getElementById("tutorial-canvas");
              if (currentButton) {
                currentButton.onclick = () => {
                  setTimeout(() => {
                    driverObj.moveNext();
                  }, 5000);
                }
              }
              const firstButton = document.createElement("button");
              firstButton.innerText = "Go to First";
              popover.footerButtons.appendChild(firstButton);

              firstButton.addEventListener("click", () => {
                driverObj.drive(0);
              });
            }
          }
        ]
      })

      driverObj.drive()
    }
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
          canvasId={canvasId}
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
