import { useMemo, useState } from 'react';
import { Box, Select, MenuItem, List, ListItem, Typography, ListItemButton, ListItemIcon } from '@mui/material';
import TimelineIcon from '@mui/icons-material/Timeline';
// import LinearScaleIcon from '@mui/icons-material/LinearScale';
// import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
// import LayersIcon from '@mui/icons-material/Layers';
// import FunctionsIcon from '@mui/icons-material/Functions';
// import OpenWithIcon from '@mui/icons-material/OpenWith';
// import WarningIcon from '@mui/icons-material/Warning';
// import SpeedIcon from '@mui/icons-material/Speed';
// import GroupWorkIcon from '@mui/icons-material/GroupWork';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { selectModelResponseByCanvasId, selectSelectedResultSetId, setSelectedResultSetId } from '../editors/editorsSlice';
import type { ModelState } from '../editors/ModelState';
import { handleViewDeflectionResults, handleViewMomentResults, handleViewShearResults } from './handleViewDeflectionResults';
import { useApiClient } from '../api-client/ApiClientContext';
import { useEditors } from '../editors/EditorContext';
import SwapVertIcon from '@mui/icons-material/SwapVert';
import ReplayIcon from '@mui/icons-material/Replay';

type ResultsInfoProps = {
    canvasId: string;
};


export function ResultsInfo({ canvasId }: ResultsInfoProps) {
    const modelState = useAppSelector(
        state => selectModelResponseByCanvasId(state, canvasId)
    ) as ModelState | null;
    const resultSetIds = useMemo(() =>
        modelState ? Object.keys(modelState.resultSets) : [],
        [modelState]
    );
    const dispatch = useAppDispatch()
    let selectedResultSetId = useAppSelector(state => selectSelectedResultSetId(state, canvasId));
    const [selectedLabel, setSelectedLabel] = useState<string | null>(null);
    const apiClient = useApiClient()
    const editor = useEditors()[canvasId];
    const editorState = useAppSelector(state => state.editors[canvasId])
    selectedResultSetId ??= Number(resultSetIds[0]);

    const resultTypes = [
        {
            label: 'Shear',
            icon: <SwapVertIcon sx={{ mr: 1 }} />,
            onClick: () => {
                void handleViewShearResults(
                    apiClient,
                    dispatch,
                    selectedResultSetId,
                    editor,
                    editorState,
                    modelState,
                    canvasId
                )
            }
        },
        {
            label: 'Moment',
            icon: <ReplayIcon sx={{ mr: 1 }} />,
            onClick: () => {
                void handleViewMomentResults(
                    apiClient,
                    dispatch,
                    selectedResultSetId,
                    editor,
                    editorState,
                    modelState,
                    canvasId
                )
            }
        },
        // { label: 'Axial', icon: <LinearScaleIcon sx={{ mr: 1 }} /> },
        // { label: 'Torsion', icon: <OpenWithIcon sx={{ mr: 1 }} /> },
        {
            label: 'Deflection',
            icon: <TimelineIcon sx={{ mr: 1 }} />,
            onClick: () => {
                void handleViewDeflectionResults(
                    apiClient,
                    dispatch,
                    selectedResultSetId,
                    editor,
                    editorState,
                    modelState,
                    canvasId
                )
            }
        },
        // { label: 'Stress', icon: <FunctionsIcon sx={{ mr: 1 }} /> },
        // { label: 'Plates', icon: <LayersIcon sx={{ mr: 1 }} /> },
        // { label: 'Buckling', icon: <WarningIcon sx={{ mr: 1 }} /> },
        // { label: 'Dynamic Frequency', icon: <SpeedIcon sx={{ mr: 1 }} /> },
        // { label: 'Near-Node Member Forces', icon: <GroupWorkIcon sx={{ mr: 1 }} /> },
    ];

    return (
        <Box sx={{ flex: 1, px: 2, overflowY: "auto" }}>
            <Typography variant="subtitle2" sx={{ my: 1 }}>
                Result Set
            </Typography>
            <div className="px-3">
                <Select
                    labelId="result-set-select-label"
                    value={selectedResultSetId}
                    label="Result Set"
                    onChange={e => { dispatch(setSelectedResultSetId({ canvasId: canvasId, selectedResultSetId: e.target.value })); }}
                    sx={{ width: '100%' }}
                >
                    {resultSetIds.map((id: string) => (
                        <MenuItem key={id} value={id}>{id}</MenuItem>
                    ))}
                </Select>
            </div>
            <List>
                {resultTypes.map(({ label, icon, onClick }) => (
                    <ListItem disablePadding key={label}>
                        <ListItemButton
                            selected={selectedLabel === label}
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
                                if (selectedLabel === label) {
                                    void editor.api.clearCurrentOverlay();
                                    setSelectedLabel(null);
                                    return;
                                }
                                setSelectedLabel(label);
                                onClick();
                            }}
                        >
                            <ListItemIcon>{icon}</ListItemIcon>
                            {label}
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
        </Box>
    );
}

export default ResultsInfo;
