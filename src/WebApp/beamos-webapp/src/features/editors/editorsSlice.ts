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
  SectionProfileResponse,
  UpdateNodeRequest,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { ToModelState, type ModelState } from "./ModelState"

// Define the shape of the editor state for a single editor
export type EditorState = {
  canvasId: string
  remoteModelId?: string
  isReadOnly: boolean
  selection: SelectedObject[] | null
  model: ModelState | null
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
          throw new Error(
            `Editor for canvasId ${action.payload.canvasId} does not exist. Ensure addEditor is dispatched before modelLoaded.`,
          )
        }
        editor.model = ToModelState(action.payload.model)
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
        editor.model.nodes[action.payload.node.id] = {
          locationPoint: action.payload.node.locationPoint,
          restraint: action.payload.node.restraint,
        }
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
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { [action.payload.nodeId]: _, ...restNodes } = editor.model.nodes
        editor.model.nodes = restNodes
      },
    ),
    createSectionProfile: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          sectionProfile: SectionProfileResponse
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
        editor.model.sectionProfiles[action.payload.sectionProfile.id] = {
          name: action.payload.sectionProfile.name,
          area: action.payload.sectionProfile.area,
          strongAxisMomentOfInertia: action.payload.sectionProfile.strongAxisMomentOfInertia,
          weakAxisMomentOfInertia: action.payload.sectionProfile.weakAxisMomentOfInertia,
          polarMomentOfInertia: action.payload.sectionProfile.polarMomentOfInertia,
          strongAxisPlasticSectionModulus: action.payload.sectionProfile.strongAxisPlasticSectionModulus,
          weakAxisPlasticSectionModulus: action.payload.sectionProfile.weakAxisPlasticSectionModulus,
          strongAxisShearArea: action.payload.sectionProfile.strongAxisShearArea,
          weakAxisShearArea: action.payload.sectionProfile.weakAxisShearArea,
          lengthUnit: action.payload.sectionProfile.lengthUnit,
        }
      },
    ),
    removeSectionProfileById: create.reducer(
      (
        state,
        action: PayloadAction<{
          canvasId: string
          sectionProfileId: number
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
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { [action.payload.sectionProfileId]: _, ...restSectionProfiles } = editor.model.sectionProfiles
        editor.model.sectionProfiles = restSectionProfiles
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
      return editor.model?.nodes[nodeId] ?? null
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
  createSectionProfile,
  removeSectionProfileById,
  moveNode,
  modelLoaded,
} = editorsSlice.actions

export const {
  selectEditorByCanvasId,
  selectModelResponseByCanvasId,
  selectNodeById,
} = editorsSlice.selectors
