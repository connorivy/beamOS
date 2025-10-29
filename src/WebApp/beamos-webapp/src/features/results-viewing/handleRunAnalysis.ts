import type { Action, Dispatch } from "@reduxjs/toolkit"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type { BeamOsEditor } from "../three-js-editor/BeamOsEditor"
import {
  addDeflectionDiagrams,
  addMomentDiagrams,
  addNodeResults,
  addResultsSet,
  addShearForceDiagrams,
  clearResults,
  setSelectedResultSetId,
} from "../editors/editorsSlice"

export async function handleRunAnalysis(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  modelId: string,
  canvasId: string,
  editor: BeamOsEditor,
) {
  dispatch(
    clearResults({
      canvasId,
    }),
  )
  await editor.api.clearCurrentOverlay()

  const results = await apiClient.runOpenSeesAnalysis(modelId, {
    unitsOverride: "kn-m",
  })
  dispatch(
    addResultsSet({
      canvasId,
      resultSetId: results.id,
    }),
  )
  dispatch(
    setSelectedResultSetId({ canvasId, selectedResultSetId: results.id }),
  )
  const nodesResults = await apiClient.getNodeResults(modelId, results.id)
  dispatch(
    addNodeResults({
      canvasId,
      resultSetId: results.id,
      nodeResults: Object.values(nodesResults).map(nodeResult => ({
        nodeId: nodeResult.nodeId,
        forces: nodeResult.forces,
        displacements: nodeResult.displacements,
      })),
    }),
  )

  if (results.deflectionDiagrams) {
    dispatch(
      addDeflectionDiagrams({
        canvasId,
        resultSetId: results.id,
        deflectionResults: results.deflectionDiagrams,
      }),
    )
  }
  if (results.shearDiagrams) {
    dispatch(
      addShearForceDiagrams({
        canvasId,
        resultSetId: results.id,
        shearForceResults: results.shearDiagrams ?? [],
      }),
    )
  }
  if (results.momentDiagrams) {
    dispatch(
      addMomentDiagrams({
        canvasId,
        resultSetId: results.id,
        momentResults: results.momentDiagrams ?? [],
      }),
    )
  }
}
