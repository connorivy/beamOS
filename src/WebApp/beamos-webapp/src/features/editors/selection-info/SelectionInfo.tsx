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
import { useAppDispatch, useAppSelector } from "../../../app/hooks"
import { BeamOsObjectTypes } from "../../three-js-editor/EditorApi/EditorApiAlphaExtensions"
import { selectEditorByCanvasId } from "../editorsSlice"
import { setNodeId } from "./node/nodeSelectionSlice"
import { Element1DSelectionInfo } from "./element1D/Element1DSelectionInfo"

const Materials = () => (
  <Typography variant="body1" color="grey.300">
    Materials Component (empty)
  </Typography>
)
const Sections = () => (
  <Typography variant="body1" color="grey.300">
    Sections Component (empty)
  </Typography>
)
const PointLoads = () => (
  <Typography variant="body1" color="grey.300">
    PointLoads Component (empty)
  </Typography>
)
const MomentLoads = () => (
  <Typography variant="body1" color="grey.300">
    MomentLoads Component (empty)
  </Typography>
)
const LoadCases = () => (
  <Typography variant="body1" color="grey.300">
    LoadCases Component (empty)
  </Typography>
)
const LoadCombinations = () => (
  <Typography variant="body1" color="grey.300">
    LoadCombinations Component (empty)
  </Typography>
)

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
    component: Element1DSelectionInfo,
  },
  {
    key: "materials",
    label: "Materials",
    icon: <WindowIcon sx={{ mr: 1 }} />,
    component: Materials,
  },
  {
    key: "sections",
    label: "Sections",
    icon: <TitleIcon sx={{ mr: 1 }} />,
    component: Sections,
  },
]

const loadTypes = [
  {
    key: "pointloads",
    label: "Point Loads",
    icon: <ArrowDownwardIcon sx={{ mr: 1 }} />,
    component: PointLoads,
  },
  {
    key: "momentloads",
    label: "Moment Loads",
    icon: <ReplayIcon sx={{ mr: 1 }} />,
    component: MomentLoads,
  },
  {
    key: "loadcases",
    label: "Load Cases",
    icon: <AssignmentIcon sx={{ mr: 1 }} />,
    component: LoadCases,
  },
  {
    key: "loadcombinations",
    label: "Load Combinations",
    icon: <FormatListNumberedIcon sx={{ mr: 1 }} />,
    component: LoadCombinations,
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
  }, [canvasId, dispatch, editorState?.selection])

  if (selectedType) {
    const SelectedComponent = elementTypes.find(
      e => e.key === selectedType,
    )?.component
    return (
      <Box
        sx={{
          width: 320,
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
