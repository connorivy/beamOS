import type { PayloadAction } from "@reduxjs/toolkit"
import { createAppSlice } from "../../app/createAppSlice"
import {
  type IUnitSettings,
  type IAnalysisSettings,
  type ModelInfoResponse,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

// Types
// export type Model = {
//   id: string;
//   name: string;
//   description: string;
//   role: string;
//   lastModified: string;
// };
export type ModelInfoResponseSerializable = {
  id: string
  name: string
  description: string
  settings: {
    unitSettings: IUnitSettings
    analysisSettings: IAnalysisSettings
    yAxisUp: boolean
  }
  lastModified: string
  role: string
}

export type ModelsPageState = {
  isLoading: boolean
  isAuthenticated: boolean
  models: ModelInfoResponseSerializable[]
  sampleModels: ModelInfoResponseSerializable[]
  searchTerm: string
  error?: string
  showCreateModelDialog: boolean
}

const initialState: ModelsPageState = {
  isLoading: false,
  isAuthenticated: false,
  models: [],
  sampleModels: [
    {
      id: "4ce66084-4ac1-40bc-99ae-3d0f334c66fa",
      name: "Twisty Bowl Framing",
      description:
        "A crazy twisting bowl type structure. Made by Bjorn Steinhagen in grasshopper and then sent to beamOS using Speckle",
      settings: {
        unitSettings: {
          lengthUnit: 1,
          forceUnit: 2,
          angleUnit: 1,
        },
        analysisSettings: {
          element1DAnalysisType: 1,
        },
        yAxisUp: true,
      },
      lastModified: "2023-11-01T12:00:00Z",
      role: "Sample",
    },
  ],
  searchTerm: "",
  error: undefined,
  showCreateModelDialog: false,
}

export const modelsPageSlice = createAppSlice({
  name: "modelsPage",
  initialState,
  reducers: create => ({
    setSearchTerm: create.reducer((state, action: PayloadAction<string>) => {
      state.searchTerm = action.payload
    }),
    showCreateModelDialog: create.reducer(state => {
      state.showCreateModelDialog = true
    }),
    hideCreateModelDialog: create.reducer(state => {
      state.showCreateModelDialog = false
    }),
    userModelsLoaded: create.reducer(
      (state, action: PayloadAction<ModelInfoResponse[]>) => {
        state.models = action.payload.map(m => ({
          id: m.id,
          name: m.name,
          description: m.description,
          settings: m.settings,
          lastModified: m.lastModified.toDateString(),
          role: m.role,
        }))
        state.isLoading = false
      },
    ),

    // login: create.reducer(state => {
    //   // TODO: Implement login logic
    //   state.isAuthenticated = true;
    // }),
    // viewModel: create.reducer((_state, _action: PayloadAction<string>) => {
    //   // TODO: Implement navigation logic
    // }),
    // fetchModelsPageData,
  }),
})

export const {
  setSearchTerm,
  showCreateModelDialog,
  hideCreateModelDialog,
  userModelsLoaded,
} = modelsPageSlice.actions

// Selectors
export const selectModelsPage = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage
export const selectModels = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage.models
export const selectSampleModels = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage.sampleModels
export const selectIsLoading = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage.isLoading
export const selectIsAuthenticated = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage.isAuthenticated
export const selectSearchTerm = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage.searchTerm
export const selectError = (state: { modelsPage: ModelsPageState }) =>
  state.modelsPage.error
export const selectShowCreateModelDialog = (state: {
  modelsPage: ModelsPageState
}) => state.modelsPage.showCreateModelDialog

export default modelsPageSlice.reducer
