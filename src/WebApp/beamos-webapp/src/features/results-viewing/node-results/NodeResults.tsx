import { useEffect } from "react"
import { useAppSelector } from "../../../app/hooks"
import { selectModelResponseByCanvasId } from "../../editors/editorsSlice"
import Typography from "@mui/material/Typography"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import Divider from "@mui/material/Divider"
import Stack from "@mui/material/Stack"
import Box from "@mui/material/Box"
import { getAngleUnitLabel, getForceUnitLabel, getLengthUnitLabel, getTorqueUnitLabel } from "../../../utils/type-extensions/UnitTypeContracts"

export const NodeResults: React.FC<{
    canvasId: string
    nodeId: number
    resultSetId: number
}> = ({ canvasId, nodeId, resultSetId }) => {
    const modelState = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId),
    )
    const currentResultSet = modelState?.resultSets[resultSetId]
    const nodeResult =
        currentResultSet && nodeId in currentResultSet.nodes
            ? currentResultSet.nodes[nodeId]
            : null

    // Chart creation and event listeners (if needed)
    useEffect(() => {
        // ...existing code...
    }, [canvasId, nodeId, resultSetId, modelState, currentResultSet])

    // Helper to format numbers
    const fmt = (val: number) => (typeof val === "number" ? val.toFixed(4) : val)

    // Displacement and rotation fields
    const displacementFields = [
        { label: "Displacement X", key: "displacementAlongX" },
        { label: "Displacement Y", key: "displacementAlongY" },
        { label: "Displacement Z", key: "displacementAlongZ" },
    ]
    const rotationFields = [
        { label: "Rotation X", key: "rotationAboutX" },
        { label: "Rotation Y", key: "rotationAboutY" },
        { label: "Rotation Z", key: "rotationAboutZ" },
    ]

    // Force and moment fields
    const forceFields = [
        { label: "Force X", key: "forceAlongX" },
        { label: "Force Y", key: "forceAlongY" },
        { label: "Force Z", key: "forceAlongZ" },
    ]
    const momentFields = [
        { label: "Moment X", key: "momentAboutX" },
        { label: "Moment Y", key: "momentAboutY" },
        { label: "Moment Z", key: "momentAboutZ" },
    ]

    return (
        <Box sx={{ width: "100%", maxWidth: 420, mx: "auto", mt: 2 }}>
            {nodeResult ? (
                <Stack spacing={2} sx={{ p: 2 }}>
                    <Typography align="center" variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
                        Displacements
                    </Typography>
                    <Divider sx={{ my: 1 }} />
                    <List disablePadding>
                        {displacementFields.map(field => {
                            const disp = nodeResult.displacements[field.key as keyof typeof nodeResult.displacements] as { value: number; unit: number };
                            return (
                                <ListItem
                                    key={field.key}
                                    sx={{ display: "flex", justifyContent: "space-between", py: 1 }}
                                >
                                    <span>{field.label}</span>
                                    <span>
                                        {fmt(disp.value)}{' '}
                                        <Typography variant="caption" color="text.secondary" component="span">
                                            [{getLengthUnitLabel(disp.unit)}]
                                        </Typography>
                                    </span>
                                </ListItem>
                            );
                        })}
                        {rotationFields.map(field => {
                            const rot = nodeResult.displacements[field.key as keyof typeof nodeResult.displacements] as { value: number; unit: number };
                            return (
                                <ListItem
                                    key={field.key}
                                    sx={{ display: "flex", justifyContent: "space-between", py: 1 }}
                                >
                                    <span>{field.label}</span>
                                    <span>
                                        {fmt(rot.value)}{' '}
                                        <Typography variant="caption" color="text.secondary" component="span">
                                            [{getAngleUnitLabel(rot.unit)}]
                                        </Typography>
                                    </span>
                                </ListItem>
                            );
                        })}
                    </List>
                    <Typography align="center" variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
                        Reactions (Forces &amp; Moments)
                    </Typography>
                    <List disablePadding>
                        {forceFields.map(field => {
                            const force = nodeResult.forces[field.key as keyof typeof nodeResult.forces] as { value: number; unit: number };
                            return (
                                <ListItem
                                    key={field.key}
                                    sx={{ display: "flex", justifyContent: "space-between", py: 1 }}
                                >
                                    <span>{field.label}</span>
                                    <span>
                                        {fmt(force.value)}{' '}
                                        <Typography variant="caption" color="text.secondary" component="span">
                                            [{getForceUnitLabel(force.unit)}]
                                        </Typography>
                                    </span>
                                </ListItem>
                            );
                        })}
                        {momentFields.map(field => {
                            const moment = nodeResult.forces[field.key as keyof typeof nodeResult.forces] as { value: number; unit: number };
                            return (
                                <ListItem
                                    key={field.key}
                                    sx={{ display: "flex", justifyContent: "space-between", py: 1 }}
                                >
                                    <span>{field.label}</span>
                                    <span>
                                        {fmt(moment.value)}{' '}
                                        <Typography variant="caption" color="text.secondary" component="span">
                                            [{getTorqueUnitLabel(moment.unit)}]
                                        </Typography>
                                    </span>
                                </ListItem>
                            );
                        })}
                    </List>
                </Stack>
            ) : (
                <Typography color="error" sx={{ mt: 2 }}>
                    No results found for this node.
                </Typography>
            )}
        </Box>
    )
}
