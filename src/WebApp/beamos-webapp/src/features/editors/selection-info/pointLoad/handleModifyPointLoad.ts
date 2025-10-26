import type { Action, Dispatch } from "@reduxjs/toolkit"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { modifyPointLoad, type EditorState } from "../../editorsSlice"
import type { BeamOsEditor } from "../../../three-js-editor/BeamOsEditor"

export async function handleModifyPointLoad(
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
  editor: BeamOsEditor,
  canvasId: string,
) {
  // Validate input
  if (!pointLoadIdInput || isNaN(Number(pointLoadIdInput))) {
    console.error("Point Load ID must be specified for modify point load")
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

  const pointLoadId = Number(pointLoadIdInput)
  const forceUnit = editorState.model.settings.unitSettings.forceUnit

  // Prepare the update request
  const updatePointLoadRequest = {
    id: pointLoadId,
    modelId: editorState.remoteModelId,
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

  // Optimistically update the store
  dispatch(
    modifyPointLoad({
      canvasId,
      pointLoadId,
      pointLoad: {
        ...updatePointLoadRequest,
        modelId: editorState.remoteModelId,
      },
    }),
  )

  // Optimistically update the UI
  // todo: this method is not implemented yet
  //   await editor.api.updatePointLoad(updatePointLoadRequest)

  // Call the API to update the point load
  try {
    await apiClient.putPointLoad(
      editorState.remoteModelId,
      pointLoadId,
      updatePointLoadRequest,
    )
  } catch (error) {
    console.error("Failed to modify point load:", error)
    // Optionally, implement rollback logic here
  }
}
