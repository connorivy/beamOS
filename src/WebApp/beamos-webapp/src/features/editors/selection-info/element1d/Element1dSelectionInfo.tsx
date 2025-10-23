import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import {
    setElement1dId,
    setElement1dIdInput,
    setStartNodeId,
    setEndNodeId,
    setMaterialId,
    setSectionProfileId,
    setSectionProfileRotation,
    element1dIdSelector,
    element1dIdInputSelector,
    startNodeIdSelector,
    endNodeIdSelector,
    materialIdSelector,
    sectionProfileIdSelector,
    sectionProfileRotationSelector,
} from "./element1dSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreateElement1d } from "./handleCreateElement1d"

type Element1dIdOption = {
    label: string;
    value: number | null;
}
function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

export const Element1dSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const element1dId = useAppSelector(element1dIdSelector)
    const element1dIdInput = useAppSelector(element1dIdInputSelector)
    const startNodeId = useAppSelector(startNodeIdSelector)
    const endNodeId = useAppSelector(endNodeIdSelector)
    const materialId = useAppSelector(materialIdSelector)
    const sectionProfileId = useAppSelector(sectionProfileIdSelector)
    const sectionProfileRotation = useAppSelector(sectionProfileRotationSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const element1dIds: Element1dIdOption[] = [
        { label: "New Element1d", value: null },
        ...Object.keys(modelResponse?.element1ds ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

    const resetInput = useCallback(() => {
        dispatch(setElement1dIdInput(""))
        dispatch(setStartNodeId(""))
        dispatch(setEndNodeId(""))
        dispatch(setMaterialId(""))
        dispatch(setSectionProfileId(""))
        dispatch(setSectionProfileRotation(""))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Element1d"
        if (element1dId === null) {
            resetInput()
        }
        else {
            const element1d = modelResponse?.element1ds[element1dId]
            if (element1d) {
                dispatch(setElement1dIdInput(element1dId.toString()))
                dispatch(setStartNodeId(element1d.startNodeId.toString()))
                dispatch(setEndNodeId(element1d.endNodeId.toString()))
                dispatch(setMaterialId(element1d.materialId.toString()))
                dispatch(setSectionProfileId(element1d.sectionProfileId.toString()))
                dispatch(setSectionProfileRotation(element1d.sectionProfileRotation?.value?.toString() ?? ""))
            }
        }
    }, [element1dId, dispatch, modelResponse?.element1ds, resetInput])

    // Only allow whole numbers for element1dId input
    const handleElement1dIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setElement1dIdInput(value))
        }
    }, [dispatch])

    const handleStartNodeIdChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || isWholeNumber(val)) {
            dispatch(setStartNodeId(val))
        }
    }, [dispatch])

    const handleEndNodeIdChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || isWholeNumber(val)) {
            dispatch(setEndNodeId(val))
        }
    }, [dispatch])

    const handleMaterialIdChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || isWholeNumber(val)) {
            dispatch(setMaterialId(val))
        }
    }, [dispatch])

    const handleSectionProfileIdChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || isWholeNumber(val)) {
            dispatch(setSectionProfileId(val))
        }
    }, [dispatch])

    const handleSectionProfileRotationChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            dispatch(setSectionProfileRotation(val))
        }
    }, [dispatch])

    const handleCreateElement1dFunc = useCallback(async () => {
        await handleCreateElement1d(
            apiClient,
            dispatch,
            element1dIdInput,
            startNodeId,
            endNodeId,
            materialId,
            sectionProfileId,
            sectionProfileRotation,
            editorState,
            canvasId
        );
    }, [apiClient, canvasId, dispatch, editorState, element1dIdInput, startNodeId, endNodeId, materialId, sectionProfileId, sectionProfileRotation])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Id
            </Typography>
            <Autocomplete
                options={element1dIds}
                getOptionLabel={(option: string | Element1dIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    element1dIds.find(
                        e => typeof e !== "string" && e.value === element1dId
                    ) ?? element1dIds[0]
                }
                inputValue={element1dIdInput}
                onInputChange={handleElement1dIdInputChange}
                onChange={(
                    _event,
                    newValue: string | Element1dIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setElement1dId(null))
                        dispatch(setElement1dIdInput(newValue))
                    } else {
                        dispatch(setElement1dId(newValue?.value ?? null))
                        dispatch(setElement1dIdInput(newValue?.label ?? ""))
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
                Start Node Id
            </Typography>
            <TextField
                label="Start Node Id*"
                value={startNodeId}
                onChange={handleStartNodeIdChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                End Node Id
            </Typography>
            <TextField
                label="End Node Id*"
                value={endNodeId}
                onChange={handleEndNodeIdChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Material Id
            </Typography>
            <TextField
                label="Material Id*"
                value={materialId}
                onChange={handleMaterialIdChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Section Profile Id
            </Typography>
            <TextField
                label="Section Profile Id*"
                value={sectionProfileId}
                onChange={handleSectionProfileIdChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Section Profile Rotation
            </Typography>
            <TextField
                label="Section Profile Rotation"
                value={sectionProfileRotation}
                onChange={handleSectionProfileRotationChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateElement1dFunc(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}
