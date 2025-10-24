import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type MomentLoadIdOption = {
  label: string
  value: number | null
}

export type Direction = { x: string; y: string; z: string }

export type MomentLoadSelectionState = {
  momentLoadId: number | null
  momentLoadIdInput: string
  loadCaseId: string
  nodeId: string
  magnitude: string
  direction: Direction
}

const initialState: MomentLoadSelectionState = {
  momentLoadId: null,
  momentLoadIdInput: "",
  loadCaseId: "",
  nodeId: "",
  magnitude: "",
  direction: { x: "", y: "", z: "" },
}

export const momentLoadSelectionSlice = createSlice({
  name: "momentLoadSelection",
  initialState,
  reducers: {
    setMomentLoadId(state, action: PayloadAction<number | null>) {
      state.momentLoadId = action.payload
    },
    setMomentLoadIdInput(state, action: PayloadAction<string>) {
      state.momentLoadIdInput = action.payload
    },
    setLoadCaseId(state, action: PayloadAction<string>) {
      state.loadCaseId = action.payload
    },
    setNodeId(state, action: PayloadAction<string>) {
      state.nodeId = action.payload
    },
    setMagnitude(state, action: PayloadAction<string>) {
      state.magnitude = action.payload
    },
    setDirection(
      state,
      action: PayloadAction<{ key: keyof Direction; value: string }>,
    ) {
      state.direction[action.payload.key] = action.payload.value
    },
    resetMomentLoadSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    momentLoadIdSelector: (state: MomentLoadSelectionState) => state.momentLoadId,
    momentLoadIdInputSelector: (state: MomentLoadSelectionState) => state.momentLoadIdInput,
    loadCaseIdSelector: (state: MomentLoadSelectionState) => state.loadCaseId,
    nodeIdSelector: (state: MomentLoadSelectionState) => state.nodeId,
    magnitudeSelector: (state: MomentLoadSelectionState) => state.magnitude,
    directionSelector: (state: MomentLoadSelectionState) => state.direction,
  },
})

export const {
  setMomentLoadId,
  setMomentLoadIdInput,
  setLoadCaseId,
  setNodeId,
  setMagnitude,
  setDirection,
  resetMomentLoadSelection,
} = momentLoadSelectionSlice.actions

export const {
  momentLoadIdSelector,
  momentLoadIdInputSelector,
  loadCaseIdSelector,
  nodeIdSelector,
  magnitudeSelector,
  directionSelector,
} = momentLoadSelectionSlice.selectors
