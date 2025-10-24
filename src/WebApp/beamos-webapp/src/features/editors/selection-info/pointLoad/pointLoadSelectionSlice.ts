import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type PointLoadIdOption = {
  label: string
  value: number | null
}

export type PointLoadSelectionState = {
  pointLoadId: number | null
  pointLoadIdInput: string
  loadCaseId: string
  nodeId: string
  magnitude: string
  directionX: string
  directionY: string
  directionZ: string
}

const initialState: PointLoadSelectionState = {
  pointLoadId: null,
  pointLoadIdInput: "",
  loadCaseId: "",
  nodeId: "",
  magnitude: "",
  directionX: "",
  directionY: "",
  directionZ: "",
}

export const pointLoadSelectionSlice = createSlice({
  name: "pointLoadSelection",
  initialState,
  reducers: {
    setPointLoadId(state, action: PayloadAction<number | null>) {
      state.pointLoadId = action.payload
    },
    setPointLoadIdInput(state, action: PayloadAction<string>) {
      state.pointLoadIdInput = action.payload
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
    setDirectionX(state, action: PayloadAction<string>) {
      state.directionX = action.payload
    },
    setDirectionY(state, action: PayloadAction<string>) {
      state.directionY = action.payload
    },
    setDirectionZ(state, action: PayloadAction<string>) {
      state.directionZ = action.payload
    },
    resetPointLoadSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    pointLoadIdSelector: (state: PointLoadSelectionState) => state.pointLoadId,
    pointLoadIdInputSelector: (state: PointLoadSelectionState) =>
      state.pointLoadIdInput,
    loadCaseIdSelector: (state: PointLoadSelectionState) => state.loadCaseId,
    nodeIdSelector: (state: PointLoadSelectionState) => state.nodeId,
    magnitudeSelector: (state: PointLoadSelectionState) => state.magnitude,
    directionXSelector: (state: PointLoadSelectionState) => state.directionX,
    directionYSelector: (state: PointLoadSelectionState) => state.directionY,
    directionZSelector: (state: PointLoadSelectionState) => state.directionZ,
  },
})

export const {
  setPointLoadId,
  setPointLoadIdInput,
  setLoadCaseId,
  setNodeId,
  setMagnitude,
  setDirectionX,
  setDirectionY,
  setDirectionZ,
  resetPointLoadSelection,
} = pointLoadSelectionSlice.actions

export const {
  pointLoadIdSelector,
  pointLoadIdInputSelector,
  loadCaseIdSelector,
  nodeIdSelector,
  magnitudeSelector,
  directionXSelector,
  directionYSelector,
  directionZSelector,
} = pointLoadSelectionSlice.selectors
