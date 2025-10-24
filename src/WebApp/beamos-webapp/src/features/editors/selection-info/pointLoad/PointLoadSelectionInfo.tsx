import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import {
    setPointLoadId,
    setPointLoadIdInput,
    setLoadCaseId,
    setNodeId,
    setMagnitude,
    setDirectionX,
    setDirectionY,
    setDirectionZ,
    pointLoadIdSelector,
    pointLoadIdInputSelector,
    loadCaseIdSelector,
    nodeIdSelector,
    magnitudeSelector,
    directionXSelector,
    directionYSelector,
    directionZSelector,
} from "./pointLoadSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreatePointLoad } from "./handleCreatePointLoad"

type PointLoadIdOption = {
    label: string;
    value: number | null;
}
function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

export const PointLoadSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const pointLoadId = useAppSelector(pointLoadIdSelector)
    const pointLoadIdInput = useAppSelector(pointLoadIdInputSelector)
    const loadCaseId = useAppSelector(loadCaseIdSelector)
    const nodeId = useAppSelector(nodeIdSelector)
    const magnitude = useAppSelector(magnitudeSelector)
    const directionX = useAppSelector(directionXSelector)
    const directionY = useAppSelector(directionYSelector)
    const directionZ = useAppSelector(directionZSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const pointLoadIds: PointLoadIdOption[] = [
        { label: "New Point Load", value: null },
        ...Object.keys(modelResponse?.pointLoads ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

    const resetInput = useCallback(() => {
        dispatch(setPointLoadIdInput(""))
        dispatch(setLoadCaseId(""))
        dispatch(setNodeId(""))
        dispatch(setMagnitude(""))
        dispatch(setDirectionX(""))
        dispatch(setDirectionY(""))
        dispatch(setDirectionZ(""))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Point Load"
        if (pointLoadId === null) {
            resetInput()
        }
        else {
            const pointLoad = modelResponse?.pointLoads[pointLoadId]
            if (pointLoad) {
                dispatch(setPointLoadIdInput(pointLoadId.toString()))
                dispatch(setLoadCaseId(pointLoad.loadCaseId.toString()))
                dispatch(setNodeId(pointLoad.nodeId.toString()))
                dispatch(setMagnitude(pointLoad.force.value.toString()))
                dispatch(setDirectionX(pointLoad.direction.x.toString()))
                dispatch(setDirectionY(pointLoad.direction.y.toString()))
                dispatch(setDirectionZ(pointLoad.direction.z.toString()))
            }
        }
    }, [pointLoadId, dispatch, modelResponse?.pointLoads, resetInput])

    // Only allow whole numbers for pointLoadId input
    const handlePointLoadIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setPointLoadIdInput(value))
        }
    }, [dispatch])

    // Only allow whole numbers for loadCaseId
    const handleLoadCaseIdChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setLoadCaseId(value))
        }
    }, [dispatch])

    // Only allow whole numbers for nodeId
    const handleNodeIdChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setNodeId(value))
        }
    }, [dispatch])

    // Only allow doubles for magnitude
    const handleMagnitudeChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            dispatch(setMagnitude(val))
        }
    }, [dispatch])

    // Only allow doubles for direction components
    const handleDirectionXChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || /^-?\d*(\.\d*)?$/.test(value)) {
            dispatch(setDirectionX(value))
        }
    }, [dispatch])

    const handleDirectionYChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || /^-?\d*(\.\d*)?$/.test(value)) {
            dispatch(setDirectionY(value))
        }
    }, [dispatch])

    const handleDirectionZChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || /^-?\d*(\.\d*)?$/.test(value)) {
            dispatch(setDirectionZ(value))
        }
    }, [dispatch])

    const handleCreatePointLoadFunc = useCallback(async () => {
        await handleCreatePointLoad(
            apiClient,
            dispatch,
            pointLoadIdInput,
            loadCaseId,
            nodeId,
            magnitude,
            directionX,
            directionY,
            directionZ,
            editorState,
            canvasId
        );
    }, [apiClient, canvasId, dispatch, editorState, pointLoadIdInput, loadCaseId, nodeId, magnitude, directionX, directionY, directionZ])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Id
            </Typography>
            <Autocomplete
                options={pointLoadIds}
                getOptionLabel={(option: string | PointLoadIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    pointLoadIds.find(
                        pl => typeof pl !== "string" && pl.value === pointLoadId
                    ) ?? pointLoadIds[0]
                }
                inputValue={pointLoadIdInput}
                onInputChange={handlePointLoadIdInputChange}
                onChange={(
                    _event,
                    newValue: string | PointLoadIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setPointLoadId(null))
                        dispatch(setPointLoadIdInput(newValue))
                    } else {
                        dispatch(setPointLoadId(newValue?.value ?? null))
                        dispatch(setPointLoadIdInput(newValue?.label ?? ""))
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
                Load Case
            </Typography>
            <Autocomplete
                options={Object.keys(modelResponse?.loadCases ?? {})}
                freeSolo
                value={loadCaseId}
                inputValue={loadCaseId}
                onInputChange={handleLoadCaseIdChange}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Load Case*"
                        variant="outlined"
                        size="small"
                    />
                )}
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Node
            </Typography>
            <Autocomplete
                options={Object.keys(modelResponse?.nodes ?? {})}
                freeSolo
                value={nodeId}
                inputValue={nodeId}
                onInputChange={handleNodeIdChange}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Node*"
                        variant="outlined"
                        size="small"
                    />
                )}
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Force
            </Typography>
            <TextField
                label="Magnitude*"
                value={magnitude}
                onChange={handleMagnitudeChange}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Direction
            </Typography>
            <Autocomplete
                options={["-1", "0", "1"]}
                freeSolo
                value={directionX}
                inputValue={directionX}
                onInputChange={handleDirectionXChange}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="X*"
                        variant="outlined"
                        size="small"
                    />
                )}
                sx={{ mb: 1 }}
            />
            <Autocomplete
                options={["-1", "0", "1"]}
                freeSolo
                value={directionY}
                inputValue={directionY}
                onInputChange={handleDirectionYChange}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Y*"
                        variant="outlined"
                        size="small"
                    />
                )}
                sx={{ mb: 1 }}
            />
            <Autocomplete
                options={["-1", "0", "1"]}
                freeSolo
                value={directionZ}
                inputValue={directionZ}
                onInputChange={handleDirectionZChange}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Z*"
                        variant="outlined"
                        size="small"
                    />
                )}
                sx={{ mb: 2 }}
            />

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreatePointLoadFunc(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}
