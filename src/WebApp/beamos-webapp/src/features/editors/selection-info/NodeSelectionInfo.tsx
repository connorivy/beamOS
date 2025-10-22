import { useCallback, useEffect } from "react"
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
import { createNode, removeNodeById, selectModelResponseByCanvasId } from "../editorsSlice"
import type { CreateNodeRequest2, NodeResponse } from "../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { useApiClient } from "../../api-client/ApiClientContext"
import { useEditors } from "../EditorContext"
// import { BeamOsObjectTypes } from "../../three-js-editor/EditorApi/EditorApiAlphaExtensions"

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
    const apiClient = useApiClient()
    const editors = useEditors()
    const editorState = useAppSelector(state => state.editors[canvasId])
    const nodeIds: NodeIdOption[] = [
        { label: "New Node", value: null },
        ...Object.keys(modelResponse?.nodes ?? {}).map(id => ({ label: id, value: Number(id) }))
    ]

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

    // useEffect(() => {
    //     if (editorState.selection?.length === 1 && editorState.selection[0].objectType === BeamOsObjectTypes.Node) {
    //         dispatch(setNodeId(editorState.selection[0].id))
    //     }
    // }, [dispatch, editorState.selection])

    useEffect(() => {
        // Reset input fields when switching to "New Node"
        if (nodeId === null) {
            resetInput()
        }
        else {
            const node = modelResponse?.nodes[nodeId]
            if (node) {
                dispatch(setNodeIdInput(nodeId.toString()))
                dispatch(setCoord({ key: "x", value: node.locationPoint.x.toString() }))
                dispatch(setCoord({ key: "y", value: node.locationPoint.y.toString() }))
                dispatch(setCoord({ key: "z", value: node.locationPoint.z.toString() }))
                dispatch(setRestraint({ key: "CanTranslateAlongX", value: node.restraint.canTranslateAlongX }))
                dispatch(setRestraint({ key: "CanTranslateAlongY", value: node.restraint.canTranslateAlongY }))
                dispatch(setRestraint({ key: "CanTranslateAlongZ", value: node.restraint.canTranslateAlongZ }))
                dispatch(setRestraint({ key: "CanRotateAboutX", value: node.restraint.canRotateAboutX }))
                dispatch(setRestraint({ key: "CanRotateAboutY", value: node.restraint.canRotateAboutY }))
                dispatch(setRestraint({ key: "CanRotateAboutZ", value: node.restraint.canRotateAboutZ }))
            }
        }
    }, [nodeId, dispatch, modelResponse?.nodes, resetInput])

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
        if (nodeIdInput || nodeIdInput !== "") {
            console.error("Node ID cannot be specified for new node")
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
        if (!editorState.model) {
            console.error("Editor model is not loaded")
            return
        }

        const lengthUnit = editorState.model.settings.unitSettings.lengthUnit;

        // Create the node request
        const createNodeRequest: CreateNodeRequest2 = {
            id: undefined,
            locationPoint: {
                x: parseFloat(coords.x),
                y: parseFloat(coords.y),
                z: parseFloat(coords.z),
                lengthUnit: lengthUnit,
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

        // Call the API to create the node
        const createNodePromise = apiClient.createNode(
            editorState.remoteModelId,
            createNodeRequest)

        // get a unique temporary id as a number from the current time
        const uniqueTempId = -1 * Date.now();
        const nodeResponse: NodeResponse = { id: uniqueTempId, modelId: editorState.remoteModelId, locationPoint: createNodeRequest.locationPoint, restraint: createNodeRequest.restraint }

        // Optimistically update the ui
        const editor = editors[canvasId];
        await editor.api.createNode(nodeResponse);

        // Optimistically update the store
        dispatch(createNode({ canvasId, node: nodeResponse }));


        try {
            const realNodeResponse = await createNodePromise;

            // remove the optimistically created node and replace with real one
            console.log(`Real node response received: ${JSON.stringify(realNodeResponse)}`);
            await editor.api.createNode(realNodeResponse);
            await editor.api.deleteNode({ id: uniqueTempId, modelId: editorState.remoteModelId });
            dispatch(removeNodeById({ canvasId, nodeId: uniqueTempId }));
            dispatch(createNode({ canvasId, node: realNodeResponse }));
        }
        catch (error) {
            console.error("Failed to create node:", error);
            // Optionally, you might want to implement rollback logic here
        }


    }, [apiClient, canvasId, coords.x, coords.y, coords.z, dispatch, editorState.model, editorState.remoteModelId, editors, nodeIdInput, restraints.CanRotateAboutX, restraints.CanRotateAboutY, restraints.CanRotateAboutZ, restraints.CanTranslateAlongX, restraints.CanTranslateAlongY, restraints.CanTranslateAlongZ])

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

            <Button variant="contained" sx={{ mt: 2, width: "100%" }} onClick={() => { void handleCreateNode(); }}>
                CREATE
            </Button>
        </MuiBox>
    )
}