import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  CreateMaterialRequest2,
  IStructuralAnalysisApiClientV1,
  MaterialResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import {
  createMaterial,
  removeMaterialById,
  type EditorState,
} from "../../editorsSlice"
import type { MaterialProperties } from "./materialSelectionSlice"
import { getPressureUnit } from "../../../../utils/type-extensions/UnitTypeContracts"

export async function handleCreateMaterial(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  materialIdInput: string,
  properties: MaterialProperties,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (materialIdInput && materialIdInput !== "") {
    console.error("Material ID cannot be specified for new material")
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

  // Create the material request
  const createMaterialRequest: CreateMaterialRequest2 = {
    id: undefined,
    modulusOfElasticity: parseFloat(properties.modulusOfElasticity),
    modulusOfRigidity: parseFloat(properties.modulusOfRigidity),
    pressureUnit: pressureUnit,
  }

  // Call the API to create the material
  const createMaterialPromise = apiClient.createMaterial(
    editorState.remoteModelId,
    createMaterialRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const materialResponse: MaterialResponse = {
    id: uniqueTempId,
    modelId: editorState.remoteModelId,
    modulusOfElasticity: createMaterialRequest.modulusOfElasticity,
    modulusOfRigidity: createMaterialRequest.modulusOfRigidity,
    pressureUnit: createMaterialRequest.pressureUnit,
  }

  // Optimistically update the store
  dispatch(createMaterial({ canvasId, material: materialResponse }))

  try {
    const realMaterialResponse = await createMaterialPromise

    // remove the optimistically created material and replace with real one
    dispatch(removeMaterialById({ canvasId, materialId: uniqueTempId }))
    dispatch(createMaterial({ canvasId, material: realMaterialResponse }))
  } catch (error) {
    console.error("Failed to create material:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
