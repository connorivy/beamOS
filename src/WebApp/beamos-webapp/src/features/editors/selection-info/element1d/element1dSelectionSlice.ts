import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type Element1dIdOption = {
  label: string
  value: number | null
}

export type Element1dSelectionState = {
  element1dId: number | null
  element1dIdInput: string
  startNodeId: string
  endNodeId: string
  materialId: string
  sectionProfileId: string
  sectionProfileRotation: string
}

const initialState: Element1dSelectionState = {
  element1dId: null,
  element1dIdInput: "",
  startNodeId: "",
  endNodeId: "",
  materialId: "",
  sectionProfileId: "",
  sectionProfileRotation: "",
}

export const element1dSelectionSlice = createSlice({
  name: "element1dSelection",
  initialState,
  reducers: {
    setElement1dId(state, action: PayloadAction<number | null>) {
      state.element1dId = action.payload
    },
    setElement1dIdInput(state, action: PayloadAction<string>) {
      state.element1dIdInput = action.payload
    },
    setStartNodeId(state, action: PayloadAction<string>) {
      state.startNodeId = action.payload
    },
    setEndNodeId(state, action: PayloadAction<string>) {
      state.endNodeId = action.payload
    },
    setMaterialId(state, action: PayloadAction<string>) {
      state.materialId = action.payload
    },
    setSectionProfileId(state, action: PayloadAction<string>) {
      state.sectionProfileId = action.payload
    },
    setSectionProfileRotation(state, action: PayloadAction<string>) {
      state.sectionProfileRotation = action.payload
    },
    resetElement1dSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    element1dIdSelector: (state: Element1dSelectionState) => {
      console.log("element1dIdSelector called, returning:", state.element1dId)
      return state.element1dId
    },
    element1dIdInputSelector: (state: Element1dSelectionState) =>
      state.element1dIdInput,
    startNodeIdSelector: (state: Element1dSelectionState) => state.startNodeId,
    endNodeIdSelector: (state: Element1dSelectionState) => state.endNodeId,
    materialIdSelector: (state: Element1dSelectionState) => state.materialId,
    sectionProfileIdSelector: (state: Element1dSelectionState) =>
      state.sectionProfileId,
    sectionProfileRotationSelector: (state: Element1dSelectionState) =>
      state.sectionProfileRotation,
  },
})

export const {
  setElement1dId,
  setElement1dIdInput,
  setStartNodeId,
  setEndNodeId,
  setMaterialId,
  setSectionProfileId,
  setSectionProfileRotation,
  resetElement1dSelection,
} = element1dSelectionSlice.actions

export const {
  element1dIdSelector,
  element1dIdInputSelector,
  startNodeIdSelector,
  endNodeIdSelector,
  materialIdSelector,
  sectionProfileIdSelector,
  sectionProfileRotationSelector,
} = element1dSelectionSlice.selectors
