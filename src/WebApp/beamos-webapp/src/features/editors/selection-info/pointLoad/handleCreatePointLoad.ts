import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  CreatePointLoadRequest,
  IStructuralAnalysisApiClientV1,
  PointLoadResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createPointLoad,
  removePointLoadById,
  type EditorState,
} from "../../editorsSlice"

export async function handleCreatePointLoad(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  pointLoadIdInput: string,
  loadCaseId: string,
  nodeId: string,
  magnitude: string,
  directionX: string,
  directionY: string,
  directionZ: string,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (pointLoadIdInput || pointLoadIdInput !== "") {
    console.error("Point Load ID cannot be specified for new point load")
    return
  }
  if (!loadCaseId) {
    console.error("Load Case ID is required")
    return
  }
  if (!nodeId) {
    console.error("Node ID is required")
    return
  }
  if (!magnitude) {
    console.error("Magnitude is required")
    return
  }
  if (directionX === "" || directionY === "" || directionZ === "") {
    console.error("All direction components are required")
    return
  }
  if (!editorState.remoteModelId) {
    console.error("Remote model ID is not available")
    return
  }
  if (!editorState.model) {
    console.error("Editor model is not loaded")
    return
  }

  const forceUnit = editorState.model.settings.unitSettings.forceUnit

  // Create the point load request
  const createPointLoadRequest: CreatePointLoadRequest = {
    nodeId: parseInt(nodeId),
    loadCaseId: parseInt(loadCaseId),
    force: {
      value: parseFloat(magnitude),
      unit: forceUnit,
    },
    direction: {
      x: parseFloat(directionX),
      y: parseFloat(directionY),
      z: parseFloat(directionZ),
    },
  }

  // Call the API to create the point load
  const createPointLoadPromise = apiClient.createPointLoad(
    editorState.remoteModelId,
    createPointLoadRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const pointLoadResponse: PointLoadResponse = {
    id: uniqueTempId,
    modelId: editorState.remoteModelId,
    nodeId: createPointLoadRequest.nodeId,
    loadCaseId: createPointLoadRequest.loadCaseId,
    force: createPointLoadRequest.force,
    direction: createPointLoadRequest.direction,
  }

  // Optimistically update the store
  dispatch(createPointLoad({ canvasId, pointLoad: pointLoadResponse }))

  try {
    const realPointLoadResponse = await createPointLoadPromise

    // remove the optimistically created point load and replace with real one
    dispatch(removePointLoadById({ canvasId, pointLoadId: uniqueTempId }))
    dispatch(createPointLoad({ canvasId, pointLoad: realPointLoadResponse }))
  } catch (error) {
    console.error("Failed to create point load:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
