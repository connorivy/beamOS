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
import type {
  ModelResponse,
  NodeResponse,
  UpdateNodeRequest,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { stat } from "fs"

// Define the shape of the editor state for a single editor
export type EditorState = {
  canvasId: string
  remoteModelId?: string
  isReadOnly: boolean
  selection: SelectedObject[] | null
  model: ModelResponse | null
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
    ),
    modelLoaded: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          model: ModelResponse
          remoteModelId?: string
        }>,
      ) => {
        const editor =
          action.payload.canvasId in state
            ? state[action.payload.canvasId]
            : null
        if (!editor) {
          state[action.payload.canvasId] = {
            canvasId: action.payload.canvasId,
            isReadOnly: false,
            selection: null,
            model: action.payload.model,
            remoteModelId: action.payload.remoteModelId,
          }
          return
          // throw new Error(
          //   `Editor for canvasId ${action.payload.canvasId} does not exist. Ensure addEditor is dispatched before modelLoaded.`,
          // )
        }
        editor.model = action.payload.model
        if (action.payload.remoteModelId) {
          editor.remoteModelId = action.payload.remoteModelId
        }
      },
    ),
    createNode: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          node: NodeResponse
        }>,
      ) => {
        const editor =
          action.payload.canvasId in state
            ? state[action.payload.canvasId]
            : null
        if (!editor?.model) {
          throw new Error(
            `Model response for canvasId ${action.payload.canvasId} is null`,
          )
        }
        editor.model.nodes ??= []
        editor.model.nodes.push(action.payload.node)
      },
    ),
    removeNodeById: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          nodeId: number
        }>,
      ) => {
        const editor =
          action.payload.canvasId in state
            ? state[action.payload.canvasId]
            : null
        if (!editor?.model) {
          throw new Error(
            `Model response for canvasId ${action.payload.canvasId} is null`,
          )
        }
        editor.model.nodes = editor.model.nodes?.filter(
          n => n.id !== action.payload.nodeId,
        )
      },
    ),
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
  }),
  selectors: {
    selectEditorByCanvasId: (state: EditorsState, canvasId: string) =>
      canvasId in state ? state[canvasId] : null,
    selectModelResponseByCanvasId: (state: EditorsState, canvasId: string) =>
      (canvasId in state ? state[canvasId] : null)?.model,
    selectNodeById: (state: EditorsState, canvasId: string, nodeId: number) => {
      const editor = state[canvasId]
      return editor.model?.nodes?.find(n => n.id === nodeId) ?? null
    },
  },
})

export const {
  addEditor,
  updateEditor,
  removeEditor,
  objectSelectionChanged,
  createNode,
  removeNodeById,
  moveNode,
  modelLoaded,
} = editorsSlice.actions

export const {
  selectEditorByCanvasId,
  selectModelResponseByCanvasId,
  selectNodeById,
} = editorsSlice.selectors
