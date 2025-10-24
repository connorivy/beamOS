import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import {
    setLoadCombinationId,
    setLoadCombinationIdInput,
    setLoadCaseFactorPairs,
    setLoadCaseId,
    setFactor,
    addLoadCaseFactorPair,
    loadCombinationIdSelector,
    loadCombinationIdInputSelector,
    loadCaseFactorPairsSelector,
} from "./loadCombinationSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreateLoadCombination } from "./handleCreateLoadCombination"

type LoadCombinationIdOption = {
    label: string;
    value: number | null;
}

type LoadCaseIdOption = {
    label: string;
    value: number;
}

function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

export const LoadCombinationSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const loadCombinationId = useAppSelector(loadCombinationIdSelector)
    const loadCombinationIdInput = useAppSelector(loadCombinationIdInputSelector)
    const loadCaseFactorPairs = useAppSelector(loadCaseFactorPairsSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editorState = useAppSelector(state => state.editors[canvasId])

    const loadCombinationIds: LoadCombinationIdOption[] = [
        { label: "New Load Combination", value: null },
        ...Object.keys(modelResponse?.loadCombinations ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

    const loadCaseIds: LoadCaseIdOption[] = Object.keys(modelResponse?.loadCases ?? {}).map(id => ({ label: id, value: Number(id) }))

    const resetInput = useCallback(() => {
        dispatch(setLoadCombinationIdInput(""))
        // Reset to a single empty pair
        dispatch(setLoadCaseFactorPairs([{ loadCaseId: "", factor: "" }]))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Load Combination"
        if (loadCombinationId === null) {
            resetInput()
        }
        else {
            const loadCombination = modelResponse?.loadCombinations[loadCombinationId]
            if (loadCombination) {
                dispatch(setLoadCombinationIdInput(loadCombinationId.toString()))
                // Convert loadCaseFactors object to array of pairs
                const pairs = Object.entries(loadCombination.loadCaseFactors).map(([loadCaseId, factor]) => ({
                    loadCaseId: loadCaseId,
                    factor: factor.toString(),
                }))
                // Update all pairs at once
                dispatch(setLoadCaseFactorPairs(pairs))
            }
        }
    }, [loadCombinationId, dispatch, modelResponse?.loadCombinations, resetInput])



    // Only allow whole numbers for loadCombinationId input
    const handleLoadCombinationIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setLoadCombinationIdInput(value))
        }
    }, [dispatch])

    const handleLoadCaseIdChange = useCallback((index: number) => (_event: React.SyntheticEvent, value: string | LoadCaseIdOption | null) => {
        const newValue = typeof value === "string" ? value : (value?.value.toString() ?? "")
        dispatch(setLoadCaseId({ index, value: newValue }))

        // If this is the last pair and both fields will have values, add a new pair
        setTimeout(() => {
            if (index === loadCaseFactorPairs.length - 1 && newValue && loadCaseFactorPairs[index].factor) {
                dispatch(addLoadCaseFactorPair())
            }
        }, 0)
    }, [dispatch, loadCaseFactorPairs])

    const handleFactorChange = useCallback((index: number) => (event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            dispatch(setFactor({ index, value: val }))

            // If this is the last pair and both fields will have values, add a new pair
            setTimeout(() => {
                if (index === loadCaseFactorPairs.length - 1 && val && loadCaseFactorPairs[index].loadCaseId) {
                    dispatch(addLoadCaseFactorPair())
                }
            }, 0)
        }
    }, [dispatch, loadCaseFactorPairs])

    const handleCreateLoadCombinationFunc = useCallback(async () => {
        // Filter out empty pairs and convert to loadCaseFactors object
        const loadCaseFactors: Record<string, number> = {}
        for (const pair of loadCaseFactorPairs) {
            if (pair.loadCaseId && pair.factor) {
                loadCaseFactors[pair.loadCaseId] = parseFloat(pair.factor)
            }
        }

        await handleCreateLoadCombination(
            apiClient,
            dispatch,
            loadCombinationIdInput,
            loadCaseFactors,
            editorState,
            canvasId
        );
    }, [apiClient, canvasId, dispatch, editorState, loadCombinationIdInput, loadCaseFactorPairs])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Id
            </Typography>
            <Autocomplete
                options={loadCombinationIds}
                getOptionLabel={(option: string | LoadCombinationIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    loadCombinationIds.find(
                        lc => typeof lc !== "string" && lc.value === loadCombinationId
                    ) ?? loadCombinationIds[0]
                }
                inputValue={loadCombinationIdInput}
                onInputChange={handleLoadCombinationIdInputChange}
                onChange={(
                    _event,
                    newValue: string | LoadCombinationIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setLoadCombinationId(null))
                        dispatch(setLoadCombinationIdInput(newValue))
                    } else {
                        dispatch(setLoadCombinationId(newValue?.value ?? null))
                        dispatch(setLoadCombinationIdInput(newValue?.label ?? ""))
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
                Load Case Factors
            </Typography>

            {loadCaseFactorPairs.map((pair, index) => (
                <MuiBox key={index} sx={{ mb: 2, display: 'flex', flexDirection: 'row', gap: 2, alignItems: 'flex-end' }}>
                    <Autocomplete
                        options={loadCaseIds}
                        getOptionLabel={(option: string | LoadCaseIdOption) =>
                            typeof option === "string" ? option : option.label
                        }
                        value={
                            loadCaseIds.find(
                                lc => typeof lc !== "string" && lc.value.toString() === pair.loadCaseId
                            ) ?? null
                        }
                        inputValue={pair.loadCaseId}
                        onInputChange={handleLoadCaseIdChange(index)}
                        onChange={(
                            _event,
                            newValue: string | LoadCaseIdOption | null
                        ) => {
                            const value = typeof newValue === "string" ? newValue : (newValue?.value.toString() ?? "")
                            dispatch(setLoadCaseId({ index, value }))
                        }}
                        renderInput={params => (
                            <TextField
                                {...params}
                                label="Load Case"
                                variant="outlined"
                                size="small"
                            />
                        )}
                        freeSolo
                        sx={{ flex: 1 }}
                    />
                    <TextField
                        label="Factor*"
                        value={pair.factor}
                        onChange={e => {
                            // Only allow up to 3 decimal places
                            const val = e.target.value
                            if (val === '' || /^-?\d*(\.\d{0,3})?$/.test(val)) {
                                handleFactorChange(index)(e as React.ChangeEvent<HTMLInputElement>)
                            }
                        }}
                        variant="outlined"
                        size="small"
                        sx={{ width: 90 }}
                        slotProps={{ htmlInput: { step: 0.001, min: -999, max: 999 } }}
                    />
                </MuiBox>
            ))}

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateLoadCombinationFunc(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}
