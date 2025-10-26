import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  IStructuralAnalysisApiClientV1,
  Element1dResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { modifyElement1d } from "../../editorsSlice"
import { AngleUnit } from "../../../../utils/type-extensions/UnitTypeContracts"
import type { EditorState } from "../../editorsSlice"
import type { BeamOsEditor } from "../../../three-js-editor/BeamOsEditor"

export async function handleModifyElement1d(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  element1dIdInput: string,
  startNodeId: string,
  endNodeId: string,
  materialId: string,
  sectionProfileId: string,
  sectionProfileRotation: string,
  editor: BeamOsEditor,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (!element1dIdInput || element1dIdInput === "") {
    console.error("Element1d ID must be specified for modification")
    return
  }
  if (!startNodeId || !endNodeId || !materialId || !sectionProfileId) {
    console.error(
      "Start node ID, end node ID, material ID, and section profile ID are required",
    )
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

  const element1dId = parseInt(element1dIdInput)

  // Prepare the element1d response for optimistic update
  const element1dResponse: Element1dResponse = {
    id: element1dId,
    modelId: editorState.remoteModelId,
    startNodeId: parseInt(startNodeId),
    endNodeId: parseInt(endNodeId),
    materialId: parseInt(materialId),
    sectionProfileId: parseInt(sectionProfileId),
    sectionProfileRotation: {
      value:
        sectionProfileRotation !== "" ? parseFloat(sectionProfileRotation) : 0,
      unit:
        editorState.model.settings.unitSettings.angleUnit ?? AngleUnit.Degree,
    },
  }

  // Optimistically update the store
  await editor.api.updateElement1d(element1dResponse)
  dispatch(
    modifyElement1d({ canvasId, element1dId, element1d: element1dResponse }),
  )

  try {
    // Call the API to modify the element1d
    const realElement1dResponse = await apiClient.putElement1d(
      editorState.remoteModelId,
      element1dId,
      element1dResponse,
    )
    // Update the store with the real response
    dispatch(
      modifyElement1d({
        canvasId,
        element1dId,
        element1d: realElement1dResponse,
      }),
    )
  } catch (error) {
    console.error("Failed to modify element1d:", error)
    // Optionally, rollback logic could be added here
  }
}
