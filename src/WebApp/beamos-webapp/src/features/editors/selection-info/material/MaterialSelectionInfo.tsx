import { useCallback, useEffect } from "react"
import { useAppSelector, useAppDispatch } from "../../../../app/hooks"
import type { MaterialProperties } from "./materialSelectionSlice"
import {
  setMaterialId,
  setMaterialIdInput,
  setMaterialProperty,
  materialIdSelector,
  materialIdInputSelector,
  materialPropertiesSelector,
} from "./materialSelectionSlice"
import {
  TextField,
  Autocomplete,
  Box as MuiBox,
  Typography,
  Button,
} from "@mui/material"
import { selectModelResponseByCanvasId } from "../../editorsSlice"
import { useApiClient } from "../../../api-client/ApiClientContext"
import { handleCreateMaterial } from "./handleCreateMaterial"
import {
  getUnitName,
  PressureUnit,
} from "../../../../utils/type-extensions/UnitTypeContracts"

type MaterialIdOption = {
  label: string
  value: number | null
}

function isWholeNumber(val: string) {
  return /^\d+$/.test(val)
}

export const MaterialSelectionInfo = ({ canvasId }: { canvasId: string }) => {
  const dispatch = useAppDispatch()
  const materialId = useAppSelector(materialIdSelector)
  const materialIdInput = useAppSelector(materialIdInputSelector)
  const properties = useAppSelector(materialPropertiesSelector)
  const modelResponse = useAppSelector(state =>
    selectModelResponseByCanvasId(state, canvasId),
  )
  const apiClient = useApiClient()
  const editorState = useAppSelector(state => state.editors[canvasId])
  const materialIds: MaterialIdOption[] = [
    { label: "New Material", value: null },
    ...Object.keys(modelResponse?.materials ?? {}).map(id => ({
      label: id,
      value: Number(id),
    })),
  ]
  const pressureUnit = getUnitName(
    PressureUnit,
    modelResponse?.settings.unitSettings.pressureUnit ??
      PressureUnit.PoundForcePerSquareInch,
  )

  const resetInput = useCallback(() => {
    dispatch(setMaterialIdInput(""))
    dispatch(
      setMaterialProperty({ key: "modulusOfElasticity", value: "" }),
    )
    dispatch(setMaterialProperty({ key: "modulusOfRigidity", value: "" }))
  }, [dispatch])

  useEffect(() => {
    // Reset input fields when switching to "New Material"
    if (materialId === null) {
      resetInput()
    } else {
      const material = modelResponse?.materials[materialId]
      if (material) {
        dispatch(setMaterialIdInput(materialId.toString()))
        dispatch(
          setMaterialProperty({
            key: "modulusOfElasticity",
            value: material.modulusOfElasticity.toString(),
          }),
        )
        dispatch(
          setMaterialProperty({
            key: "modulusOfRigidity",
            value: material.modulusOfRigidity.toString(),
          }),
        )
      }
    }
  }, [materialId, dispatch, modelResponse?.materials, resetInput])

  // Only allow whole numbers for materialId input
  const handleMaterialIdInputChange = useCallback(
    (_event: React.SyntheticEvent, value: string) => {
      if (value === "" || isWholeNumber(value)) {
        dispatch(setMaterialIdInput(value))
      }
    },
    [dispatch],
  )

  // Only allow doubles for properties
  const handlePropertyChange = useCallback(
    (key: keyof MaterialProperties) =>
      (event: React.ChangeEvent<HTMLInputElement>) => {
        const val = event.target.value
        if (val === "" || /^-?\d*(\.\d*)?$/.test(val)) {
          dispatch(setMaterialProperty({ key, value: val }))
        }
      },
    [dispatch],
  )

  const handleCreateMaterialFunc = useCallback(async () => {
    await handleCreateMaterial(
      apiClient,
      dispatch,
      materialIdInput,
      properties,
      editorState,
      canvasId,
    )
  }, [apiClient, canvasId, dispatch, editorState, materialIdInput, properties])

  return (
    <MuiBox sx={{ px: 2, py: 2 }}>
      <Typography variant="subtitle2" sx={{ mb: 1 }}>
        Material Id
      </Typography>
      <Autocomplete
        options={materialIds}
        getOptionLabel={(option: string | MaterialIdOption) =>
          typeof option === "string" ? option : option.label
        }
        value={
          materialIds.find(
            n => typeof n !== "string" && n.value === materialId,
          ) ?? materialIds[0]
        }
        inputValue={materialIdInput}
        onInputChange={handleMaterialIdInputChange}
        onChange={(
          _event,
          newValue: string | MaterialIdOption | null,
        ) => {
          if (typeof newValue === "string") {
            dispatch(setMaterialId(null))
            dispatch(setMaterialIdInput(newValue))
          } else {
            dispatch(setMaterialId(newValue?.value ?? null))
            dispatch(setMaterialIdInput(newValue?.label ?? ""))
          }
        }}
        renderInput={params => (
          <TextField
            {...params}
            label="Material Id"
            variant="outlined"
            size="small"
          />
        )}
        freeSolo
        sx={{ mb: 2 }}
      />

      <Typography variant="subtitle2" sx={{ mb: 1 }}>
        Material Properties
      </Typography>
      <TextField
        label="Modulus of Elasticity*"
        value={properties.modulusOfElasticity}
        onChange={handlePropertyChange("modulusOfElasticity")}
        variant="outlined"
        size="small"
        sx={{ mb: 1 }}
        slotProps={{
          input: {
            endAdornment: <Typography sx={{ ml: 1 }}>{pressureUnit}</Typography>,
          },
        }}
      />
      <TextField
        label="Modulus of Rigidity*"
        value={properties.modulusOfRigidity}
        onChange={handlePropertyChange("modulusOfRigidity")}
        variant="outlined"
        size="small"
        sx={{ mb: 2 }}
        slotProps={{
          input: {
            endAdornment: <Typography sx={{ ml: 1 }}>{pressureUnit}</Typography>,
          },
        }}
      />

      <Button
        variant="contained"
        sx={{ mt: 2, width: "100%" }}
        onClick={() => {
          void handleCreateMaterialFunc()
        }}
      >
        CREATE
      </Button>
    </MuiBox>
  )
}
