import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type LoadCaseIdOption = {
  label: string
  value: number | null
}

export type LoadCaseSelectionState = {
  loadCaseId: number | null
  loadCaseIdInput: string
  name: string
}

const initialState: LoadCaseSelectionState = {
  loadCaseId: null,
  loadCaseIdInput: "",
  name: "",
}

export const loadCaseSelectionSlice = createSlice({
  name: "loadCaseSelection",
  initialState,
  reducers: {
    setLoadCaseId(state, action: PayloadAction<number | null>) {
      state.loadCaseId = action.payload
    },
    setLoadCaseIdInput(state, action: PayloadAction<string>) {
      state.loadCaseIdInput = action.payload
    },
    setName(state, action: PayloadAction<string>) {
      state.name = action.payload
    },
    resetLoadCaseSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    loadCaseIdSelector: (state: LoadCaseSelectionState) => state.loadCaseId,
    loadCaseIdInputSelector: (state: LoadCaseSelectionState) => state.loadCaseIdInput,
    nameSelector: (state: LoadCaseSelectionState) => state.name,
  },
})

export const {
  setLoadCaseId,
  setLoadCaseIdInput,
  setName,
  resetLoadCaseSelection,
} = loadCaseSelectionSlice.actions

export const {
  loadCaseIdSelector,
  loadCaseIdInputSelector,
  nameSelector,
} = loadCaseSelectionSlice.selectors
