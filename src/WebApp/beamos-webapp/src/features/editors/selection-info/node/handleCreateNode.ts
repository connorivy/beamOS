import type { Action, Dispatch } from "@reduxjs/toolkit"
import type {
  CreateNodeRequest2,
  IStructuralAnalysisApiClientV1,
  NodeResponse,
} from "../../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type { BeamOsEditor } from "../../../three-js-editor/BeamOsEditor"
import {
  createNode,
  removeNodeById,
  type EditorState,
} from "../../editorsSlice"
import type { Coords, Restraints } from "./nodeSelectionSlice"

export async function handleCreateNode(
  apiClient: IStructuralAnalysisApiClientV1,
  dispatch: Dispatch<Action>,
  nodeIdInput: string,
  coords: Coords,
  editorState: EditorState,
  editor: BeamOsEditor,
  restraints: Restraints,
  canvasId: string,
) {
  // Validate input
  if (nodeIdInput || nodeIdInput !== "") {
    console.error("Node ID cannot be specified for new node")
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

  const lengthUnit = editorState.model.settings.unitSettings.lengthUnit

  // Create the node request
  const createNodeRequest: CreateNodeRequest2 = {
    id: undefined,
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

  // Call the API to create the node
  const createNodePromise = apiClient.createNode(
    editorState.remoteModelId,
    createNodeRequest,
  )

  // get a unique temporary id as a number from the current time
  const uniqueTempId = -1 * Date.now()
  const nodeResponse: NodeResponse = {
    id: uniqueTempId,
    modelId: editorState.remoteModelId,
    locationPoint: createNodeRequest.locationPoint,
    restraint: createNodeRequest.restraint,
  }

  // Optimistically update the ui
  await editor.api.createNode(nodeResponse)

  // Optimistically update the store
  dispatch(createNode({ canvasId, node: nodeResponse }))

  try {
    const realNodeResponse = await createNodePromise

    // remove the optimistically created node and replace with real one
    console.log(
      `Real node response received: ${JSON.stringify(realNodeResponse)}`,
    )
    await editor.api.createNode(realNodeResponse)
    await editor.api.deleteNode({
      id: uniqueTempId,
      modelId: editorState.remoteModelId,
    })
    dispatch(removeNodeById({ canvasId, nodeId: uniqueTempId }))
    dispatch(createNode({ canvasId, node: realNodeResponse }))
  } catch (error) {
    console.error("Failed to create node:", error)
    // Optionally, you might want to implement rollback logic here
  }
}
