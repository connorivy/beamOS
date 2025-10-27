import Box from '@mui/material/Box'
import IconButton from '@mui/material/IconButton'
import LockIcon from '@mui/icons-material/Lock'
import CalculateIcon from '@mui/icons-material/Calculate'
import { Tooltip } from "@mui/material"
import { useAppSelector } from "../../../app/hooks"
import { selectModelResponseByCanvasId } from "../editorsSlice"
// import HomeIcon from '@mui/icons-material/Home'
// import ZoomInIcon from '@mui/icons-material/ZoomIn'
// import SettingsIcon from '@mui/icons-material/Settings'

export const Toolbar: React.FC<{ canvasId: string }> = ({ canvasId }) => {
    const modelState = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const hasResults = modelState?.resultSets && Object.keys(modelState.resultSets).length > 0
    // const hasResults = React.useMemo(
    //     () => modelState?.resultSets && Object.keys(modelState.resultSets).length > 0,
    //     [modelState?.resultSets]
    // )
    return (
        <Box
            sx={{
                position: 'absolute',
                top: 0,
                left: '50%',
                transform: 'translateX(-50%)',
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                bgcolor: 'background.paper',
                borderRadius: 2,
                boxShadow: 4,
                p: 1.5,
                zIndex: 1000,
            }}
        >
            <IconButton>
                {hasResults ? (
                    <Tooltip title="Unlock model">
                        <LockIcon />
                    </Tooltip>
                ) : (
                    <Tooltip title="Analyze">
                        <CalculateIcon />
                    </Tooltip>
                )}
            </IconButton>
            {/* <IconButton color="primary" title="Home">
                <HomeIcon />
            </IconButton>
            <IconButton color="primary" title="Zoom">
                <ZoomInIcon />
            </IconButton>
            <IconButton color="primary" title="Settings">
                <SettingsIcon />
            </IconButton> */}
        </Box>
    )
}
