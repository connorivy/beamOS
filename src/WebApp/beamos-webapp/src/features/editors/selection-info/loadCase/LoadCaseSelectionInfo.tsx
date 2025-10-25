import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import {
    setLoadCaseId,
    setLoadCaseIdInput,
    setName,
    loadCaseIdSelector,
    loadCaseIdInputSelector,
    nameSelector,
} from "./loadCaseSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreateLoadCase } from "./handleCreateLoadCase"
import { handleModifyLoadCase } from "./handleModifyLoadCase"

type LoadCaseIdOption = {
    label: string;
    value: number | null;
}
function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

export const LoadCaseSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const loadCaseId = useAppSelector(loadCaseIdSelector)
    const loadCaseIdInput = useAppSelector(loadCaseIdInputSelector)
    const name = useAppSelector(nameSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const loadCaseIds: LoadCaseIdOption[] = [
        { label: "New Load Case", value: null },
        ...Object.keys(modelResponse?.loadCases ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

    const handleModifyLoadCaseFunc = useCallback(async () => {
        if (typeof loadCaseId === "number" && !isNaN(loadCaseId)) {
            await handleModifyLoadCase(
                apiClient,
                dispatch,
                loadCaseId,
                name,
                editorState,
                canvasId
            );
        }
    }, [apiClient, canvasId, dispatch, editorState, loadCaseId, name])

    const resetInput = useCallback(() => {
        dispatch(setLoadCaseIdInput(""))
        dispatch(setName(""))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Load Case"
        if (loadCaseId === null) {
            resetInput()
        }
        else {
            const loadCase = modelResponse?.loadCases[loadCaseId]
            if (loadCase) {
                dispatch(setLoadCaseIdInput(loadCaseId.toString()))
                dispatch(setName(loadCase.name))
            }
        }
    }, [loadCaseId, dispatch, modelResponse?.loadCases, resetInput])

    // Only allow whole numbers for loadCaseId input
    const handleLoadCaseIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setLoadCaseIdInput(value))
        }
    }, [dispatch])

    const handleNameChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        dispatch(setName(event.target.value))
    }, [dispatch])

    const handleCreateLoadCaseFunc = useCallback(async () => {
        await handleCreateLoadCase(
            apiClient,
            dispatch,
            loadCaseIdInput,
            name,
            editorState,
            canvasId
        );
    }, [apiClient, canvasId, dispatch, editorState, loadCaseIdInput, name])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Id
            </Typography>
            <Autocomplete
                options={loadCaseIds}
                getOptionLabel={(option: string | LoadCaseIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    loadCaseIds.find(
                        lc => typeof lc !== "string" && lc.value === loadCaseId
                    ) ?? loadCaseIds[0]
                }
                inputValue={loadCaseIdInput}
                onInputChange={handleLoadCaseIdInputChange}
                onChange={(
                    _event,
                    newValue: string | LoadCaseIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setLoadCaseId(null));
                        dispatch(setLoadCaseIdInput(newValue));
                    } else {
                        dispatch(setLoadCaseId(newValue?.value ?? null));
                        dispatch(setLoadCaseIdInput(newValue?.label ?? ""));
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
                Name
            </Typography>
            <TextField
                label="Name*"
                value={name}
                onChange={handleNameChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            {loadCaseId === null ? (
                <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateLoadCaseFunc(); }}>
                    CREATE
                </Button>
            ) : (
                <Button variant="contained" color="primary" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleModifyLoadCaseFunc(); }}>
                    APPLY
                </Button>
            )}
        </MuiBox>
    )
}
