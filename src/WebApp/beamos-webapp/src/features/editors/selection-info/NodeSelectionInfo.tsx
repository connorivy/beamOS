import { useCallback } from "react"
import { useAppSelector, useAppDispatch } from "../../../app/hooks"
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
import { selectModelResponseByCanvasId } from "../editorsSlice"

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


export const NodeSelectionInfo = ({ canvasId }: { canvasId: string }) => {
    const dispatch = useAppDispatch()
    const nodeId = useAppSelector(nodeIdSelector)
    const nodeIdInput = useAppSelector(nodeIdInputSelector)
    const coords = useAppSelector(coordsSelector)
    const restraints = useAppSelector(restraintsSelector)
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const nodeIds: (string | NodeIdOption)[] = [
        { label: "New Node", value: null },
        ...modelResponse?.nodes?.map(n => ({ label: n.id.toString(), value: n.id })).sort() ?? []
    ]

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
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>Inch</Typography> } }}
                />
                <TextField
                    label="Y*"
                    value={coords.y}
                    onChange={handleCoordChange("y")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 1 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>Inch</Typography> } }}
                />
                <TextField
                    label="Z*"
                    value={coords.z}
                    onChange={handleCoordChange("z")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 2 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>Inch</Typography> } }}
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

            <Button variant="contained" sx={{ mt: 2, width: "100%" }}>
                CREATE
            </Button>
        </MuiBox>
    )
}