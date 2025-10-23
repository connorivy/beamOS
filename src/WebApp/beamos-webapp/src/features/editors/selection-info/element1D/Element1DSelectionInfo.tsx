import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import {
    elementIdSelector,
    elementIdInputSelector,
    startNodeIdSelector,
    endNodeIdSelector,
    sectionProfileIdSelector,
    materialIdSelector,
    orientationSelector,
    setElementId,
    setElementIdInput,
    setStartNodeId,
    setEndNodeId,
    setSectionProfileId,
    setMaterialId,
    setOrientation,
    resetElement1DSelection
} from "./element1DSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
    Collapse,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import type { ActionCreatorWithPayload } from "@reduxjs/toolkit"
// import { handleCreateElement1D } from "./handleCreateElement1D" // To be implemented

import type { Dispatch, Action } from "@reduxjs/toolkit"
// TODO: Replace with actual API call logic
const handleCreateElement1D = (
    dispatch: Dispatch<Action>
) => {
    // Optimistically update Redux/Three.js here
    // Then call API and update with response
    // Placeholder logic
    dispatch(resetElement1DSelection())
}

export const Element1DSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    // Handler for node selection changes
    const handleNodeChange = (actionCreator: ActionCreatorWithPayload<number | null>) =>
        (_event: React.SyntheticEvent, newValue: { label: string; value: number } | null) => {
            dispatch(actionCreator(newValue?.value ?? null))
        }

    // Handler for section selection changes
    const handleSectionChange = (_event: React.SyntheticEvent, newValue: { label: string; value: number } | null) => {
        dispatch(setSectionProfileId(newValue?.value ?? null))
    }

    // Handler for material selection changes
    const handleMaterialChange = (_event: React.SyntheticEvent, newValue: { label: string; value: number } | null) => {
        dispatch(setMaterialId(newValue?.value ?? null))
    }

    // Handler for orientation input changes
    const handleOrientationChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value
        // Only allow empty string or a valid number
        if (value === "" || /^-?\d*\.?\d*$/.test(value)) {
            dispatch(setOrientation(value === "" ? null : Number(value)))
        }
    }
    const elementId = useAppSelector(elementIdSelector)
    const elementIdInput = useAppSelector(elementIdInputSelector)
    const startNodeId = useAppSelector(startNodeIdSelector)
    const endNodeId = useAppSelector(endNodeIdSelector)
    const sectionProfileId = useAppSelector(sectionProfileIdSelector)
    const materialId = useAppSelector(materialIdSelector)
    const orientation = useAppSelector(orientationSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    // Removed unused variables: apiClient, editors, editorState
    const elementIds = [
        { label: "New Element", value: null },
        ...Object.keys(modelResponse?.element1ds ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]
    const nodeOptions = Object.keys(modelResponse?.nodes ?? {}).map(id => ({ label: id, value: Number(id) }))
    const sectionOptions = Object.keys(modelResponse?.sectionProfiles ?? {}).map(id => ({ label: id, value: Number(id) }))
    const materialOptions = Object.keys(modelResponse?.materials ?? {}).map(id => ({ label: id, value: Number(id) }))

    const resetInput = useCallback(() => {
        dispatch(setElementIdInput(""))
        dispatch(setStartNodeId(null))
        dispatch(setEndNodeId(null))
        dispatch(setSectionProfileId(null))
        dispatch(setMaterialId(null))
        dispatch(setOrientation(null))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Element"
        if (elementId === null) {
            resetInput()
        } else {
            const element = modelResponse?.element1ds[elementId]
            if (element) {
                dispatch(setElementIdInput(elementId.toString()))
                dispatch(setStartNodeId(element.startNodeId))
                dispatch(setEndNodeId(element.endNodeId))
                dispatch(setSectionProfileId(element.sectionProfileId))
                dispatch(setMaterialId(element.materialId))
                dispatch(setOrientation(element.sectionProfileRotation?.value ?? null))
            }
        }
    }, [elementId, dispatch, modelResponse?.element1ds, resetInput])

    // Only allow whole numbers for elementId input
    const handleElementIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || /^\d+$/.test(value)) {
            dispatch(setElementIdInput(value))
        }
    }, [dispatch])

    const handleCreateElement1DFunc = useCallback(() => {
        handleCreateElement1D(dispatch)
    }, [dispatch])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Element 1D Id
            </Typography>
            <Autocomplete
                options={elementIds}
                getOptionLabel={(option: string | { label: string; value: number | null }) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    elementIds.find(
                        n => typeof n !== "string" && n.value === elementId
                    ) ?? elementIds[0]
                }
                inputValue={elementIdInput}
                onInputChange={handleElementIdInputChange}
                onChange={(
                    _event,
                    newValue: string | { label: string; value: number | null } | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setElementId(null))
                        dispatch(setElementIdInput(newValue))
                    } else {
                        dispatch(setElementId(newValue?.value ?? null))
                        dispatch(setElementIdInput(newValue?.label ?? ""))
                    }
                }}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Element 1D Id"
                        variant="outlined"
                        size="small"
                    />
                )}
                freeSolo
                sx={{ mb: 2 }}
            />

            <Collapse in>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    Connectivity
                </Typography>
                <Autocomplete
                    options={nodeOptions}
                    getOptionLabel={option => option.label}
                    value={nodeOptions.find(n => n.value === startNodeId) ?? null}
                    onChange={handleNodeChange(setStartNodeId)}
                    renderInput={params => (
                        <TextField {...params} label="Start Node*" variant="outlined" size="small" />
                    )}
                    sx={{ mb: 1 }}
                />
                <Autocomplete
                    options={nodeOptions}
                    getOptionLabel={option => option.label}
                    value={nodeOptions.find(n => n.value === endNodeId) ?? null}
                    onChange={handleNodeChange(setEndNodeId)}
                    renderInput={params => (
                        <TextField {...params} label="End Node*" variant="outlined" size="small" />
                    )}
                    sx={{ mb: 2 }}
                />
            </Collapse>

            <Collapse in>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    Section
                </Typography>
                <Autocomplete
                    options={sectionOptions}
                    getOptionLabel={option => option.label}
                    value={sectionOptions.find(s => s.value === sectionProfileId) ?? null}
                    onChange={handleSectionChange}
                    renderInput={params => (
                        <TextField {...params} label="Section*" variant="outlined" size="small" />
                    )}
                    sx={{ mb: 2 }}
                />
            </Collapse>

            <Collapse in>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    Material
                </Typography>
                <Autocomplete
                    options={materialOptions}
                    getOptionLabel={option => option.label}
                    value={materialOptions.find(m => m.value === materialId) ?? null}
                    onChange={handleMaterialChange}
                    renderInput={params => (
                        <TextField {...params} label="Material*" variant="outlined" size="small" />
                    )}
                    sx={{ mb: 2 }}
                />
            </Collapse>

            <Collapse in>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    Orientation
                </Typography>
                <TextField
                    label="Orientation"
                    value={orientation ?? ""}
                    onChange={handleOrientationChange}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 2 }}
                />
            </Collapse>

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={handleCreateElement1DFunc}>
                CREATE
            </Button>
        </MuiBox>
    )
}
