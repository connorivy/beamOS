import type { Action, Dispatch } from "@reduxjs/toolkit"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type { BeamOsEditor } from "../three-js-editor/BeamOsEditor"
import { clearResults } from "../editors/editorsSlice"

export async function handleClearResults(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  modelId: string | undefined,
  canvasId: string,
  editor: BeamOsEditor,
) {
  dispatch(
    clearResults({
      canvasId,
    }),
  )
  await editor.api.clearCurrentOverlay()
  if (modelId) {
    await apiClient.clearResults(modelId)
  }
}
