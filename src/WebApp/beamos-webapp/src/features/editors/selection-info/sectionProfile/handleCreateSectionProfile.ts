import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  CreateSectionProfileRequest2,
  IStructuralAnalysisApiClientV1,
  SectionProfileResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type { EditorState } from "../../editorsSlice"
import {
  createSectionProfile,
  removeSectionProfileById,
} from "../../editorsSlice"
import type { SectionProfileProperties } from "./sectionProfileSelectionSlice"

export async function handleCreateSectionProfile(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  sectionProfileIdInput: string,
  properties: SectionProfileProperties,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (sectionProfileIdInput || sectionProfileIdInput !== "") {
    console.error(
      "Section Profile ID cannot be specified for new section profile",
    )
    return
  }
  if (!properties.name) {
    console.error("Name is required")
    return
  }
  if (
    !properties.area ||
    !properties.strongAxisMomentOfInertia ||
    !properties.weakAxisMomentOfInertia ||
    !properties.polarMomentOfInertia ||
    !properties.strongAxisPlasticSectionModulus ||
    !properties.weakAxisPlasticSectionModulus ||
    !properties.strongAxisShearArea ||
    !properties.weakAxisShearArea
  ) {
    console.error("All section profile properties are required")
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

  const lengthUnit = editorState.model.settings.unitSettings.lengthUnit

  // Create the section profile request
  const createSectionProfileRequest: CreateSectionProfileRequest2 = {
    id: undefined,
    name: properties.name,
    area: parseFloat(properties.area),
    strongAxisMomentOfInertia: parseFloat(properties.strongAxisMomentOfInertia),
    weakAxisMomentOfInertia: parseFloat(properties.weakAxisMomentOfInertia),
    polarMomentOfInertia: parseFloat(properties.polarMomentOfInertia),
    strongAxisPlasticSectionModulus: parseFloat(
      properties.strongAxisPlasticSectionModulus,
    ),
    weakAxisPlasticSectionModulus: parseFloat(
      properties.weakAxisPlasticSectionModulus,
    ),
    strongAxisShearArea: parseFloat(properties.strongAxisShearArea),
    weakAxisShearArea: parseFloat(properties.weakAxisShearArea),
    lengthUnit: lengthUnit,
  }

  // Call the API to create the section profile
  const createSectionProfilePromise = apiClient.createSectionProfile(
    editorState.remoteModelId,
    createSectionProfileRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const sectionProfileResponse: SectionProfileResponse = {
    id: uniqueTempId,
    modelId: editorState.remoteModelId,
    name: createSectionProfileRequest.name,
    area: createSectionProfileRequest.area,
    strongAxisMomentOfInertia:
      createSectionProfileRequest.strongAxisMomentOfInertia,
    weakAxisMomentOfInertia:
      createSectionProfileRequest.weakAxisMomentOfInertia,
    polarMomentOfInertia: createSectionProfileRequest.polarMomentOfInertia,
    strongAxisPlasticSectionModulus:
      createSectionProfileRequest.strongAxisPlasticSectionModulus,
    weakAxisPlasticSectionModulus:
      createSectionProfileRequest.weakAxisPlasticSectionModulus,
    strongAxisShearArea: createSectionProfileRequest.strongAxisShearArea,
    weakAxisShearArea: createSectionProfileRequest.weakAxisShearArea,
    lengthUnit: createSectionProfileRequest.lengthUnit,
  }

  // Optimistically update the store
  dispatch(
    createSectionProfile({ canvasId, sectionProfile: sectionProfileResponse }),
  )

  try {
    const realSectionProfileResponse = await createSectionProfilePromise

    // remove the optimistically created section profile and replace with real one
    dispatch(
      removeSectionProfileById({ canvasId, sectionProfileId: uniqueTempId }),
    )
    dispatch(
      createSectionProfile({
        canvasId,
        sectionProfile: realSectionProfileResponse,
      }),
    )
  } catch (error) {
    console.error("Failed to create section profile:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
