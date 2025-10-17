import type React from "react"
import { useState } from "react"
import Dialog from "@mui/material/Dialog"
import DialogTitle from "@mui/material/DialogTitle"
import DialogContent from "@mui/material/DialogContent"
import DialogActions from "@mui/material/DialogActions"
import Box from "@mui/material/Box"
import TextField from "@mui/material/TextField"
import Button from "@mui/material/Button"
import { AnalysisSettings, CreateModelRequest, ModelSettings, UnitSettings } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

export type CreateModelDialogProps = {
    open: boolean
    onClose: () => void
    onCreate: (request: CreateModelRequest) => Promise<void>
}

const CreateModelDialog: React.FC<CreateModelDialogProps> = ({
    open,
    onClose,
    onCreate,
}) => {
    const [modelName, setModelName] = useState("")
    const [modelDescription, setModelDescription] = useState("")
    const [unitSettings, setUnitSettings] = useState({
        lengthUnit: 1,
        forceUnit: 2,
        angleUnit: 1,
    })
    const [yAxisUp, setYAxisUp] = useState(true)
    const [analysisType, setAnalysisType] = useState(1)
    const [submitting, setSubmitting] = useState(false)

    const handleClose = () => {
        if (!submitting) {
            setModelName("")
            setModelDescription("")
            setUnitSettings({ lengthUnit: 1, forceUnit: 2, angleUnit: 1 })
            setYAxisUp(true)
            setAnalysisType(1)
            onClose()
        }
    }

    const handleCreate = async () => {
        setSubmitting(true)
        try {
            await onCreate(new CreateModelRequest({
                name: modelName,
                description: modelDescription,
                settings: new ModelSettings({
                    unitSettings: new UnitSettings(unitSettings),
                    yAxisUp,
                    analysisSettings: new AnalysisSettings({ element1DAnalysisType: analysisType }),
                }),
            }))
            handleClose()
        } catch (err) {
            // TODO: show error
            console.error("Failed to create model", err)
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
            <DialogTitle>Create Model</DialogTitle>
            <DialogContent>
                <Box display="flex" flexDirection="column" gap={2} mt={1}>
                    <TextField
                        label="Name"
                        value={modelName}
                        onChange={e => {
                            setModelName(e.target.value)
                        }}
                        fullWidth
                        required
                    />
                    <TextField
                        label="Description"
                        value={modelDescription}
                        onChange={e => {
                            setModelDescription(e.target.value)
                        }}
                        fullWidth
                        multiline
                        minRows={2}
                    />
                    <Box display="flex" gap={2}>
                        <TextField
                            label="Length Unit"
                            type="number"
                            value={unitSettings.lengthUnit}
                            onChange={e => {
                                setUnitSettings(u => ({
                                    ...u,
                                    lengthUnit: Number(e.target.value),
                                }))
                            }}
                            fullWidth
                        />
                        <TextField
                            label="Force Unit"
                            type="number"
                            value={unitSettings.forceUnit}
                            onChange={e => {
                                setUnitSettings(u => ({
                                    ...u,
                                    forceUnit: Number(e.target.value),
                                }))
                            }}
                            fullWidth
                        />
                        <TextField
                            label="Angle Unit"
                            type="number"
                            value={unitSettings.angleUnit}
                            onChange={e => {
                                setUnitSettings(u => ({
                                    ...u,
                                    angleUnit: Number(e.target.value),
                                }))
                            }}
                            fullWidth
                        />
                    </Box>
                    <Box display="flex" gap={2} alignItems="center">
                        <label>Y Axis Up</label>
                        <input
                            type="checkbox"
                            checked={yAxisUp}
                            onChange={e => {
                                setYAxisUp(e.target.checked)
                            }}
                        />
                    </Box>
                    <TextField
                        label="Element 1D Analysis Type"
                        type="number"
                        value={analysisType}
                        onChange={e => {
                            setAnalysisType(Number(e.target.value))
                        }}
                        fullWidth
                    />
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} disabled={submitting}>
                    Cancel
                </Button>
                <Button
                    onClick={() => {
                        void handleCreate()
                    }}
                    disabled={submitting || !modelName}
                    variant="contained"
                >
                    Create
                </Button>
            </DialogActions>
        </Dialog>
    )
}

export default CreateModelDialog
