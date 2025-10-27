import { useEffect, useState } from "react"
import Box from "@mui/material/Box"
import Button from "@mui/material/Button"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import Typography from "@mui/material/Typography"
import Divider from "@mui/material/Divider"
import LinearScaleIcon from "@mui/icons-material/LinearScale"
import WindowIcon from "@mui/icons-material/Window"
import AssignmentIcon from "@mui/icons-material/Assignment"
import FormatListNumberedIcon from "@mui/icons-material/FormatListNumbered"
import TitleIcon from "@mui/icons-material/Title"
import CircleIcon from "@mui/icons-material/Circle"
import ReplayIcon from "@mui/icons-material/Replay"
import ArrowDownwardIcon from "@mui/icons-material/ArrowDownward"

import { NodeSelectionInfo } from "./node/NodeSelectionInfo"
import { MaterialSelectionInfo } from "./material/MaterialSelectionInfo"
import { useAppDispatch, useAppSelector } from "../../../app/hooks"
import { BeamOsObjectTypes } from "../../three-js-editor/EditorApi/EditorApiAlphaExtensions"
import { selectEditorByCanvasId } from "../editorsSlice"
import { setNodeId } from "./node/nodeSelectionSlice"
import { LoadCaseSelectionInfo } from "./loadCase/LoadCaseSelectionInfo"
import { LoadCombinationSelectionInfo } from "./loadCombination/LoadCombinationSelectionInfo"
import { SectionProfileSelectionInfo } from "./sectionProfile/SectionProfileSelectionInfo"
import { MomentLoadSelectionInfo } from "./momentLoad/MomentLoadSelectionInfo"
import { Element1dSelectionInfo } from "./element1d/Element1dSelectionInfo"
import { PointLoadSelectionInfo } from "./pointLoad/PointLoadSelectionInfo"
import { setElement1dId } from "./element1d/element1dSelectionSlice"
import { setPointLoadId } from "./pointLoad/pointLoadSelectionSlice"
import { setMomentLoadId } from "./momentLoad/momentLoadSelectionSlice"

// Precision for rounding coordinate values to avoid floating point precision issues
// Using 1e4 allows for 4 decimal places of precision
export const COORDINATE_PRECISION_MULTIPLIER = 1e4


const elementTypes = [
  {
    key: "nodes",
    label: "Nodes",
    icon: <CircleIcon sx={{ mr: 1 }} />,
    component: NodeSelectionInfo,
  },
  {
    key: "element1ds",
    label: "Element1Ds",
    icon: <LinearScaleIcon sx={{ mr: 1 }} />,
    component: Element1dSelectionInfo,
  },
  {
    key: "materials",
    label: "Materials",
    icon: <WindowIcon sx={{ mr: 1 }} />,
    component: MaterialSelectionInfo,
  },
  {
    key: "sections",
    label: "Sections",
    icon: <TitleIcon sx={{ mr: 1 }} />,
    component: SectionProfileSelectionInfo,
  },
]

const loadTypes = [
  {
    key: "pointloads",
    label: "Point Loads",
    icon: <ArrowDownwardIcon sx={{ mr: 1 }} />,
    component: PointLoadSelectionInfo,
  },
  {
    key: "momentloads",
    label: "Moment Loads",
    icon: <ReplayIcon sx={{ mr: 1 }} />,
    component: MomentLoadSelectionInfo,
  },
  {
    key: "loadcases",
    label: "Load Cases",
    icon: <AssignmentIcon sx={{ mr: 1 }} />,
    component: LoadCaseSelectionInfo,
  },
  {
    key: "loadcombinations",
    label: "Load Combinations",
    icon: <FormatListNumberedIcon sx={{ mr: 1 }} />,
    component: LoadCombinationSelectionInfo,
  },
]

