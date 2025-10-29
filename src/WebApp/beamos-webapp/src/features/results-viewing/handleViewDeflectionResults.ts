import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  DeflectionDiagramResponse,
  IStructuralAnalysisApiClientV1,
  MomentDiagramResponse,
  ShearDiagramResponse,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type { BeamOsEditor } from "../three-js-editor/BeamOsEditor"
import type { EditorState } from "../editors/editorsSlice"
import {
  addDeflectionDiagrams,
  addMomentDiagrams,
  addShearForceDiagrams,
} from "../editors/editorsSlice"
import type { ModelState } from "../editors/ModelState"

export async function handleGetDiagramResults(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  resultSetId: number,
  modelId: string,
  canvasId: string,
) {
  const diagramResults = await apiClient.getDiagrams(
    modelId,
    resultSetId,
    "kn-m",
  )
  if (!diagramResults.deflectionDiagrams) {
    console.error("Deflection results are not available")
    return
  }
  dispatch(
    addDeflectionDiagrams({
      canvasId,
      resultSetId,
      deflectionResults: diagramResults.deflectionDiagrams,
    }),
  )
  dispatch(
    addShearForceDiagrams({
      canvasId,
      resultSetId,
      shearForceResults: diagramResults.shearDiagrams ?? [],
    }),
  )
  dispatch(
    addMomentDiagrams({
      canvasId,
      resultSetId,
      momentResults: diagramResults.momentDiagrams ?? [],
    }),
  )

  return diagramResults
}

export async function handleViewDeflectionResults(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  resultSetId: number,
  editor: BeamOsEditor,
  editorState: EditorState,
  modelState: ModelState | null,
  canvasId: string,
) {
  const modelId = editorState.remoteModelId
  if (!modelId) {
    console.error("Remote model ID is not available")
    return
  }
  let deflectionResults: DeflectionDiagramResponse[]
  if (
    modelState?.resultSets?.[resultSetId]?.deflectionDiagrams &&
    modelState.resultSets[resultSetId].deflectionDiagrams.length > 0
  ) {
    deflectionResults = modelState.resultSets[resultSetId].deflectionDiagrams
  } else {
    const diagramResults = await handleGetDiagramResults(
      apiClient,
      dispatch,
      resultSetId,
      modelId,
      canvasId,
    )
    deflectionResults = diagramResults?.deflectionDiagrams ?? []
  }

  await editor.api.createDeflectionDiagrams(deflectionResults)
}

export async function handleViewShearResults(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  resultSetId: number,
  editor: BeamOsEditor,
  editorState: EditorState,
  modelState: ModelState | null,
  canvasId: string,
) {
  const modelId = editorState.remoteModelId
  if (!modelId) {
    console.error("Remote model ID is not available")
    return
  }
  let shearResults: ShearDiagramResponse[]
  if (
    modelState?.resultSets?.[resultSetId]?.shearDiagrams &&
    modelState.resultSets[resultSetId].shearDiagrams.length > 0
  ) {
    shearResults = modelState.resultSets[resultSetId].shearDiagrams
  } else {
    const diagramResults = await handleGetDiagramResults(
      apiClient,
      dispatch,
      resultSetId,
      modelId,
      canvasId,
    )
    shearResults = diagramResults?.shearDiagrams ?? []
  }

  await editor.api.createShearDiagrams(shearResults)
}

export async function handleViewMomentResults(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  resultSetId: number,
  editor: BeamOsEditor,
  editorState: EditorState,
  modelState: ModelState | null,
  canvasId: string,
) {
  const modelId = editorState.remoteModelId
  if (!modelId) {
    console.error("Remote model ID is not available")
    return
  }
  let momentResults: MomentDiagramResponse[]
  if (
    modelState?.resultSets?.[resultSetId]?.momentDiagrams &&
    modelState.resultSets[resultSetId].momentDiagrams.length > 0
  ) {
    momentResults = modelState.resultSets[resultSetId].momentDiagrams
  } else {
    const diagramResults = await handleGetDiagramResults(
      apiClient,
      dispatch,
      resultSetId,
      modelId,
      canvasId,
    )
    momentResults = diagramResults?.momentDiagrams ?? []
  }

  await editor.api.createMomentDiagrams(momentResults)
}
