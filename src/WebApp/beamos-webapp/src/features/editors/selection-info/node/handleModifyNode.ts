
import type { Dispatch, Action } from "@reduxjs/toolkit"
import type { Coords, Restraints } from "./nodeSelectionSlice"

// Use the same types as handleCreateNode
import type { EditorState} from "../../editorsSlice";
import { modifyNode } from "../../editorsSlice"
import type { BeamOsEditor } from "../../../three-js-editor/BeamOsEditor"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

// The apiClient is the same type as in handleCreateNode
// It should have a putNode method: (modelId, nodeId, updateRequest)
// We'll follow the same pattern as handleCreateNode


export async function handleModifyNode(
    apiClient: IStructuralAnalysisApiClientV1,
    dispatch: Dispatch<Action>,
    nodeIdInput: string,
    coords: Coords,
    editorState: EditorState,
    editor: BeamOsEditor,
    restraints: Restraints,
    canvasId: string
) {
    // Validate input
    if (!nodeIdInput || isNaN(Number(nodeIdInput))) {
        console.error("Node ID must be specified for modify node")
        return
    }
    if (!coords.x || !coords.y || !coords.z) {
        console.error("All coordinates are required")
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

    const nodeId = Number(nodeIdInput)
    const lengthUnit = editorState.model.settings.unitSettings.lengthUnit

    // Prepare the update request (same shape as create, but with id)
    const updateNodeRequest = {
        id: nodeId,
        locationPoint: {
            x: parseFloat(coords.x),
            y: parseFloat(coords.y),
            z: parseFloat(coords.z),
            lengthUnit: lengthUnit,
        },
        restraint: {
            canTranslateAlongX: restraints.CanTranslateAlongX,
            canTranslateAlongY: restraints.CanTranslateAlongY,
            canTranslateAlongZ: restraints.CanTranslateAlongZ,
            canRotateAboutX: restraints.CanRotateAboutX,
            canRotateAboutY: restraints.CanRotateAboutY,
            canRotateAboutZ: restraints.CanRotateAboutZ,
        },
    }

    // Optimistically update the store
    dispatch(modifyNode({
        canvasId,
        nodeId,
        node: updateNodeRequest
    }))


    // Optimistically update the UI (editor)
    const nodeResponse = {
        ...updateNodeRequest,
        modelId: editorState.remoteModelId,
    }
    await editor.api.updateNode(nodeResponse)

    // Call the API to update the node
    try {
        await apiClient.putNode(
            editorState.remoteModelId,
            nodeId,
            updateNodeRequest
        )
    } catch (error) {
        console.error("Failed to modify node:", error)
        // Optionally, implement rollback logic here
    }
}
