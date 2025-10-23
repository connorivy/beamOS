import type { PayloadAction } from "@reduxjs/toolkit"
import { createSlice } from "@reduxjs/toolkit"

export type SectionProfileIdOption = {
  label: string
  value: number | null
}

export type SectionProfileProperties = {
  name: string
  area: string
  strongAxisMomentOfInertia: string
  weakAxisMomentOfInertia: string
  polarMomentOfInertia: string
  strongAxisPlasticSectionModulus: string
  weakAxisPlasticSectionModulus: string
  strongAxisShearArea: string
  weakAxisShearArea: string
}

export type SectionProfileSelectionState = {
  sectionProfileId: number | null
  sectionProfileIdInput: string
  properties: SectionProfileProperties
}

const initialState: SectionProfileSelectionState = {
  sectionProfileId: null,
  sectionProfileIdInput: "",
  properties: {
    name: "",
    area: "",
    strongAxisMomentOfInertia: "",
    weakAxisMomentOfInertia: "",
    polarMomentOfInertia: "",
    strongAxisPlasticSectionModulus: "",
    weakAxisPlasticSectionModulus: "",
    strongAxisShearArea: "",
    weakAxisShearArea: "",
  },
}

export const sectionProfileSelectionSlice = createSlice({
  name: "sectionProfileSelection",
  initialState,
  reducers: {
    setSectionProfileId(state, action: PayloadAction<number | null>) {
      state.sectionProfileId = action.payload
    },
    setSectionProfileIdInput(state, action: PayloadAction<string>) {
      state.sectionProfileIdInput = action.payload
    },
    setProperties(state, action: PayloadAction<SectionProfileProperties>) {
      state.properties = action.payload
    },
    setProperty(
      state,
      action: PayloadAction<{ key: keyof SectionProfileProperties; value: string }>,
    ) {
      state.properties[action.payload.key] = action.payload.value
    },
    resetSectionProfileSelection(state) {
      Object.assign(state, initialState)
    },
  },
  selectors: {
    sectionProfileIdSelector: (state: SectionProfileSelectionState) => state.sectionProfileId,
    sectionProfileIdInputSelector: (state: SectionProfileSelectionState) => state.sectionProfileIdInput,
    propertiesSelector: (state: SectionProfileSelectionState) => state.properties,
  },
})

export const {
  setSectionProfileId,
  setSectionProfileIdInput,
  setProperties,
  setProperty,
  resetSectionProfileSelection,
} = sectionProfileSelectionSlice.actions

export const {
  sectionProfileIdSelector,
  sectionProfileIdInputSelector,
  propertiesSelector,
} = sectionProfileSelectionSlice.selectors
