import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  CreateMomentLoadRequest,
  IStructuralAnalysisApiClientV1,
  MomentLoadResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createMomentLoad,
  removeMomentLoadById,
  type EditorState,
} from "../../editorsSlice"
import type { Direction } from "./momentLoadSelectionSlice"
import { getTorqueUnit } from "../../../../utils/type-extensions/UnitTypeContracts"

export async function handleCreateMomentLoad(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  momentLoadIdInput: string,
  loadCaseId: string,
  nodeId: string,
  magnitude: string,
  direction: Direction,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (momentLoadIdInput || momentLoadIdInput !== "") {
    console.error("Moment Load ID cannot be specified for new moment load")
    return
  }
  if (!loadCaseId || !nodeId || !magnitude) {
    console.error("Load Case ID, Node ID, and Magnitude are required")
    return
  }
  if (!direction.x || !direction.y || !direction.z) {
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

  const torqueUnit = getTorqueUnit(
    editorState.model.settings.unitSettings.lengthUnit,
    editorState.model.settings.unitSettings.forceUnit,
  )

  // Create the moment load request
  const createMomentLoadRequest: CreateMomentLoadRequest = {
    id: undefined,
    nodeId: parseInt(nodeId),
    loadCaseId: parseInt(loadCaseId),
    torque: {
      value: parseFloat(magnitude),
      unit: torqueUnit,
    },
    axisDirection: {
      x: parseFloat(direction.x),
      y: parseFloat(direction.y),
      z: parseFloat(direction.z),
    },
  }

  // Call the API to create the moment load
  const createMomentLoadPromise = apiClient.createMomentLoad(
    editorState.remoteModelId,
    createMomentLoadRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const momentLoadResponse: MomentLoadResponse = {
    id: uniqueTempId,
    nodeId: createMomentLoadRequest.nodeId,
    loadCaseId: createMomentLoadRequest.loadCaseId,
    modelId: editorState.remoteModelId,
    torque: createMomentLoadRequest.torque,
    axisDirection: createMomentLoadRequest.axisDirection,
  }

  // Optimistically update the store
  dispatch(createMomentLoad({ canvasId, momentLoad: momentLoadResponse }))

  try {
    const realMomentLoadResponse = await createMomentLoadPromise

    // remove the optimistically created moment load and replace with real one
    dispatch(removeMomentLoadById({ canvasId, momentLoadId: uniqueTempId }))
    dispatch(createMomentLoad({ canvasId, momentLoad: realMomentLoadResponse }))
  } catch (error) {
    console.error("Failed to create moment load:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
