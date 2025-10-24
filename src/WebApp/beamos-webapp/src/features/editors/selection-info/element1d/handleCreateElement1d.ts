import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  CreateElement1dRequest,
  IStructuralAnalysisApiClientV1,
  Element1dResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createElement1d,
  removeElement1dById,
  type EditorState,
} from "../../editorsSlice"
import { AngleUnit } from "../../../../utils/type-extensions/UnitTypeContracts"

export async function handleCreateElement1d(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  element1dIdInput: string,
  startNodeId: string,
  endNodeId: string,
  materialId: string,
  sectionProfileId: string,
  sectionProfileRotation: string,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (element1dIdInput || element1dIdInput !== "") {
    console.error("Element1d ID cannot be specified for new element1d")
    return
  }
  if (!startNodeId || !endNodeId || !materialId || !sectionProfileId) {
    console.error("Start node ID, end node ID, material ID, and section profile ID are required")
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

  // Create the element1d request
  const createElement1dRequest: CreateElement1dRequest = {
    id: undefined,
    startNodeId: parseInt(startNodeId),
    endNodeId: parseInt(endNodeId),
    materialId: parseInt(materialId),
    sectionProfileId: parseInt(sectionProfileId),
    sectionProfileRotation: sectionProfileRotation ? {
      value: parseFloat(sectionProfileRotation),
      unit: editorState.model.settings.unitSettings.angleUnit ?? AngleUnit.Degree,
    } : undefined,
  }

  // Call the API to create the element1d
  const createElement1dPromise = apiClient.createElement1d(
    editorState.remoteModelId,
    createElement1dRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const element1dResponse: Element1dResponse = {
    id: uniqueTempId,
    modelId: editorState.remoteModelId,
    startNodeId: parseInt(startNodeId),
    endNodeId: parseInt(endNodeId),
    materialId: parseInt(materialId),
    sectionProfileId: parseInt(sectionProfileId),
    sectionProfileRotation: createElement1dRequest.sectionProfileRotation ?? {
      value: 0,
      unit: editorState.model.settings.unitSettings.angleUnit ?? AngleUnit.Degree,
    },
  }

  // Optimistically update the store
  dispatch(createElement1d({ canvasId, element1d: element1dResponse }))

  try {
    const realElement1dResponse = await createElement1dPromise

    // remove the optimistically created element1d and replace with real one
    console.log(
      `Real element1d response received: ${JSON.stringify(realElement1dResponse)}`,
    )
    dispatch(removeElement1dById({ canvasId, element1dId: uniqueTempId }))
    dispatch(createElement1d({ canvasId, element1d: realElement1dResponse }))
  } catch (error) {
    console.error("Failed to create element1d:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
