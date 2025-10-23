import { createSlice } from "@reduxjs/toolkit"
import type { PayloadAction } from "@reduxjs/toolkit"

export type Element1DSelectionState = {
  elementId: number | null
  elementIdInput: string
  startNodeId: number | null
  endNodeId: number | null
  sectionProfileId: number | null
  materialId: number | null
  orientation: number | null
}

const initialState: Element1DSelectionState = {
  elementId: null,
  elementIdInput: "",
  startNodeId: null,
  endNodeId: null,
  sectionProfileId: null,
  materialId: null,
  orientation: null,
}

export const element1DSelectionSlice = createSlice({
  name: "element1DSelection",
  initialState,
  reducers: {
    setElementId(state, action: PayloadAction<number | null>) {
      state.elementId = action.payload
    },
    setElementIdInput(state, action: PayloadAction<string>) {
      state.elementIdInput = action.payload
    },
    setStartNodeId(state, action: PayloadAction<number | null>) {
      state.startNodeId = action.payload
    },
    setEndNodeId(state, action: PayloadAction<number | null>) {
      state.endNodeId = action.payload
    },
    setSectionProfileId(state, action: PayloadAction<number | null>) {
      state.sectionProfileId = action.payload
    },
    setMaterialId(state, action: PayloadAction<number | null>) {
      state.materialId = action.payload
    },
    setOrientation(state, action: PayloadAction<number | null>) {
      state.orientation = action.payload
    },
    resetElement1DSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    elementIdSelector: (state: Element1DSelectionState) => state.elementId,
    elementIdInputSelector: (state: Element1DSelectionState) =>
      state.elementIdInput,
    startNodeIdSelector: (state: Element1DSelectionState) => state.startNodeId,
    endNodeIdSelector: (state: Element1DSelectionState) => state.endNodeId,
    sectionProfileIdSelector: (state: Element1DSelectionState) =>
      state.sectionProfileId,
    materialIdSelector: (state: Element1DSelectionState) => state.materialId,
    orientationSelector: (state: Element1DSelectionState) => state.orientation,
  },
})

export const {
  setElementId,
  setElementIdInput,
  setStartNodeId,
  setEndNodeId,
  setSectionProfileId,
  setMaterialId,
  setOrientation,
  resetElement1DSelection,
} = element1DSelectionSlice.actions

export const {
  elementIdSelector,
  elementIdInputSelector,
  startNodeIdSelector,
  endNodeIdSelector,
  sectionProfileIdSelector,
  materialIdSelector,
  orientationSelector,
} = element1DSelectionSlice.selectors
