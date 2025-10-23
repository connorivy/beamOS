import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import type { Coords, Restraints } from "./nodeSelectionSlice"
import {
    setNodeId,
    setNodeIdInput,
    setCoord,
    setRestraint,
    nodeIdSelector,
    nodeIdInputSelector,
    coordsSelector,
    restraintsSelector,
} from "./nodeSelectionSlice"
import {
    TextField,
    Autocomplete,
    Checkbox,
    FormControlLabel,
    FormGroup,
    Collapse,
    Box as MuiBox,
    Typography,
    Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { useEditors } from "../../EditorContext"
import { handleCreateNode } from "./handleCreateNode"
import { getUnitName, LengthUnit } from "../../../../utils/type-extensions/UnitTypeContracts"
import { convertLength } from "../../../../utils/unitConversion"

type NodeIdOption = {
    label: string;
    value: number | null;
}
function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

const restraintOptions: { key: keyof Restraints; label: string }[] = [
    { key: "CanTranslateAlongX", label: "Can Translate Along X" },
    { key: "CanTranslateAlongY", label: "Can Translate Along Y" },
    { key: "CanTranslateAlongZ", label: "Can Translate Along Z" },
    { key: "CanRotateAboutX", label: "Can Rotate About X" },
    { key: "CanRotateAboutY", label: "Can Rotate About Y" },
    { key: "CanRotateAboutZ", label: "Can Rotate About Z" },
]

// Precision for rounding coordinate values to avoid floating point precision issues
// Using 1e10 allows for 10 decimal places of precision, which is more than sufficient for engineering applications
const COORDINATE_PRECISION_MULTIPLIER = 1e10


export const NodeSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const nodeId = useAppSelector(nodeIdSelector)
    const nodeIdInput = useAppSelector(nodeIdInputSelector)
    const coords = useAppSelector(coordsSelector)
    const restraints = useAppSelector(restraintsSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
    const editors = useEditors()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const nodeIds: NodeIdOption[] = [
        { label: "New Node", value: null },
        ...Object.keys(modelResponse?.nodes ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]
    const lengthUnit = getUnitName(LengthUnit, modelResponse?.settings.unitSettings.lengthUnit ?? LengthUnit.Inch)

    const resetInput = useCallback(() => {
        dispatch(setNodeIdInput(""))
        dispatch(setCoord({ key: "x", value: "" }))
        dispatch(setCoord({ key: "y", value: "" }))
        dispatch(setCoord({ key: "z", value: "" }))
        dispatch(setRestraint({ key: "CanTranslateAlongX", value: false }))
        dispatch(setRestraint({ key: "CanTranslateAlongY", value: false }))
        dispatch(setRestraint({ key: "CanTranslateAlongZ", value: false }))
        dispatch(setRestraint({ key: "CanRotateAboutX", value: false }))
        dispatch(setRestraint({ key: "CanRotateAboutY", value: false }))
        dispatch(setRestraint({ key: "CanRotateAboutZ", value: false }))
    }, [dispatch])

    useEffect(() => {
        // Reset input fields when switching to "New Node"
        if (nodeId === null) {
            resetInput()
        }
        else {
            const node = modelResponse?.nodes[nodeId]
            if (node) {
                const modelLengthUnit = modelResponse?.settings.unitSettings.lengthUnit ?? LengthUnit.Inch

                // Convert node coordinates from their unit to the model's unit for display
                const x = convertLength(node.locationPoint.x, node.locationPoint.lengthUnit, modelLengthUnit)
                const y = convertLength(node.locationPoint.y, node.locationPoint.lengthUnit, modelLengthUnit)
                const z = convertLength(node.locationPoint.z, node.locationPoint.lengthUnit, modelLengthUnit)

                // Round to avoid floating point precision issues (e.g., 1.0999999999999999 -> 1.1)
                const roundedX = Math.round(x * COORDINATE_PRECISION_MULTIPLIER) / COORDINATE_PRECISION_MULTIPLIER
                const roundedY = Math.round(y * COORDINATE_PRECISION_MULTIPLIER) / COORDINATE_PRECISION_MULTIPLIER
                const roundedZ = Math.round(z * COORDINATE_PRECISION_MULTIPLIER) / COORDINATE_PRECISION_MULTIPLIER

                dispatch(setNodeIdInput(nodeId.toString()))
                dispatch(setCoord({ key: "x", value: roundedX.toString() }))
                dispatch(setCoord({ key: "y", value: roundedY.toString() }))
                dispatch(setCoord({ key: "z", value: roundedZ.toString() }))
                dispatch(setRestraint({ key: "CanTranslateAlongX", value: node.restraint.canTranslateAlongX }))
                dispatch(setRestraint({ key: "CanTranslateAlongY", value: node.restraint.canTranslateAlongY }))
                dispatch(setRestraint({ key: "CanTranslateAlongZ", value: node.restraint.canTranslateAlongZ }))
                dispatch(setRestraint({ key: "CanRotateAboutX", value: node.restraint.canRotateAboutX }))
                dispatch(setRestraint({ key: "CanRotateAboutY", value: node.restraint.canRotateAboutY }))
                dispatch(setRestraint({ key: "CanRotateAboutZ", value: node.restraint.canRotateAboutZ }))
            }
        }
    }, [nodeId, dispatch, modelResponse?.nodes, modelResponse?.settings.unitSettings.lengthUnit, resetInput])

    // Only allow whole numbers for nodeId input
    const handleNodeIdInputChange = useCallback((_event: React.SyntheticEvent, value: string) => {
        if (value === "" || isWholeNumber(value)) {
            dispatch(setNodeIdInput(value))
        }
    }, [dispatch])

    // Only allow doubles for coordinates
    const handleCoordChange = useCallback((key: keyof Coords) => (event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
            dispatch(setCoord({ key, value: val }))
        }
    }, [dispatch])

    const handleRestraintChange = useCallback((key: keyof Restraints) => (event: React.ChangeEvent<HTMLInputElement>) => {
        dispatch(setRestraint({ key, value: event.target.checked }))
    }, [dispatch])

    const handleCreateNodeFunc = useCallback(async () => {
        await handleCreateNode(
            apiClient,
            dispatch,
            nodeIdInput,
            coords,
            editorState,
            editors[canvasId],
            restraints,
            canvasId
        );
    }, [apiClient, canvasId, coords, dispatch, editorState, editors, nodeIdInput, restraints])

    return (
        <MuiBox sx={{ px: 2, py: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>
                Node Id
            </Typography>
            <Autocomplete
                options={nodeIds}
                getOptionLabel={(option: string | NodeIdOption) =>
                    typeof option === "string" ? option : option.label
                }
                value={
                    nodeIds.find(
                        n => typeof n !== "string" && n.value === nodeId
                    ) ?? nodeIds[0]
                }
                inputValue={nodeIdInput}
                onInputChange={handleNodeIdInputChange}
                onChange={(
                    _event,
                    newValue: string | NodeIdOption | null
                ) => {
                    if (typeof newValue === "string") {
                        dispatch(setNodeId(null))
                        dispatch(setNodeIdInput(newValue))
                    } else {
                        dispatch(setNodeId(newValue?.value ?? null))
                        dispatch(setNodeIdInput(newValue?.label ?? ""))
                    }
                }}
                renderInput={params => (
                    <TextField
                        {...params}
                        label="Node Id"
                        variant="outlined"
                        size="small"
                    />
                )}
                freeSolo
                sx={{ mb: 2 }}
            />

            <Collapse in>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    LocationPoint
                </Typography>
                <TextField
                    label="X*"
                    value={coords.x}
                    onChange={handleCoordChange("x")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 1 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>{lengthUnit}</Typography> } }}
                />
                <TextField
                    label="Y*"
                    value={coords.y}
                    onChange={handleCoordChange("y")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 1 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>{lengthUnit}</Typography> } }}
                />
                <TextField
                    label="Z*"
                    value={coords.z}
                    onChange={handleCoordChange("z")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 2 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>{lengthUnit}</Typography> } }}
                />
            </Collapse>

            <Collapse in>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    Restraint
                </Typography>
                <FormGroup>
                    {restraintOptions.map(opt => (
                        <FormControlLabel
                            key={opt.key}
                            control={
                                <Checkbox
                                    checked={restraints[opt.key]}
                                    onChange={handleRestraintChange(opt.key)}
                                />
                            }
                            label={opt.label}
                        />
                    ))}
                </FormGroup>
            </Collapse>

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateNodeFunc(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}