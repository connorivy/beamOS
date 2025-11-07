import type React from "react"
import { useState } from "react"
import Dialog from "@mui/material/Dialog"
import DialogTitle from "@mui/material/DialogTitle"
import DialogContent from "@mui/material/DialogContent"
import DialogActions from "@mui/material/DialogActions"
import {
  AngleUnit,
  LengthUnit,
} from "../../utils/type-extensions/UnitTypeContracts"
import { ForceUnit } from "../../utils/type-extensions/UnitTypeContracts"
import Box from "@mui/material/Box"
import Select from "@mui/material/Select"
import MenuItem from "@mui/material/MenuItem"
import TextField from "@mui/material/TextField"
import Button from "@mui/material/Button"
import type {
  CreateModelRequest
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1";
import { Element1dAnalysisType } from "../../utils/type-extensions/EnumContracts"

type UnitSelectProps = {
  label: string
  value: number | undefined
  unitEnum: Record<string, number>
  onChange: (value: number | undefined) => void
}

const UnitSelect: React.FC<UnitSelectProps> = ({
  label,
  value,
  unitEnum,
  onChange,
}) => (
  <Select
    label={label}
    displayEmpty
    value={value === undefined ? "" : String(value)}
    onChange={e => {
      const v = e.target.value === "" ? undefined : parseInt(e.target.value, 10)
      onChange(v)
    }}
    fullWidth
    renderValue={selected => {
      if (!selected) {
        return label
      }
      const entry = Object.entries(unitEnum).find(
        ([_, val]) => String(val) === selected,
      )
      return entry ? entry[0] : label
    }}
  >
    <MenuItem value="" disabled>
      {label}
    </MenuItem>
    {Object.entries(unitEnum)
      .filter(([key]) => key !== "Undefined")
      .map(([key, val]) => (
        <MenuItem key={String(val)} value={String(val)}>
          {key}
        </MenuItem>
      ))}
  </Select>
)

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
  const [unitSettings, setUnitSettings] = useState<{
    lengthUnit: number | undefined
    forceUnit: number | undefined
    angleUnit: number | undefined
  }>({
    lengthUnit: undefined,
    forceUnit: undefined,
    angleUnit: undefined,
  })
  const [yAxisUp, setYAxisUp] = useState(false)
  const [analysisType, setAnalysisType] = useState<number | undefined>(
    undefined,
  )
  const [submitting, setSubmitting] = useState(false)

  const handleClose = () => {
    if (!submitting) {
      setModelName("")
      setModelDescription("")
      setUnitSettings({
        lengthUnit: undefined,
        forceUnit: undefined,
        angleUnit: undefined,
      })
      setYAxisUp(true)
      setAnalysisType(1)
      onClose()
    }
  }

  const handleCreate = async () => {
    setSubmitting(true)
    try {
      await onCreate(
        {
          name: modelName,
          description: modelDescription,
          settings: {
            unitSettings: {
              lengthUnit: unitSettings.lengthUnit ?? 1,
              forceUnit: unitSettings.forceUnit ?? 2,
              angleUnit: unitSettings.angleUnit ?? 1,
            },
            yAxisUp,
            analysisSettings: {
              element1DAnalysisType: analysisType,
            },
            workflowSettings: {
              modelingMode: 2, // Independent mode
            },
          },
        },
      )
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
            <UnitSelect
              label="Length Unit"
              value={unitSettings.lengthUnit}
              unitEnum={LengthUnit}
              onChange={val => {
                setUnitSettings(u => ({ ...u, lengthUnit: val }))
              }}
            />
            <UnitSelect
              label="Force Unit"
              value={unitSettings.forceUnit}
              unitEnum={ForceUnit}
              onChange={val => {
                setUnitSettings(u => ({ ...u, forceUnit: val }))
              }}
            />
            <UnitSelect
              label="Angle Unit"
              value={unitSettings.angleUnit}
              unitEnum={AngleUnit}
              onChange={val => {
                setUnitSettings(u => ({ ...u, angleUnit: val }))
              }}
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
          <UnitSelect
            label="Element 1D Analysis Type"
            value={analysisType}
            unitEnum={Element1dAnalysisType}
            onChange={val => {
              setAnalysisType(val)
            }}
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
