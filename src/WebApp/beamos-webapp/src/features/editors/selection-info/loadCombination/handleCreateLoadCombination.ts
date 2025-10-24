import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  LoadCombinationData,
  IStructuralAnalysisApiClientV1,
  LoadCombination,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createLoadCombination,
  removeLoadCombinationById,
  type EditorState,
} from "../../editorsSlice"

export async function handleCreateLoadCombination(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  loadCombinationIdInput: string,
  loadCaseFactors: { [key: string]: number },
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (loadCombinationIdInput || loadCombinationIdInput !== "") {
    console.error("Load Combination ID cannot be specified for new load combination")
    return
  }
  if (Object.keys(loadCaseFactors).length === 0) {
    console.error("At least one load case with factor is required")
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

  // Create the load combination request
  const createLoadCombinationRequest: LoadCombinationData = {
    loadCaseFactors: loadCaseFactors,
  }

  // Call the API to create the load combination
  const createLoadCombinationPromise = apiClient.createLoadCombination(
    editorState.remoteModelId,
    createLoadCombinationRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const loadCombinationResponse: LoadCombination = {
    id: uniqueTempId,
    loadCaseFactors: loadCaseFactors,
  }

  // Optimistically update the store
  dispatch(createLoadCombination({ canvasId, loadCombination: loadCombinationResponse }))

  try {
    const realLoadCombinationResponse = await createLoadCombinationPromise

    // remove the optimistically created load combination and replace with real one
    console.log(
      `Real load combination response received: ${JSON.stringify(realLoadCombinationResponse)}`,
    )
    dispatch(removeLoadCombinationById({ canvasId, loadCombinationId: uniqueTempId }))
    dispatch(createLoadCombination({ canvasId, loadCombination: realLoadCombinationResponse }))
  } catch (error) {
    console.error("Failed to create load combination:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
