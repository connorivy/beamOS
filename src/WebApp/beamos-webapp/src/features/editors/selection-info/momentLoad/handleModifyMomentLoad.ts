import type { Action, Dispatch } from "@reduxjs/toolkit"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { modifyMomentLoad, type EditorState } from "../../editorsSlice"
import type { BeamOsEditor } from "../../../three-js-editor/BeamOsEditor"
import { getTorqueUnit } from "../../../../utils/type-extensions/UnitTypeContracts"

export async function handleModifyMomentLoad(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  momentLoadIdInput: string,
  loadCaseId: string,
  nodeId: string,
  magnitude: string,
  directionX: string,
  directionY: string,
  directionZ: string,
  editorState: EditorState,
  _editor: BeamOsEditor,
  canvasId: string,
) {
  // Validate input
  if (!momentLoadIdInput || isNaN(Number(momentLoadIdInput))) {
    console.error("Moment Load ID must be specified for modify moment load")
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

  const momentLoadId = Number(momentLoadIdInput)
  const torqueUnit = getTorqueUnit(
    editorState.model.settings.unitSettings.lengthUnit,
    editorState.model.settings.unitSettings.forceUnit,
  )

  // Prepare the update request
  const updateMomentLoadRequest = {
    id: momentLoadId,
    modelId: editorState.remoteModelId,
    nodeId: parseInt(nodeId),
    loadCaseId: parseInt(loadCaseId),
    torque: {
      value: parseFloat(magnitude),
      unit: torqueUnit,
    },
    axisDirection: {
      x: parseFloat(directionX),
      y: parseFloat(directionY),
      z: parseFloat(directionZ),
    },
  }

  // Optimistically update the store
  dispatch(
    modifyMomentLoad({
      canvasId,
      momentLoadId,
      momentLoad: {
        ...updateMomentLoadRequest,
        modelId: editorState.remoteModelId,
      },
    }),
  )

  // Optimistically update the UI
  // todo: this method is not implemented yet
  // await editor.api.updateMomentLoad(updateMomentLoadRequest)

  // Call the API to update the moment load
  try {
    await apiClient.putMomentLoad(
      editorState.remoteModelId,
      momentLoadId,
      updateMomentLoadRequest,
    )
  } catch (error) {
    console.error("Failed to modify moment load:", error)
    // Optionally, implement rollback logic here
  }
}
