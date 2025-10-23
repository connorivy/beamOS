import { type IStructuralAnalysisApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { createListenerMiddleware } from "@reduxjs/toolkit"
import { moveNode } from "../editors/editorsSlice"
import { LengthUnit } from "../../utils/type-extensions/UnitTypeContracts"

export function moveNodeListenerMiddleware(
  apiClient: IStructuralAnalysisApiClientV1,
) {
  const listenerMiddleware = createListenerMiddleware()

  listenerMiddleware.startListening({
    actionCreator: moveNode,
    effect: async action => {
      if (!action.payload.command.modelId) {
        // No remote model ID; nothing to patch
        return
      }

      const request = {
        id: action.payload.command.nodeId,
        locationPoint: {
          x: action.payload.command.newLocation.x,
          y: action.payload.command.newLocation.y,
          z: action.payload.command.newLocation.z,
          lengthUnit: LengthUnit.Meter,
        },
        restraint: undefined,
      }

      try {
        await apiClient.patchNode(action.payload.command.modelId, request)
      } catch (error) {
        console.error("Failed to patch node:", error)
        throw error
      }
    },
  })

  return listenerMiddleware
}
