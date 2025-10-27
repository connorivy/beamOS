import { useMemo, useState } from 'react';
import { Box, Stack, Select, MenuItem, Button, FormControl, InputLabel, List, ListItem, ListItemIcon, ListItemText, ListItemButton, Typography } from '@mui/material';
import TimelineIcon from '@mui/icons-material/Timeline';
import SyncAltIcon from '@mui/icons-material/SyncAlt';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
// import LinearScaleIcon from '@mui/icons-material/LinearScale';
// import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
// import LayersIcon from '@mui/icons-material/Layers';
// import FunctionsIcon from '@mui/icons-material/Functions';
// import OpenWithIcon from '@mui/icons-material/OpenWith';
// import WarningIcon from '@mui/icons-material/Warning';
// import SpeedIcon from '@mui/icons-material/Speed';
// import GroupWorkIcon from '@mui/icons-material/GroupWork';
import { useAppSelector } from '../../app/hooks';
import { selectModelResponseByCanvasId } from '../editors/editorsSlice';
import type { ModelState } from '../editors/ModelState';

type ResultsInfoProps = {
    canvasId: string;
};

const resultTypes = [
    // { label: 'Reactions', icon: <ArrowDownwardIcon sx={{ mr: 1 }} /> },
    { label: 'Shear', icon: <SyncAltIcon sx={{ mr: 1 }} /> },
    { label: 'Moment', icon: <TimelineIcon sx={{ mr: 1 }} /> },
    // { label: 'Axial', icon: <LinearScaleIcon sx={{ mr: 1 }} /> },
    // { label: 'Torsion', icon: <OpenWithIcon sx={{ mr: 1 }} /> },
    { label: 'Deflection', icon: <TrendingUpIcon sx={{ mr: 1 }} /> },
    // { label: 'Stress', icon: <FunctionsIcon sx={{ mr: 1 }} /> },
    // { label: 'Plates', icon: <LayersIcon sx={{ mr: 1 }} /> },
    // { label: 'Buckling', icon: <WarningIcon sx={{ mr: 1 }} /> },
    // { label: 'Dynamic Frequency', icon: <SpeedIcon sx={{ mr: 1 }} /> },
    // { label: 'Near-Node Member Forces', icon: <GroupWorkIcon sx={{ mr: 1 }} /> },
];


export function ResultsInfo({ canvasId }: ResultsInfoProps) {
    const modelState = useAppSelector(
        state => selectModelResponseByCanvasId(state, canvasId)
    ) as ModelState | null;
    const resultSetIds = useMemo(() =>
        modelState ? Object.keys(modelState.resultSets) : [],
        [modelState]
    );
    const [selectedResultSet, setSelectedResultSet] = useState(resultSetIds[0] ?? '');

    const handleButtonClick = (type: string) => {
        alert(`Clicked ${type}`);
    };

    return (
        <Box sx={{ flex: 1, px: 2, overflowY: "auto" }}>
            <Typography variant="subtitle2" sx={{ my: 1 }}>
                Result Set
            </Typography>
            <div className="px-3">
                <Select
                    labelId="result-set-select-label"
                    value={selectedResultSet}
                    label="Result Set"
                    onChange={e => { setSelectedResultSet(e.target.value); }}
                    sx={{ width: '100%' }}
                >
                    {resultSetIds.map((id: string) => (
                        <MenuItem key={id} value={id}>{id}</MenuItem>
                    ))}
                </Select>
            </div>
            <List>
                {resultTypes.map(({ label, icon }) => (
                    <ListItem disablePadding key={label}>
                        <Button
                            startIcon={icon}
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
                            onClick={() => { handleButtonClick(label); }}
                        >
                            {label}
                        </Button>
                    </ListItem>
                ))}
            </List>
        </Box>
    );
}

export default ResultsInfo;
