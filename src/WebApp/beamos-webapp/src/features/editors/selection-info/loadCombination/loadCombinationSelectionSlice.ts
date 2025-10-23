import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type LoadCombinationIdOption = {
  label: string
  value: number | null
}

export type LoadCaseFactorPair = {
  loadCaseId: string
  factor: string
}

export type LoadCombinationSelectionState = {
  loadCombinationId: number | null
  loadCombinationIdInput: string
  loadCaseFactorPairs: LoadCaseFactorPair[]
}

const initialState: LoadCombinationSelectionState = {
  loadCombinationId: null,
  loadCombinationIdInput: "",
  loadCaseFactorPairs: [{ loadCaseId: "", factor: "" }],
}

export const loadCombinationSelectionSlice = createSlice({
  name: "loadCombinationSelection",
  initialState,
  reducers: {
    setLoadCombinationId(state, action: PayloadAction<number | null>) {
      state.loadCombinationId = action.payload
    },
    setLoadCombinationIdInput(state, action: PayloadAction<string>) {
      state.loadCombinationIdInput = action.payload
    },
    setLoadCaseFactorPairs(state, action: PayloadAction<LoadCaseFactorPair[]>) {
      state.loadCaseFactorPairs = action.payload
    },
    setLoadCaseId(state, action: PayloadAction<{ index: number; value: string }>) {
      if (state.loadCaseFactorPairs[action.payload.index]) {
        state.loadCaseFactorPairs[action.payload.index].loadCaseId = action.payload.value
      }
    },
    setFactor(state, action: PayloadAction<{ index: number; value: string }>) {
      if (state.loadCaseFactorPairs[action.payload.index]) {
        state.loadCaseFactorPairs[action.payload.index].factor = action.payload.value
      }
    },
    addLoadCaseFactorPair(state) {
      state.loadCaseFactorPairs.push({ loadCaseId: "", factor: "" })
    },
    removeLoadCaseFactorPair(state, action: PayloadAction<number>) {
      if (state.loadCaseFactorPairs.length > 1) {
        state.loadCaseFactorPairs.splice(action.payload, 1)
      }
    },
    resetLoadCombinationSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    loadCombinationIdSelector: (state: LoadCombinationSelectionState) => state.loadCombinationId,
    loadCombinationIdInputSelector: (state: LoadCombinationSelectionState) => state.loadCombinationIdInput,
    loadCaseFactorPairsSelector: (state: LoadCombinationSelectionState) => state.loadCaseFactorPairs,
  },
})

export const {
  setLoadCombinationId,
  setLoadCombinationIdInput,
  setLoadCaseFactorPairs,
  setLoadCaseId,
  setFactor,
  addLoadCaseFactorPair,
  removeLoadCaseFactorPair,
  resetLoadCombinationSelection,
} = loadCombinationSelectionSlice.actions

export const {
  loadCombinationIdSelector,
  loadCombinationIdInputSelector,
  loadCaseFactorPairsSelector,
} = loadCombinationSelectionSlice.selectors
