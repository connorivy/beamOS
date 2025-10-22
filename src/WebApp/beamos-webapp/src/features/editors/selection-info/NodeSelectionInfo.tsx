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
import { selectModelResponseByCanvasId, selectEditorByCanvasId, nodeAdded } from "../editorsSlice"
import { useApiClient } from "../../api-client/ApiClientContext"
import type { CreateNodeRequest2 } from "../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

type NodeIdOption = {
    label: string;
    value: number | null;
}
function isWholeNumber(val: string) {
    return /^\d+$/.test(val)
}

const restraintOptions: { key: keyof Restraints; label: string }[] = [
    { key: "CanTranslateAlongX", label: "can translate along x" },
    { key: "CanTranslateAlongY", label: "can translate along y" },
    { key: "CanTranslateAlongZ", label: "can translate along z" },
    { key: "CanRotateAboutX", label: "can rotate about x" },
    { key: "CanRotateAboutY", label: "can rotate about y" },
    { key: "CanRotateAboutZ", label: "can rotate about z" },
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
    const editorState = useAppSelector(state =>
        selectEditorByCanvasId(state, canvasId)
    )
    const apiClient = useApiClient()
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

    const handleCreateNode = useCallback(async () => {
        // Validate input
        if (!nodeIdInput || nodeIdInput === "") {
            console.error("Node ID is required")
            return
        }
        if (!coords.x || !coords.y || !coords.z) {
            console.error("All coordinates are required")
            return
        }
        if (!editorState.remoteModelId) {
            console.error("Remote model ID is not available")
            return
        }

        const nodeIdNumber = parseInt(nodeIdInput, 10)
        
        // Create the node request
        const createNodeRequest: CreateNodeRequest2 = {
            id: nodeIdNumber,
            locationPoint: {
                x: parseFloat(coords.x),
                y: parseFloat(coords.y),
                z: parseFloat(coords.z),
                lengthUnit: 0, // Assuming 0 is the default unit (inches based on UI)
            },
            restraint: {
                canTranslateAlongX: restraints.CanTranslateAlongX,
                canTranslateAlongY: restraints.CanTranslateAlongY,
                canTranslateAlongZ: restraints.CanTranslateAlongZ,
                canRotateAboutX: restraints.CanRotateAboutX,
                canRotateAboutY: restraints.CanRotateAboutY,
                canRotateAboutZ: restraints.CanRotateAboutZ,
            },
        }

        try {
            // 1. Optimistically create node in the BeamOsEditor (UI)
            if (editorState.editorRef) {
                await editorState.editorRef.api.createNode({
                    id: nodeIdNumber,
                    locationPoint: createNodeRequest.locationPoint,
                    restraint: createNodeRequest.restraint,
                })
            }

            // 2. Call the backend API to persist the node
            const nodeResponse = await apiClient.createNode(
                editorState.remoteModelId,
                createNodeRequest
            )

            // 3. Update Redux store with the new node
            dispatch(nodeAdded({ canvasId, node: nodeResponse }))

            console.log("Node created successfully:", nodeResponse)
        } catch (error) {
            console.error("Failed to create node:", error)
            // TODO: Handle error - potentially remove optimistically added node
        }
    }, [nodeIdInput, coords, restraints, editorState, apiClient, dispatch, canvasId])

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
                        label="node id"
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
                    label="x"
                    value={coords.x}
                    onChange={handleCoordChange("x")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 1 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>Inch</Typography> } }}
                />
                <TextField
                    label="y"
                    value={coords.y}
                    onChange={handleCoordChange("y")}
                    variant="outlined"
                    size="small"
                    sx={{ mb: 1 }}
                    slotProps={{ input: { endAdornment: <Typography sx={{ ml: 1 }}>Inch</Typography> } }}
                />
                <TextField
                    label="z"
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

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={handleCreateNode}>
                create
            </Button>
        </MuiBox>
    )
}