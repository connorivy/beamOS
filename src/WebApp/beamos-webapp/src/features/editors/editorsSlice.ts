import type { PayloadAction } from "@reduxjs/toolkit"
import { createAppSlice } from "../../app/createAppSlice"
import type {
  MoveNodeCommand,
  SelectedObject,
} from "../three-js-editor/EditorApi/EditorEventsApi"
// Dependencies injected via Redux store's extraArgument
export type AppDependencies = {
  apiClient: {
    patchNode: (
      remoteModelId: string,
      request: UpdateNodeRequest,
    ) => Promise<unknown>
  }
  // Add undoManager or other dependencies here as needed
}
import type { UpdateNodeRequest } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

// Define the shape of the editor state for a single editor
export type EditorState = {
  canvasId: string
  remoteModelId?: string
  isReadOnly: boolean
  selection: SelectedObject[] | null
}

// The state is a map of id -> EditorState
export type EditorsState = Record<string, EditorState>

const initialState: EditorsState = {}

export const editorsSlice = createAppSlice({
  name: "editors",
  initialState,
  reducers: create => ({
    addEditor: create.reducer((state, action: PayloadAction<EditorState>) => {
      state[action.payload.canvasId] = action.payload
    }),
    updateEditor: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          changes: Partial<EditorState>
        }>,
      ) => {
        state[action.payload.canvasId] = {
          ...state[action.payload.canvasId],
          ...action.payload.changes,
        }
      },
    ),
    removeEditor: create.reducer((state, action: PayloadAction<string>) => {
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      const { [action.payload]: _removed, ...rest } = state
      return rest
    }),
    objectSelectionChanged: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          selection: SelectedObject[] | null
        }>,
      ) => {
        const editor = state[action.payload.canvasId]
        editor.selection = action.payload.selection
      },
    ),
    moveNode: create.reducer(
      (
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        _state,
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        _action: PayloadAction<{
          command: MoveNodeCommand
        }>,
      ) => {
        // This reducer can be empty if the actual node movement is handled elsewhere
      },
      // moveNode: create.asyncThunk(
      //   async (command: MoveNodeCommand, thunkAPI) => {
      //     // Use injected dependencies from extra
      //     const { apiClient } = thunkAPI.extra as AppDependencies
      //     const state = thunkAPI.getState() as { editors: EditorsState }
      //     const editorState = state.editors[command.canvasId]
      //     if (!editorState || !editorState.remoteModelId) {
      //       return
      //     }
      //     // Ensure required properties for UpdateNodeRequest
      //     // Convert Coordinate3D to NullableOfPartialPoint
      //     const locationPoint = new NullableOfPartialPoint({
      //       x: command.newLocation.x,
      //       y: command.newLocation.y,
      //       z: command.newLocation.z,
      //       lengthUnit: LengthUnit._0, // Set to desired unit, e.g. meters
      //     })
      //     await apiClient.patchNode(
      //       editorState.remoteModelId,
      //       new UpdateNodeRequest({
      //         id: command.nodeId,
      //         locationPoint,
      //       }),
      //     )
      //     // Here you would typically make an API call to move the node
      //     // For this example, we'll just log the command
      //     await Promise.resolve() // Simulate async operation
      //     console.log("Node moved:", command)
      //   },
      //   {
      //     rejected: (_state, action) => {
      //       console.error("Failed to move node:", action.error)
      //     },
      //   },
    ),
  }),
})

export const {
  addEditor,
  updateEditor,
  removeEditor,
  objectSelectionChanged,
  moveNode,
} = editorsSlice.actions

// Selector to get editor state by id
export const selectEditorById = (
  state: { editors: EditorsState },
  id: string,
) => state.editors[id]
