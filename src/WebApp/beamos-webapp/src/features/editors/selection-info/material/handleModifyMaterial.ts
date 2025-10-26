import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  IStructuralAnalysisApiClientV1,
  MaterialResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type {
  EditorState} from "../../editorsSlice";
import {
  createMaterial,
  removeMaterialById,
} from "../../editorsSlice"
import type { MaterialProperties } from "./materialSelectionSlice"
import { getPressureUnit } from "../../../../utils/type-extensions/UnitTypeContracts"

export async function handleModifyMaterial(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  materialIdInput: string,
  properties: MaterialProperties,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (!materialIdInput || materialIdInput === "") {
    console.error("Material ID must be specified for modify material")
    return
  }
  if (!properties.modulusOfElasticity || !properties.modulusOfRigidity) {
    console.error("All material properties are required")
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

  const pressureUnit = getPressureUnit(
    editorState.model.settings.unitSettings.forceUnit,
    editorState.model.settings.unitSettings.lengthUnit,
  )

  const materialId = Number(materialIdInput)
  const updateMaterialRequest = {
    id: materialId,
    modulusOfElasticity: parseFloat(properties.modulusOfElasticity),
    modulusOfRigidity: parseFloat(properties.modulusOfRigidity),
    pressureUnit: pressureUnit,
  }

  // Optimistically update the store
  const materialResponse: MaterialResponse = {
    id: materialId,
    modelId: editorState.remoteModelId,
    modulusOfElasticity: updateMaterialRequest.modulusOfElasticity,
    modulusOfRigidity: updateMaterialRequest.modulusOfRigidity,
    pressureUnit: updateMaterialRequest.pressureUnit,
  }
  dispatch(createMaterial({ canvasId, material: materialResponse }))

  try {
    const realMaterialResponse = await apiClient.putMaterial(
      editorState.remoteModelId,
      materialId,
      updateMaterialRequest,
    )
    dispatch(removeMaterialById({ canvasId, materialId }))
    dispatch(createMaterial({ canvasId, material: realMaterialResponse }))
  } catch (error) {
    console.error("Failed to modify material:", error)
    // Optionally, implement rollback logic here
  }
}
