import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import {
    setMomentLoadId,
    setMomentLoadIdInput,
    setLoadCaseId,
    setNodeId,
    setMagnitude,
    setDirection,
    momentLoadIdSelector,
    momentLoadIdInputSelector,
    loadCaseIdSelector,
    nodeIdSelector,
    magnitudeSelector,
    directionSelector,
} from "./momentLoadSelectionSlice"
import {
    TextField,
    Autocomplete,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreateMomentLoad } from "./handleCreateMomentLoad"
import { getTorqueUnit } from "../../../../utils/type-extensions/UnitTypeContracts"
import { convertTorque } from "../../../../utils/unitConversion"
import { COORDINATE_PRECISION_MULTIPLIER } from "../SelectionInfo"

type MomentLoadIdOption = {
    label: string;
    value: number | null;
}
function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

export const MomentLoadSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const momentLoadId = useAppSelector(momentLoadIdSelector)
    const momentLoadIdInput = useAppSelector(momentLoadIdInputSelector)
    const loadCaseId = useAppSelector(loadCaseIdSelector)
    const nodeId = useAppSelector(nodeIdSelector)
    const magnitude = useAppSelector(magnitudeSelector)
    const direction = useAppSelector(directionSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const momentLoadIds: MomentLoadIdOption[] = [
        { label: "New Moment Load", value: null },
        ...Object.keys(modelResponse?.momentLoads ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

    const resetInput = useCallback(() => {
        dispatch(setMomentLoadIdInput(""))
        dispatch(setLoadCaseId(""))
        dispatch(setNodeId(""))
        dispatch(setMagnitude(""))
        dispatch(setDirection({ key: "x", value: "" }))
        dispatch(setDirection({ key: "y", value: "" }))
        dispatch(setDirection({ key: "z", value: "" }))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Moment Load"
        if (momentLoadId === null) {
            resetInput()
        }
        else {
            const momentLoad = modelResponse?.momentLoads[momentLoadId]
            if (momentLoad) {
                const modelTorqueUnit = getTorqueUnit(modelResponse.settings.unitSettings.lengthUnit, modelResponse.settings.unitSettings.forceUnit)

                // Convert node coordinates from their unit to the model's unit for display
                const val = convertTorque(momentLoad.torque.value, momentLoad.torque.unit, modelTorqueUnit)

                // Round to avoid floating point precision issues (e.g., 1.0999999999999999 -> 1.1)
                const roundedVal = Math.round(val * COORDINATE_PRECISION_MULTIPLIER) / COORDINATE_PRECISION_MULTIPLIER

                dispatch(setMomentLoadIdInput(momentLoadId.toString()))
                dispatch(setLoadCaseId(momentLoad.loadCaseId.toString()))
                dispatch(setNodeId(momentLoad.nodeId.toString()))
                dispatch(setMagnitude(roundedVal.toString()))
                dispatch(setDirection({ key: "x", value: momentLoad.axisDirection.x.toString() }))
                dispatch(setDirection({ key: "y", value: momentLoad.axisDirection.y.toString() }))
                dispatch(setDirection({ key: "z", value: momentLoad.axisDirection.z.toString() }))
            }
        }
    }, [momentLoadId, dispatch, modelResponse?.momentLoads, resetInput, modelResponse?.settings.unitSettings.lengthUnit, modelResponse?.settings.unitSettings.forceUnit])

    // Only allow whole numbers for momentLoadId input
    const handleMomentLoadIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setMomentLoadIdInput(value))
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

    const handleMagnitudeChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            dispatch(setMagnitude(val))
        }
    }, [dispatch])

    const handleDirectionChange = useCallback((key: "x" | "y" | "z") => (event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            dispatch(setDirection({ key, value: val }))
        }
    }, [dispatch])

    const handleCreateMomentLoadFunc = useCallback(async () => {
        await handleCreateMomentLoad(
            apiClient,
            dispatch,
            momentLoadIdInput,
            loadCaseId,
            nodeId,
            magnitude,
            direction,
            editorState,
            canvasId
        );
    }, [apiClient, canvasId, direction, dispatch, editorState, loadCaseId, magnitude, momentLoadIdInput, nodeId])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Id
            </Typography>
            <Autocomplete
                options={momentLoadIds}
                getOptionLabel={(option: string | MomentLoadIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    momentLoadIds.find(
                        ml => typeof ml !== "string" && ml.value === momentLoadId
                    ) ?? momentLoadIds[0]
                }
                inputValue={momentLoadIdInput}
                onInputChange={handleMomentLoadIdInputChange}
                onChange={(
                    _event,
                    newValue: string | MomentLoadIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setMomentLoadId(null))
                        dispatch(setMomentLoadIdInput(newValue))
                    } else {
                        dispatch(setMomentLoadId(newValue?.value ?? null))
                        dispatch(setMomentLoadIdInput(newValue?.label ?? ""))
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
                Magnitude
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
            <TextField
                label="X*"
                value={direction.x}
                onChange={handleDirectionChange("x")}
                variant="outlined"
                size="small"
                sx={{ mb: 1 }}
            />
            <TextField
                label="Y*"
                value={direction.y}
                onChange={handleDirectionChange("y")}
                variant="outlined"
                size="small"
                sx={{ mb: 1 }}
            />
            <TextField
                label="Z*"
                value={direction.z}
                onChange={handleDirectionChange("z")}
                variant="outlined"
                size="small"
                sx={{ mb: 2 }}
            />

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateMomentLoadFunc(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}
