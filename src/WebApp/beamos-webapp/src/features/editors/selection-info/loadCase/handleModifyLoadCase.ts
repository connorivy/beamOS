import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  IStructuralAnalysisApiClientV1,
  LoadCase,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { editorsSlice, type EditorState } from "../../editorsSlice"

export async function handleModifyLoadCase(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  loadCaseId: number,
  name: string,
  editorState: EditorState,
  canvasId: string,
) {
  if (typeof loadCaseId !== "number" || isNaN(loadCaseId)) {
    console.error("Load Case ID must be specified for modifying a load case")
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

  // Prepare the updated load case data
  const updatedLoadCase: LoadCase = {
    id: loadCaseId,
    name: name,
  }

  // Optimistically update the store
  dispatch(
    editorsSlice.actions.modifyLoadCase({
      canvasId,
      loadCaseId,
      loadCase: updatedLoadCase,
    }),
  )

  try {
    // Call the API to update the load case
    const realLoadCaseResponse = await apiClient.putLoadCase(
      editorState.remoteModelId,
      loadCaseId,
      { name },
    )
    // Update the store with the real response
    dispatch(
      editorsSlice.actions.modifyLoadCase({
        canvasId,
        loadCaseId,
        loadCase: realLoadCaseResponse,
      }),
    )
  } catch (error) {
    console.error("Failed to modify load case:", error)
    // Optionally, rollback logic could be added here
  }
}
