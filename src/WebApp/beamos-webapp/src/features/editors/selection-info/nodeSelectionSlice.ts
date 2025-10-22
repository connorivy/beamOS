import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type NodeIdOption = {
  label: string
  value: number | null
}

export type Coords = { x: string; y: string; z: string }
export type Restraints = {
  CanTranslateAlongX: boolean
  CanTranslateAlongY: boolean
  CanTranslateAlongZ: boolean
  CanRotateAboutX: boolean
  CanRotateAboutY: boolean
  CanRotateAboutZ: boolean
}

export type NodeSelectionState = {
  nodeId: number | null
  nodeIdInput: string
  coords: Coords
  restraints: Restraints
}

const initialState: NodeSelectionState = {
  nodeId: null,
  nodeIdInput: "",
  coords: { x: "", y: "", z: "" },
  restraints: {
    CanTranslateAlongX: false,
    CanTranslateAlongY: false,
    CanTranslateAlongZ: false,
    CanRotateAboutX: false,
    CanRotateAboutY: false,
    CanRotateAboutZ: false,
  },
}

export const nodeSelectionSlice = createSlice({
  name: "nodeSelection",
  initialState,
  reducers: {
    setNodeId(state, action: PayloadAction<number | null>) {
      state.nodeId = action.payload
      if (action.payload === null) {
        state.nodeIdInput = ""
        state.coords = { x: "", y: "", z: "" }
        state.restraints = {
          CanTranslateAlongX: false,
          CanTranslateAlongY: false,
          CanTranslateAlongZ: false,
          CanRotateAboutX: false,
          CanRotateAboutY: false,
          CanRotateAboutZ: false,
        }
      }
    },
    setNodeIdInput(state, action: PayloadAction<string>) {
      state.nodeIdInput = action.payload
    },
    setCoords(state, action: PayloadAction<Coords>) {
      state.coords = action.payload
    },
    setCoord(
      state,
      action: PayloadAction<{ key: keyof Coords; value: string }>,
    ) {
      state.coords[action.payload.key] = action.payload.value
    },
    setRestraints(state, action: PayloadAction<Restraints>) {
      state.restraints = action.payload
    },
    setRestraint(
      state,
      action: PayloadAction<{ key: keyof Restraints; value: boolean }>,
    ) {
      state.restraints[action.payload.key] = action.payload.value
    },
    resetNodeSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    nodeIdSelector: (state: NodeSelectionState) => state.nodeId,
    nodeIdInputSelector: (state: NodeSelectionState) => state.nodeIdInput,
    coordsSelector: (state: NodeSelectionState) => state.coords,
    restraintsSelector: (state: NodeSelectionState) => state.restraints,
  },
})

export const {
  setNodeId,
  setNodeIdInput,
  setCoords,
  setCoord,
  setRestraints,
  setRestraint,
  resetNodeSelection,
} = nodeSelectionSlice.actions

export const {
  nodeIdSelector,
  nodeIdInputSelector,
  coordsSelector,
  restraintsSelector,
} = nodeSelectionSlice.selectors
