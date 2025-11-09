import { useState, useEffect } from "react"
import { driver, PopoverDOM } from "driver.js"
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
      let popover: PopoverDOM | null = null;
      const driverObj = driver({
        allowClose: false,
        showButtons: ["next", "close"],
        onPopoverRender: (pop, { }) => {
          popover = pop;
          popover.nextButton.style.display = "none";
        },
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
                  }, 50);
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
                  }, 50);
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
            onHighlighted: () => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              if (popover) {
                popover.nextButton.style.display = "block";
                popover.nextButton.addEventListener("click", () => {
                  driverObj.moveNext();
                  popover!.nextButton.style.display = "none";
                });
              }
            }
          },
          {
            element: () => {
              var allProposals = document
                .querySelectorAll('#model-proposals-select ul li')

              for (const prop of allProposals) {
                if (!prop.textContent?.trim().includes('No Selection')) {
                  return prop as Element;
                }
              }

              return document.getElementById('model-proposals-select') || document.body;
            },
            popover: {
              title: "Accept Model Proposal",
              description:
                "If you are satisfied with the model proposal, you can accept it to integrate the proposed changes into your main model.",
            },
            onHighlighted: () => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              var allProposals = document
                .querySelectorAll('#model-proposals-select ul li')

              for (const prop of allProposals) {
                if (!prop.textContent?.trim().includes('No Selection')) {
                  // todo: disable the reject button
                  var acceptButton = prop.querySelector('button[aria-label="accept"]')
                  if (!acceptButton) {
                    console.error("Accept button not found on proposal item.");
                    return;
                  }
                  currentButton = acceptButton as HTMLElement;
                  currentButton.onclick = () => {
                    driverObj.moveNext();
                  }
                  break;
                }
              }
            },
          },
          {
            popover: {
              title: "Add Analytical Info",
              description: "You can add analytical information such as loads and supports to your model. For this tutorial, we'll simulate this step.",
            },
            onHighlighted: () => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              if (popover) {
                popover.nextButton.style.display = "block";
                popover.nextButton.addEventListener("click", async () => {
                  // Add LoadCase and PointLoads via patchModel endpoint
                  console.log("Next button clicked, modelId:", modelId);
                  if (modelId) {
                    console.log("Patching model with LoadCase and PointLoads");
                    const result = await apiClient.patchModel(modelId, {
                      loadCases: [
                        { id: 1, name: "Load Case 1" }
                      ],
                      pointLoads: [
                        {
                          force: { value: 150, unit: ForceUnit.KilopoundForce },
                          direction: { x: 1, y: 0, z: 0 },
                          nodeId: 1,
                          loadCaseId: 1,
                        },
                        {
                          force: { value: 300, unit: ForceUnit.KilopoundForce },
                          direction: { x: 0, y: -1, z: 0 },
                          nodeId: 1,
                          loadCaseId: 1,
                        },
                      ]
                    });
                    console.log("PatchModel result:", result);
                  } else {
                    console.error("Model ID is not available");
                  }
                  driverObj.moveNext();
                  popover!.nextButton.style.display = "none";
                });
              }
            }
          },
          {
            element: "#model-proposals-select",
            popover: {
              title: "Import BIM Geometry Changes",
              description: "As your BIM model evolves, you can push those changes to beamOS where they can be reviewed as model proposals",
            },
            onHighlighted: () => {
              if (currentButton) {
                currentButton.onclick = null;
              }
              if (popover) {
                popover.nextButton.style.display = "block";
                popover.nextButton.addEventListener("click", () => {
                  driverObj.moveNext();
                  popover!.nextButton.style.display = "none";
                });
              }
            }
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
                  }, 50);
                }
              }
            },
          },
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
