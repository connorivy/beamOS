import type { Action, Dispatch } from "@reduxjs/toolkit"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type { EditorState } from "../../editorsSlice"
import { modifySectionProfile } from "../../editorsSlice"
import type { SectionProfileProperties } from "./sectionProfileSelectionSlice"

export async function handleModifySectionProfile(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  sectionProfileIdInput: string,
  properties: SectionProfileProperties,
  editorState: EditorState,
  canvasId: string,
) {
  // Validate input
  if (!sectionProfileIdInput || isNaN(Number(sectionProfileIdInput))) {
    console.error(
      "Section Profile ID must be specified for modify section profile",
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

  const sectionProfileId = Number(sectionProfileIdInput)
  const lengthUnit = editorState.model.settings.unitSettings.lengthUnit

  // Prepare the update request
  const updateSectionProfileRequest = {
    id: sectionProfileId,
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

  // Optimistically update the store
  dispatch(
    modifySectionProfile({
      canvasId,
      sectionProfileId,
      sectionProfile: updateSectionProfileRequest,
    }),
  )

  // Call the API to update the section profile
  try {
    await apiClient.putSectionProfile(
      editorState.remoteModelId,
      sectionProfileId,
      updateSectionProfileRequest,
    )
  } catch (error) {
    console.error("Failed to modify section profile:", error)
    // Optionally, implement rollback logic here
  }
}
