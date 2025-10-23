import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  LoadCaseData,
  IStructuralAnalysisApiClientV1,
  LoadCase,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createLoadCase,
  removeLoadCaseById,
  type EditorState,
} from "../../editorsSlice"

export async function handleCreateLoadCase(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  loadCaseIdInput: string,
  name: string,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (loadCaseIdInput || loadCaseIdInput !== "") {
    console.error("Load Case ID cannot be specified for new load case")
    return
  }
  if (!name) {
    console.error("Name is required")
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

  // Create the load case request
  const createLoadCaseRequest: LoadCaseData = {
    name: name,
  }

  // Call the API to create the load case
  const createLoadCasePromise = apiClient.createLoadCase(
    editorState.remoteModelId,
    createLoadCaseRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const loadCaseResponse: LoadCase = {
    id: uniqueTempId,
    name: name,
  }

  // Optimistically update the store
  dispatch(createLoadCase({ canvasId, loadCase: loadCaseResponse }))

  try {
    const realLoadCaseResponse = await createLoadCasePromise

    // remove the optimistically created load case and replace with real one
    console.log(
      `Real load case response received: ${JSON.stringify(realLoadCaseResponse)}`,
    )
    dispatch(removeLoadCaseById({ canvasId, loadCaseId: uniqueTempId }))
    dispatch(createLoadCase({ canvasId, loadCase: realLoadCaseResponse }))
  } catch (error) {
    console.error("Failed to create load case:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
