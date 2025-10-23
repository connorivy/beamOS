import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type MaterialIdOption = {
  label: string
  value: number | null
}

export type MaterialProperties = {
  modulusOfElasticity: string
  modulusOfRigidity: string
}

export type MaterialSelectionState = {
  materialId: number | null
  materialIdInput: string
  properties: MaterialProperties
}

const initialState: MaterialSelectionState = {
  materialId: null,
  materialIdInput: "",
  properties: {
    modulusOfElasticity: "",
    modulusOfRigidity: "",
  },
}

export const materialSelectionSlice = createSlice({
  name: "materialSelection",
  initialState,
  reducers: {
    setMaterialId(state, action: PayloadAction<number | null>) {
      state.materialId = action.payload
    },
    setMaterialIdInput(state, action: PayloadAction<string>) {
      state.materialIdInput = action.payload
    },
    setMaterialProperties(state, action: PayloadAction<MaterialProperties>) {
      state.properties = action.payload
    },
    setMaterialProperty(
      state,
      action: PayloadAction<{ key: keyof MaterialProperties; value: string }>,
    ) {
      state.properties[action.payload.key] = action.payload.value
    },
    resetMaterialSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    materialIdSelector: (state: MaterialSelectionState) => state.materialId,
    materialIdInputSelector: (state: MaterialSelectionState) =>
      state.materialIdInput,
    materialPropertiesSelector: (state: MaterialSelectionState) =>
      state.properties,
  },
})

export const {
  setMaterialId,
  setMaterialIdInput,
  setMaterialProperties,
  setMaterialProperty,
  resetMaterialSelection,
} = materialSelectionSlice.actions

export const {
  materialIdSelector,
  materialIdInputSelector,
  materialPropertiesSelector,
} = materialSelectionSlice.selectors