export default function SelectionInfo({ canvasId }: { canvasId: string }) {
  const [selectedType, setSelectedType] = useState<string | null>(null)
  const editorState = useAppSelector(state => selectEditorByCanvasId(state, canvasId))
  const dispatch = useAppDispatch()

  useEffect(() => {
    if (editorState?.selection === null || editorState?.selection.length === 0) {
      setSelectedType(null)
      dispatch(setNodeId(null))
    }
    else if (editorState?.selection.length === 1 && editorState.selection[0].objectType == BeamOsObjectTypes.Node) {
      setSelectedType("nodes")
      dispatch(setNodeId(editorState.selection[0].id))
    }
    else if (editorState?.selection.length === 1 && editorState.selection[0].objectType == BeamOsObjectTypes.Element1d) {
      setSelectedType("element1ds")
      dispatch(setElement1dId(editorState.selection[0].id))
    }
    else if (editorState?.selection.length == 1 && editorState.selection[0].objectType == BeamOsObjectTypes.PointLoad) {
      setSelectedType("pointloads")
      dispatch(setPointLoadId(editorState.selection[0].id))
    }
    else if (editorState?.selection.length == 1 && editorState.selection[0].objectType == BeamOsObjectTypes.MomentLoad) {
      setSelectedType("momentloads")
      dispatch(setMomentLoadId(editorState.selection[0].id))
    }
  }, [canvasId, dispatch, editorState?.selection])

  if (selectedType) {
    const SelectedComponent = [...elementTypes, ...loadTypes].find(
      e => e.key === selectedType,
    )?.component
    return (
      <Box
        sx={{
          width: "100%",
          height: "100%",
          display: "flex",
          flexDirection: "column",
          p: 0,
        }}
      >
        <Button
          variant="outlined"
          onClick={() => {
            setSelectedType(null)
          }}
          sx={{ m: 2 }}
        >
          ← Back
        </Button>
        <Box sx={{ flex: 1, px: 2 }}>
          {SelectedComponent ? (
            <Box mt={2}>
              <SelectedComponent canvasId={canvasId} />
            </Box>
          ) : null}
        </Box>
      </Box>
    )
  }

  return (
    <>
      {/* Main sections */}
      <Box sx={{ flex: 1, px: 2, overflowY: "auto" }}>
        <Typography variant="subtitle2" sx={{ my: 1 }}>
          Elements
        </Typography>
        <Divider sx={{ my: 1 }} />
        <List sx={{ mb: 2 }}>
          {elementTypes.map(e => (
            <ListItem key={e.key} disablePadding>
              <Button
                startIcon={e.icon}
                sx={{
                  color: "grey.100",
                  justifyContent: "flex-start",
                  width: "100%",
                  textTransform: "none",
                  fontWeight: 500,
                  pl: 1,
                  mb: 0.5,
                  bgcolor: "transparent",
                  "&:hover": { bgcolor: "grey.900" },
                }}
                onClick={() => {
                  setSelectedType(e.key)
                }}
              >
                {e.label}
              </Button>
            </ListItem>
          ))}
        </List>
        <Typography variant="subtitle2" sx={{ my: 1 }}>
          Loads
        </Typography>
        <Divider sx={{ my: 1 }} />
        <List sx={{ mb: 2 }}>
          {loadTypes.map(e => (
            <ListItem key={e.key} disablePadding>
              <Button
                startIcon={e.icon}
                sx={{
                  color: "grey.100",
                  justifyContent: "flex-start",
                  width: "100%",
                  textTransform: "none",
                  fontWeight: 500,
                  pl: 1,
                  mb: 0.5,
                  bgcolor: "transparent",
                  "&:hover": { bgcolor: "grey.900" },
                }}
                onClick={() => {
                  setSelectedType(e.key)
                }}
              >
                {e.label}
              </Button>
            </ListItem>
          ))}
        </List>
      </Box>

      {/* Bottom section */}
      {/* <Box sx={{ px: 2, py: 2, bgcolor: 'grey.900', borderRadius: 2, m: 2 }}>
                <Typography variant="caption" sx={{ color: 'grey.400' }}>What's new</Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', mt: 0.5 }}>
                    <InfoIcon sx={{ color: 'grey.400', fontSize: 18, mr: 1 }} />
                    <Typography variant="body2" sx={{ color: 'grey.100' }}>Mobile app redesign</Typography>
                </Box>
            </Box> */}
    </>
  )
}
