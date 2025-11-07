import type React from "react";
import { useCallback, useState } from "react";
import Drawer from '@mui/material/Drawer';
import useMediaQuery from '@mui/material/useMediaQuery';
import AppBarMain from "../components/AppBarMain";
import ApartmentIcon from '@mui/icons-material/Apartment';
import FunctionsIcon from '@mui/icons-material/Functions';
import ChangeHistoryIcon from '@mui/icons-material/ChangeHistory';

const drawerWidth = 220;

import SelectionInfo from "../features/editors/selection-info/SelectionInfo";
import { Box, Button, IconButton, Tooltip } from "@mui/material";
import DarkPaper from "../components/DarkPaper";
import ResultsInfo from "../features/results-viewing/ResultsInfo";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectModelResponseByCanvasId } from "../features/editors/editorsSlice";
import { handleRunAnalysis } from "../features/results-viewing/handleRunAnalysis";
import { useApiClient } from "../features/api-client/ApiClientContext";
import { useEditors } from "../features/editors/EditorContext";
import { ProposalsView } from "../features/proposals/ProposalsView";

const ResponsiveIconSidebarLayout: React.FC<{ canvasId: string, children?: React.ReactNode }> = ({ canvasId, children }) => {
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [selectedSidebar, setSelectedSidebar] = useState<string>('physical');
    const isMobile = useMediaQuery('(max-width:900px)');
    const modelState = useAppSelector(
        state => selectModelResponseByCanvasId(state, canvasId)
    )
    const hasResults = modelState?.resultSets && Object.keys(modelState.resultSets).length > 0;

    // Icon sidebar items
    const icons = [
        { id: "physical", label: "Physical Model", icon: <ApartmentIcon />, elementId: "physical-model-view" },
        { id: "analytical", label: "Analytical Model", icon: <FunctionsIcon />, elementId: "analytical-model-view" },
        { id: "proposals", label: "Model Proposals", icon: <ChangeHistoryIcon />, elementId: "model-proposals-view" },
    ];

    // Handle icon click for sidebar selection and mobile toggle
    const handleIconClick = (id: string) => {
        setSelectedSidebar(id);
        if (isMobile) {
            setSidebarOpen(true);
        }
    };

    // Keyboard navigation for sidebar toggle
    const handleKeyDown = (event: React.KeyboardEvent<HTMLButtonElement>) => {
        if (event.key === 'Enter' || event.key === ' ') {
            handleIconClick('nav');
        }
    };

    // Sidebar content for each icon
    const sidebarContents: Record<string, React.ReactNode> = {
        physical: <SelectionInfo canvasId={canvasId} />,
        analytical: hasResults ? <ResultsInfo canvasId={canvasId} /> : <Calculate canvasId={canvasId} />,
        proposals: <ProposalsView canvasId={canvasId} />,
    };

    return (
        // <div className="flex flex-col h-screen w-screen">
        <div className="flex flex-col h-full w-full">
            <AppBarMain />
            <div className="flex flex-row h-full w-full">
                <DarkPaper>
                    {/* Icon Sidebar with highlight and separator */}
                    <div className="flex flex-col items-center" style={{ minWidth: 56, paddingTop: '8px' }}>
                        {icons.map(icon => (
                            <Tooltip key={icon.id} title={icon.label} placement="right" arrow>
                                <IconButton
                                    id={icon.elementId}
                                    onClick={() => { handleIconClick(icon.id); }}
                                    onKeyDown={handleKeyDown}
                                    aria-label={icon.label}
                                    // size="large"
                                    sx={{
                                        color: selectedSidebar === icon.id ? 'primary.contrastText' : 'grey.400',
                                        backgroundColor: selectedSidebar === icon.id ? 'primary.main' : 'inherit',
                                        boxShadow: selectedSidebar === icon.id ? 2 : 0,
                                        marginBottom: 2,
                                        width: 48,
                                        height: 48,
                                        '&:hover': {
                                            backgroundColor: selectedSidebar === icon.id ? 'primary.dark' : 'grey.900',
                                        },
                                    }}
                                >
                                    {icon.icon}
                                </IconButton>
                            </Tooltip>
                        ))}
                    </div>
                </DarkPaper>
                {/* Main Sidebar Content with left shadow for separation */}
                {isMobile ? (
                    <Drawer
                        id="main-sidebar"
                        anchor="left"
                        open={sidebarOpen}
                        onClose={() => { setSidebarOpen(false); }}
                        ModalProps={{ keepMounted: true }}
                        sx={{
                            '& .MuiDrawer-paper': {
                                width: drawerWidth,
                                boxShadow: '4px 0 12px rgba(0,0,0,0.12)',
                                borderLeft: '1px solid #222',
                            },
                            width: drawerWidth,
                        }}
                        role="dialog"
                        aria-modal="true"
                        aria-label="Sidebar content"
                    >
                        {sidebarContents[selectedSidebar]}
                    </Drawer>
                ) : (
                    <div id="main-sidebar">
                        <DarkPaper className="w-96 shadow-lg">
                            {sidebarContents[selectedSidebar]}
                        </DarkPaper>
                    </div>
                )}
                {/* Main Content */}
                <div className="h-full w-full relative">
                    {children}
                </div>
            </div>
        </div>
    );
};

export default ResponsiveIconSidebarLayout;

export function Calculate({ canvasId }: { canvasId: string }) {
    const modelState = useAppSelector(
        state => selectModelResponseByCanvasId(state, canvasId)
    )
    const dispatch = useAppDispatch()
    const apiClient = useApiClient()
    const editor = useEditors()[canvasId];

    const handleRunAnalysisFunc = useCallback(async () => {
        if (!modelState) {
            return;
        }
        console.log("Running analysis...");
        await handleRunAnalysis(apiClient, dispatch, modelState.id, canvasId, editor);
        console.log("Analysis complete.");
        // Implement the analysis logic here
    }, [apiClient, canvasId, dispatch, editor, modelState]);

    return (
        <Box
            sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                height: '100%',
                width: '100%',
            }}
        >
            <Button variant="contained" color="primary" onClick={() => { console.log("Button clicked"); void handleRunAnalysisFunc() }}>
                Analyze
            </Button>
        </Box>
    );
}
