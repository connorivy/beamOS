import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import type { SectionProfileProperties } from "./sectionProfileSelectionSlice"
import {
    setSectionProfileId,
    setSectionProfileIdInput,
    setProperty,
    sectionProfileIdSelector,
    sectionProfileIdInputSelector,
    propertiesSelector,
} from "./sectionProfileSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreateSectionProfile } from "./handleCreateSectionProfile"

type SectionProfileIdOption = {
    label: string;
    value: number | null;
}

function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

function formatNumber(value: number | undefined): string {
    if (value === undefined || value === null) {
        return ""
    }
    // Ensure at least one decimal place
    if (value === Math.floor(value)) {
        return value.toFixed(1)
    }
    return value.toString()
}

export const SectionProfileSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const sectionProfileId = useAppSelector(sectionProfileIdSelector)
    const sectionProfileIdInput = useAppSelector(sectionProfileIdInputSelector)
    const properties = useAppSelector(propertiesSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const sectionProfileIds: SectionProfileIdOption[] = [
        { label: "New Section Profile", value: null },
        ...Object.keys(modelResponse?.sectionProfiles ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

    const resetInput = useCallback(() => {
        dispatch(setSectionProfileIdInput(""))
        dispatch(setProperty({ key: "name", value: "" }))
        dispatch(setProperty({ key: "area", value: "" }))
        dispatch(setProperty({ key: "strongAxisMomentOfInertia", value: "" }))
        dispatch(setProperty({ key: "weakAxisMomentOfInertia", value: "" }))
        dispatch(setProperty({ key: "polarMomentOfInertia", value: "" }))
        dispatch(setProperty({ key: "strongAxisPlasticSectionModulus", value: "" }))
        dispatch(setProperty({ key: "weakAxisPlasticSectionModulus", value: "" }))
        dispatch(setProperty({ key: "strongAxisShearArea", value: "" }))
        dispatch(setProperty({ key: "weakAxisShearArea", value: "" }))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Section Profile"
        if (sectionProfileId === null) {
            resetInput()
        }
        else {
            const sectionProfile = modelResponse?.sectionProfiles[sectionProfileId]
            if (sectionProfile) {
                dispatch(setSectionProfileIdInput(sectionProfileId.toString()))
                dispatch(setProperty({ key: "name", value: sectionProfile.name }))
                dispatch(setProperty({ key: "area", value: formatNumber(sectionProfile.area) }))
                dispatch(setProperty({ key: "strongAxisMomentOfInertia", value: formatNumber(sectionProfile.strongAxisMomentOfInertia) }))
                dispatch(setProperty({ key: "weakAxisMomentOfInertia", value: formatNumber(sectionProfile.weakAxisMomentOfInertia) }))
                dispatch(setProperty({ key: "polarMomentOfInertia", value: formatNumber(sectionProfile.polarMomentOfInertia) }))
                dispatch(setProperty({ key: "strongAxisPlasticSectionModulus", value: formatNumber(sectionProfile.strongAxisPlasticSectionModulus) }))
                dispatch(setProperty({ key: "weakAxisPlasticSectionModulus", value: formatNumber(sectionProfile.weakAxisPlasticSectionModulus) }))
                dispatch(setProperty({ key: "strongAxisShearArea", value: formatNumber(sectionProfile.strongAxisShearArea) }))
                dispatch(setProperty({ key: "weakAxisShearArea", value: formatNumber(sectionProfile.weakAxisShearArea) }))
            }
        }
    }, [sectionProfileId, dispatch, modelResponse?.sectionProfiles, resetInput])

    // Only allow whole numbers for sectionProfileId input
    const handleSectionProfileIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setSectionProfileIdInput(value))
        }
    }, [dispatch])

    // Only allow doubles for numeric properties
    const handlePropertyChange = useCallback((key: keyof SectionProfileProperties) => (event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (key === "name") {
            // Allow any string for name
            dispatch(setProperty({ key, value: val }))
        } else if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            // Allow doubles for numeric fields
            dispatch(setProperty({ key, value: val }))
        }
    }, [dispatch])

    const handleCreateSectionProfileFunc = useCallback(async () => {
        await handleCreateSectionProfile(
            apiClient,
            dispatch,
            sectionProfileIdInput,
            properties,
            editorState,
            canvasId
        );
    }, [apiClient, canvasId, dispatch, editorState, properties, sectionProfileIdInput])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Id
            </Typography>
            <Autocomplete
                options={sectionProfileIds}
                getOptionLabel={(option: string | SectionProfileIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    sectionProfileIds.find(
                        n => typeof n !== "string" && n.value === sectionProfileId
                    ) ?? sectionProfileIds[0]
                }
                inputValue={sectionProfileIdInput}
                onInputChange={handleSectionProfileIdInputChange}
                onChange={(
                    _event,
                    newValue: string | SectionProfileIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setSectionProfileId(null))
                        dispatch(setSectionProfileIdInput(newValue))
                    } else {
                        dispatch(setSectionProfileId(newValue?.value ?? null))
                        dispatch(setSectionProfileIdInput(newValue?.label ?? ""))
                    }
                }}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Id"
                        variant="outlined"
                        size="small"
                    />
                )}
                freeSolo
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Properties
            </Typography>
            <TextField
                label="Name*"
                value={properties.name}
                onChange={handlePropertyChange("name")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "name" } }}
            />
            <TextField
                label="Area*"
                value={properties.area}
                onChange={handlePropertyChange("area")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "cross-section-area" } }}
            />
            <TextField
                label="Strong Axis Moment of Inertia*"
                value={properties.strongAxisMomentOfInertia}
                onChange={handlePropertyChange("strongAxisMomentOfInertia")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "strong axis moment of inertia" } }}
            />
            <TextField
                label="Weak Axis Moment of Inertia*"
                value={properties.weakAxisMomentOfInertia}
                onChange={handlePropertyChange("weakAxisMomentOfInertia")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "weak axis moment of inertia" } }}
            />
            <TextField
                label="Polar Moment of Inertia*"
                value={properties.polarMomentOfInertia}
                onChange={handlePropertyChange("polarMomentOfInertia")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "polar moment of inertia" } }}
            />
            <TextField
                label="Strong Axis Plastic Section Modulus*"
                value={properties.strongAxisPlasticSectionModulus}
                onChange={handlePropertyChange("strongAxisPlasticSectionModulus")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "strong axis plastic section modulus" } }}
            />
            <TextField
                label="Weak Axis Plastic Section Modulus*"
                value={properties.weakAxisPlasticSectionModulus}
                onChange={handlePropertyChange("weakAxisPlasticSectionModulus")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "weak axis plastic section modulus" } }}
            />
            <TextField
                label="Weak Axis Shear Area*"
                value={properties.weakAxisShearArea}
                onChange={handlePropertyChange("weakAxisShearArea")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 1 }}
                slotProps={{ htmlInput: { "aria-label": "weak axis shear region" } }}
            />
            <TextField
                label="Strong Axis Shear Area*"
                value={properties.strongAxisShearArea}
                onChange={handlePropertyChange("strongAxisShearArea")}
                variant="outlined"
                size="small"
                fullWidth
                sx={{ mb: 2 }}
                slotProps={{ htmlInput: { "aria-label": "strong axis shear region" } }}
            />

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateSectionProfileFunc(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}
