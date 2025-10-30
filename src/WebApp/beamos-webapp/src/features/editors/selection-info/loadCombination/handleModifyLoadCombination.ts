import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  LoadCombinationData,
  IStructuralAnalysisApiClientV1,
  LoadCombination,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createLoadCombination,
  type EditorState,
} from "../../editorsSlice"

export async function handleModifyLoadCombination(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  loadCombinationId: number,
  loadCaseFactors: Record<string, number>,
  editorState: EditorState,
  canvasId: string,
) {
  if (!loadCombinationId && loadCombinationId !== 0) {
    console.error("Load Combination ID must be specified for modification")
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

  // Prepare the update request
  const updateLoadCombinationRequest: LoadCombinationData = {
    loadCaseFactors: loadCaseFactors,
  }

  // Optimistically update the store
  const optimisticLoadCombination: LoadCombination = {
    id: loadCombinationId,
    loadCaseFactors: loadCaseFactors,
  }
  dispatch(
    createLoadCombination({
      canvasId,
      loadCombination: optimisticLoadCombination,
    }),
  )

  try {
    const realLoadCombinationResponse = await apiClient.putLoadCombination(
      editorState.remoteModelId,
      loadCombinationId,
      updateLoadCombinationRequest,
    )
    // Replace with real response
    dispatch(
      createLoadCombination({
        canvasId,
        loadCombination: realLoadCombinationResponse,
      }),
    )
  } catch (error) {
    console.error("Failed to modify load combination:", error)
    // Optionally, rollback logic could be added here
  }
}
