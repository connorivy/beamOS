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
                popover.nextButton.addEventListener("click", () => {
                  driverObj.moveNext();
                  // todo: add analytical info
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
                popover.nextButton.addEventListener("click", async () => {
                  // Simulate updated BIM geometry - move node 1 from (12, 16, 0) to (16, 16, 0)
                  console.log("Import BIM Geometry Changes - Starting putSourceModel call");
                  console.log("bimSourceModelId:", bimSourceModelId);
                  console.log("modelId:", modelId);
                  try {
                    if (bimSourceModelId) {
                      console.log("Calling putNode to update node 1...");
                      // Update node 1 to new location
                      await apiClient.putNode(bimSourceModelId, 1, {
                      locationPoint: { x: 16, y: 16, z: 0, lengthUnit: LengthUnit.Foot },
                      restraint: {
                        canTranslateAlongX: true,
                        canTranslateAlongY: true,
                        canTranslateAlongZ: true,
                        canRotateAboutX: true,
                        canRotateAboutY: true,
                        canRotateAboutZ: true,
                      },
                    });
                    console.log("Calling putSourceModel...");
                    await apiClient.putSourceModel(bimSourceModelId, {
                      element1dsToAddOrUpdateByExternalId: [
                        {
                          externalId: "Element-1",
                          startNodeLocation: {
                            x: 0,
                            y: 0,
                            z: 0,
                            lengthUnit: LengthUnit.Foot,
                          },
                          endNodeLocation: {
                            x: 16,
                            y: 16,
                            z: 0,
                            lengthUnit: LengthUnit.Foot,
                          },
                        },
                        {
                          externalId: "Element-2",
                          startNodeLocation: {
                            x: 12,
                            y: 0,
                            z: 0,
                            lengthUnit: LengthUnit.Foot,
                          },
                          endNodeLocation: {
                            x: 16,
                            y: 16,
                            z: 0,
                            lengthUnit: LengthUnit.Foot,
                          },
                        },
                        {
                          externalId: "Element-3",
                          startNodeLocation: {
                            x: 24,
                            y: 0,
                            z: 0,
                            lengthUnit: LengthUnit.Foot,
                          },
                          endNodeLocation: {
                            x: 16,
                            y: 16,
                            z: 0,
                            lengthUnit: LengthUnit.Foot,
                          },
                        },
                      ],
                    });
                    console.log("putSourceModel completed successfully");
                    
                    // Get the proposals and accept the latest one
                    console.log("Getting model proposals...");
                    const proposals = await apiClient.getModelProposals(modelId);
                    console.log("proposals:", proposals);
                    if (proposals && proposals.length > 0) {
                      // Find the most recent proposal (assuming it's the last in the array)
                      const latestProposal = proposals[proposals.length - 1];
                      console.log("Accepting proposal:", latestProposal.id);
                      await apiClient.acceptModelProposal(modelId, latestProposal.id);
                      console.log("Proposal accepted successfully");
                    }
                  } catch (error) {
                    console.error("Error in Import BIM Geometry Changes:", error);
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
